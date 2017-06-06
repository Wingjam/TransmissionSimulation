using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransmissionSimulation.Helpers;
using TransmissionSimulation.Models;
using TransmissionSimulation.Ressources;

namespace TransmissionSimulation.Components
{
    class Station
    {
        /* Constructor initialized fields */
        Constants.Station stationType;
        ITransmitter transmitter;
        Dictionary<int, Frame> inputBuffer;
        Dictionary<int, Frame> outputBuffer;
        int bufferSize;
        int timeoutInMs;
        FileStream fileStream;

        /* Constants */

        /// <summary>
        /// Size of the data section of a frame in bytes
        /// </summary>
        int DataSizeInFrame;

        /// <summary>
        /// Maximum sequence number for frame ids. We use the value going from 0 to MaxSequence as ids for frame in order to keep the Id field of the Frame class from being too big. We reuse ids starting from 0 when we reach the MaxSequence.
        /// </summary>
        UInt16 MaxSequence;

        /// <summary>
        /// Timeout in milliseconds during which we hope that a Data frame will be sent so that we can add the ACK sequence number inside it.
        /// </summary>
        int AckTimeout { get { return bufferSize / 2; } }


        /* Internal fields for treatment */

        /* Sender fields */
        UInt16 NextAwaitedAckSequenceNumber { get; set; }
        UInt16 NextFrameToSendSequenceNumber { get; set; }
        bool FrameReadyToSend {
            get {
                // TODO Implement logic to check if there is still frames to send or if the file is completly sent.
                // NOTE : Must work for Sender Station and Receiver Station (Data Frame, Ack Frame, Nak frame)

                // We can send another frame if our outputBuffer isn't full. Valid only it we are sending a data frame. An Ack frame doesn't need this validation.
                return outputBuffer.Count < bufferSize;
            }
        }


        /* Receiver fields */
        // ****************************************************************************************
        // TODO These fields should take values going from 0 to MaxSequence - 1 and an overflow should take them back to 0. Should be implemented in setter?                   
        // ****************************************************************************************
        UInt16 NextAwaitedFrameSequenceNumber { get; set; }
        UInt16 LastFrameToSaveSequenceNumber
        {
            get { return (UInt16)(NextAwaitedFrameSequenceNumber + bufferSize); }
        }
        UInt16 LastFrameSequenceNumberForNak { get; set; }
        bool NoNakSentForNextAwaitedFrame
        {
            get { return LastFrameSequenceNumberForNak != NextAwaitedFrameSequenceNumber; }
        }
        /// <summary>
        /// Next nak to send
        /// </summary>
        Frame NakToSend { get; set; }

        /// <summary>
        /// Timer that enforce that we send an ACK frame if no Data frame is sent fast enough to communicate the Ack  
        /// </summary>
        System.Timers.Timer AckTimer { get; set; }
        bool SendAck { get; set; }


        public Station(Constants.Station stationType, ITransmitter transmitter, int bufferSize, int timeoutInMs, FileStream fileStream)
        {
            this.stationType = stationType;
            this.transmitter = transmitter;
            this.bufferSize = bufferSize;
            inputBuffer = new Dictionary<int, Frame>();
            outputBuffer = new Dictionary<int, Frame>();
            this.timeoutInMs = timeoutInMs;
            this.fileStream = fileStream;

            // Initialize constants
            int frameSizeBeforeHamming = HammingHelper.GetDataSize(Constants.FrameSize);
            int frameHeaderSize = Frame.HeaderSize();
            DataSizeInFrame = frameSizeBeforeHamming - frameHeaderSize;
            MaxSequence = (UInt16)(bufferSize * 2 + 1);

            // Initialize fields
            NextAwaitedAckSequenceNumber = 0;
            NextFrameToSendSequenceNumber = 0;
            NextAwaitedFrameSequenceNumber = 0;

            // Since frames cannot use MaxSequence as a sequence number, we use it for LastFrameSequenceNumberForNak.
            // This is meant to ensure that no Frame matches this field if there was no Nak sent.
            LastFrameSequenceNumberForNak = MaxSequence;

            // Initialise ack timer logic
            AckTimer = new System.Timers.Timer(AckTimeout);
            // When the timer ends, we need to inform the program that we need to send an Ack now. Also stop the timer.
            AckTimer.Elapsed += (sender, e) => { SendAck = true; AckTimer.Stop(); };
            SendAck = false;
        }

        public void Start()
        {
            while (true)
            {
                //events (in order of priority):
                // - nak is ready to be sent, we send it right now
                // - ready to send on wire and frame to send available
                // - ack timer (receiver : we weren't able to send the ack with a data frame, we need to send it now!)
                // - data received on wire (correct or corrupt)
                // - timeout (sender : haven't received my ack yet, resend frame)

                // timer needs:
                // ack: 
                // - nothing
                // - cancel : canceled whenever a frame is sent. There is only one instance of this timer at all times.
                // - activate : there is no timer currently and we receive a frame
                // timeout:
                // - expired frame sequence number
                // - cancel : cancel with a 

                if (transmitter.TransmitterReady(stationType) && NakToSend != null)
                {
                    // Send Nak
                    SendFrame(NakToSend);

                    // Erase NakToSend because it has been sent.
                    NakToSend = null;
                }
                else if (transmitter.TransmitterReady(stationType) && FrameReadyToSend) // - ready to send on wire and frame to send available
                {
                    // TODO check this, we need to send the right Ack, especially when there is no Ack at all!!! The sender must not send something invalid
                    UInt16 ack = (UInt16)(NextAwaitedAckSequenceNumber - 1);
                    Frame nextFrame = BuildDataFrame(NextFrameToSendSequenceNumber, ack);

                    // if we are sending a frame, we have the Ack in it, so we do not need to send the ack later anymore. We stop the timer.
                    AckTimer.Stop();

                    // Makes sure that we are not sending an Ack. Important because the AckTimer could have ended before we canceled it.
                    SendAck = false;

                    // Mark the frame as sent and keep a reference on it in the outputBuffer to show that we are awaiting an Ack for this frame.
                    outputBuffer.Add(nextFrame.Id, nextFrame);

                    // Send the frame
                    SendFrame(nextFrame);

                    // Increment the frame to send sequence number because we have sent the current one.
                    NextFrameToSendSequenceNumber++;
                }
                else if (transmitter.TransmitterReady(stationType) && SendAck) // - ack timer (receiver : we weren't able to send the ack with a data frame, we need to send it now!)
                {
                    // Build an Ack frame
                    Frame ackFrame = new Frame(Constants.FrameType.Ack, (UInt16)(NextAwaitedFrameSequenceNumber - 1));

                    // TODO Check to make sure we do not need to Stop the timer.
                    // Early reflexion : should not do it -> if we are here it's because the timer ticked and flipped the SendAck to true, so it stopped itself. Logically, we shouldn't start it again if the SendAck is true, so we know it's off.
                    // AckTimer.Stop();

                    // Inform the program that we did send the Ack
                    SendAck = false;

                    SendFrame(ackFrame);
                }
                else if (transmitter.DataReceived(stationType)) // data received on wire (correct or corrupt)
                {
                    Frame frameReceived = GetFrameFromTransmitter();

                    if (frameReceived == null)
                    {
                        // The frame was corrupted, we prepare a NAK, but only if we have not sent another one already
                        if (NoNakSentForNextAwaitedFrame)
                        {
                            // Set the LastFrameSequenceNumberForNak to the currently awaited frame's sequence number
                            LastFrameSequenceNumberForNak = NextAwaitedFrameSequenceNumber;

                            // Build a NAK frame
                            Frame nakFrame = new Frame(Constants.FrameType.Nak, LastFrameSequenceNumberForNak);

                            // Set the NakToSend to nakFrame to send it next
                            NakToSend = nakFrame;
                        }
                    }
                    else
                    {
                        // The frame is correct

                        if (frameReceived.Type == Constants.FrameType.Data)
                        {
                            // TODO Check if the frame id fits in the input buffer. If it does not, we ignore its content, but still take its ack value

                            // we can add it to the input buffer if not already there
                            if (!inputBuffer.ContainsKey(frameReceived.Id))
                            {
                                inputBuffer.Add(frameReceived.Id, frameReceived);
                            }

                            // Try to pass data to the superior layer (in the file)
                            while (inputBuffer.ContainsKey(NextAwaitedFrameSequenceNumber))
                            {
                                // TODO Write to file the frame data


                                // Remove the frame from the input buffer
                                inputBuffer.Remove(NextAwaitedFrameSequenceNumber);

                                // Increment the awaited frame sequence number because this one has been treated
                                NextAwaitedFrameSequenceNumber++;
                            }

                            // TODO Send an ack for NextAwaitedFrameSequenceNumber - 1


                            // TODO Update the Ack value with the Ack in the frame

                        }
                    }
                }
                else if (true) // timeout (sender : haven't received my ack yet, resend frame)
                {

                }


            }
        }
        private void SendData()
        {
            //int dataSizeInFrame = HammingHelper.GetDataSize(Constants.FrameSize) / 8 - 32;
            //byte[] fileReadBuffer = new byte[dataSizeInFrame];
            //int numberOfFrameSent = 0;

            //int numberBytesRead = fileStream.Read(fileReadBuffer, 0, dataSizeInFrame);
            //while (numberBytesRead > 0)
            //{
            //    Frame frame = new Frame(new BitArray(fileReadBuffer));

            //    byte[] frameInByteArray = ObjectToByteArray(frame);
            //    BitArray formattedFrameInByteArray = HammingHelper.Encrypt(new BitArray(frameInByteArray));
            //    buffer.Enqueue(formattedFrameInByteArray);

            //    numberBytesRead = fileStream.Read(fileReadBuffer, Math.Max(numberOfFrameSent * Constants.FrameSize, (int)fileStream.Length - 1), Constants.FrameSize);
            //}

            //allFrameBuffered = true;

            
        }

        /// <summary>
        /// Encode the frame and send it over to the transmitter.
        /// </summary>
        /// <param name="frame"></param>
        private void SendFrame(Frame frame)
        {
            // Prepare the frame to be sent on the wire (converts to BitArray and encode for error control with Hamming)
            BitArray frameBitArray = new BitArray(ObjectToByteArray(frame));
            BitArray encodedFrameBitArray = HammingHelper.Encrypt(frameBitArray);

            // Send the data
            transmitter.SendData(encodedFrameBitArray, stationType);
        }

        private Frame BuildDataFrame(UInt16 numSequence, UInt16 ack)
        {
            BitArray data = new BitArray(DataSizeInFrame);

            // Fill data with next file chunk
            // TODO

            return new Frame(numSequence, Constants.FrameType.Data, ack, data);
        }

        /// <summary>
        /// Get the frame from the transmitter.
        /// </summary>
        /// <returns>The decoded Frame. Null if Frame was corrupted or there was no data in the transmitter.</returns>
        private Frame GetFrameFromTransmitter()
        {
            if (transmitter.DataReceived(stationType))
            {
                // there is indeed a data, we are going to get it
                BitArray encodedFrameBitArray = transmitter.GetData(stationType);

                // ****************************************************
                // TODO Check if data is corrupted (NEED JONATHAN TO ADD A SERVICE TO ITS HAMMING HELPER)
                // ****************************************************
                bool isCorrupted = false;
                if (isCorrupted)
                {
                    return null;
                }

                // Decode the frame
                BitArray frameBitArray = HammingHelper.Decrypt(encodedFrameBitArray);

                // Converts BitArray to Frame
                byte[] frameByteArray = new byte[frameBitArray.Length];
                frameBitArray.CopyTo(frameByteArray, 0);
                Frame frame = (Frame)ByteArrayToObject(frameByteArray);

                return frame;
            }
            
            return null;
        }

        // Convert an object to a byte array
        private byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        // Convert a byte array to an Object
        private Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}
