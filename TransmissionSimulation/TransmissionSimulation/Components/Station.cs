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
        int BufferSize { get; set; }
        int TimeoutInMs { get; set; }
        FileStream fileStream;
        Constants.ShowFrameDelegate sendFrameDelegate;

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
        int AckTimeout {
            get {
                // TODO Check if this ratio is valid 
                return TimeoutInMs / 2;
            } }


        /* Internal fields for treatment */

        /* Sender fields */
        UInt16 NextAwaitedAckSequenceNumber { get; set; }
        UInt16 NextFrameToSendSequenceNumber { get; set; }
        bool FrameReadyToSend {
            get {
                if (stationType != Constants.Station.Source)
                {
                    return false;
                }

                // Check if there is still frames to send or if the file is completly sent.
                bool fileNotEntirelySent = fileStream.Position < fileStream.Length;

                // We can send another frame if our output window isn't full. Valid only it we are sending a data frame. An Ack frame doesn't need this validation.
                int outputWindowSize = NextFrameToSendSequenceNumber - NextAwaitedAckSequenceNumber;
                if (outputWindowSize < 0)
                {
                    outputWindowSize += MaxSequence;
                }
                return fileNotEntirelySent && outputWindowSize < BufferSize;
            }
        }


        /* Receiver fields */
        UInt16 NextAwaitedFrameSequenceNumber { get; set; }
        UInt16 LastFrameToSaveSequenceNumber
        {
            get { return (UInt16)(NextAwaitedFrameSequenceNumber + BufferSize); }
        }
        UInt16 LastFrameSequenceNumberForNak { get; set; }
        bool NoNakSentForNextAwaitedFrame
        {
            get { return LastFrameSequenceNumberForNak != NextAwaitedFrameSequenceNumber; }
        }

        /// <summary>
        /// Timer that enforce that we send an ACK frame if no Data frame is sent fast enough to communicate the Ack  
        /// </summary>
        System.Timers.Timer AckTimer { get; set; }
        volatile bool sendAck;

        /// <summary>
        /// A Queue containing the high priority frames. These frames should be sent before any other frame, as soon as the transmitter is ready.
        /// </summary>
        Queue<Frame> HighPriorityFrames { get; set; }

        ConcurrentDictionary<UInt16, System.Timers.Timer> TimeoutTimers { get; set; }


        public Station(Constants.Station stationType, ITransmitter transmitter, int bufferSize, int timeoutInMs, FileStream fileStream, Constants.ShowFrameDelegate sendFrame)
        {
            this.stationType = stationType;
            this.transmitter = transmitter;
            this.BufferSize = bufferSize;
            inputBuffer = new Dictionary<int, Frame>();
            outputBuffer = new Dictionary<int, Frame>();
            this.TimeoutInMs = timeoutInMs;
            this.fileStream = fileStream;
            this.sendFrameDelegate = sendFrame;

            // Initialize constants
            int frameSizeBeforeHamming = HammingHelper.GetDataSize(Constants.FrameSize * 8) / 8;
            int frameHeaderSize = Frame.HeaderSize();
            DataSizeInFrame = frameSizeBeforeHamming - frameHeaderSize;
            MaxSequence = (UInt16)(bufferSize * 2 + 1);

            // Initialize fields
            NextAwaitedAckSequenceNumber = MaxSequence;
            NextFrameToSendSequenceNumber = 0;
            NextAwaitedFrameSequenceNumber = 0;

            // Since frames cannot use MaxSequence as a sequence number, we use it for LastFrameSequenceNumberForNak.
            // This is meant to ensure that no Frame matches this field if there was no Nak sent.
            LastFrameSequenceNumberForNak = MaxSequence;

            // Initialise ack timer logic
            AckTimer = new System.Timers.Timer(AckTimeout);
            // When the timer ends, we need to inform the program that we need to send an Ack now. Also stop the timer.
            AckTimer.Elapsed += (sender, e) => { sendAck = true; AckTimer.Stop(); };
            sendAck = false;

            HighPriorityFrames = new Queue<Frame>();
            TimeoutTimers = new ConcurrentDictionary<UInt16, System.Timers.Timer>();
        }

        public void Start()
        {
            while (true)
            {
                //events (in order of priority):
                // - high priority frame ready (examples : nak is ready to be sent, we need to resend a frame for which a nak was received, we need to resend a frame for which the timeout occured)
                // - ready to send on wire and frame to send available
                // - ack timer (receiver : we weren't able to send the ack with a data frame, we need to send it now!)
                // - data received on wire (correct or corrupt)
                //checks to do every time:
                // - check timeouts (sender : haven't received my ack yet, resend frame soon)

                // timer needs:
                // ack: 
                // - nothing
                // - cancel : canceled whenever a frame is sent. There is only one instance of this timer at all times.
                // - activate : there is no timer currently and we receive a frame
                // timeout:
                // - expired frame sequence number
                // - cancel : cancel with a 

                bool transmitterReady = transmitter.TransmitterReady(stationType);
                bool transmitterDataReceived = transmitter.DataReceived(stationType);
                if (transmitterReady && HighPriorityFrames.Count > 0)
                {
                    // Gets next high priority frame
                    Frame frame = HighPriorityFrames.Dequeue();

                    // Update frame Ack to latest Ack
                    frame.Ack = DecrementSequenceNumber(NextAwaitedAckSequenceNumber);

                    // Send the frame
                    SendFrame(frame);
                }
                else if (transmitterReady && FrameReadyToSend) // - ready to send on wire and frame to send available
                {
                    // TODO check this, we need to send the right Ack, especially when there is no Ack at all!!! The sender must not send something invalid
                    UInt16 ack = DecrementSequenceNumber(NextAwaitedFrameSequenceNumber);
                    Frame nextFrame = BuildDataFrame(NextFrameToSendSequenceNumber, ack);

                    // if we are sending a frame, we have the Ack in it, so we do not need to send the ack later anymore. We stop the timer.
                    AckTimer.Stop();

                    // Makes sure that we are not sending an Ack later, it is included in the current frame. Important because the AckTimer could have ended before we canceled it.
                    sendAck = false;

                    // Mark the frame as sent and keep a reference on it in the outputBuffer to show that we are awaiting an Ack for this frame.
                    outputBuffer.Add(nextFrame.Id % BufferSize, nextFrame);

                    // Send the frame
                    SendFrame(nextFrame);

                    // Register a timeout timer for the sent frame
                    RegisterTimeout(NextFrameToSendSequenceNumber);

                    // Increment the frame to send sequence number because we have sent the current one.
                    NextFrameToSendSequenceNumber = IncrementSequenceNumber(NextFrameToSendSequenceNumber);
                }
                else if (transmitterReady && sendAck) // - ack timer (receiver : we weren't able to send the ack with a data frame, we need to send it now!)
                {
                    // TODO (later) Use HighPriorityQueue instead of doing it here with a boolean (sendAck). This could unify logic, but watch out for concurrency!!

                    // Build an Ack frame
                    Frame ackFrame = new Frame(Constants.FrameType.Ack, DecrementSequenceNumber(NextAwaitedFrameSequenceNumber));

                    // TODO Check to make sure we do not need to Stop the timer.
                    // Early reflexion : should not do it -> if we are here it's because the timer ticked and flipped the SendAck to true, so it stopped itself. Logically, we shouldn't start it again if the SendAck is true, so we know it's off.
                    // AckTimer.Stop();

                    // Inform the program that we did send the Ack
                    sendAck = false;

                    SendFrame(ackFrame);
                }
                else if (transmitterDataReceived) // data received on wire (correct or corrupt)
                {
                    Frame frameReceived = GetFrameFromTransmitter();

                    if (frameReceived == null)
                    {
                        // The frame was corrupted, we prepare a NAK, but only if we have not sent another one already
                        if (NoNakSentForNextAwaitedFrame)
                        {
                            // Set the LastFrameSequenceNumberForNak to the currently awaited frame's sequence number
                            LastFrameSequenceNumberForNak = NextAwaitedFrameSequenceNumber;

                            // Build a Nak frame
                            Frame nakFrame = new Frame(Constants.FrameType.Nak, DecrementSequenceNumber(NextAwaitedFrameSequenceNumber));

                            // Put the Nak frame in the high priority queue to send it as soon as possible
                            HighPriorityFrames.Enqueue(nakFrame);
                        }
                    }
                    else
                    {
                        // The frame is correct

                        if (frameReceived.Type == Constants.FrameType.Data)
                        {
                            // If the frame is not the Awaited frame, we preemptively prepare a Nak, but only if no Nak was sent already, because the Awaited frame was probably lost
                            if (frameReceived.Id != NextAwaitedFrameSequenceNumber && NoNakSentForNextAwaitedFrame)
                            {
                                // Set the LastFrameSequenceNumberForNak to the currently awaited frame's sequence number
                                LastFrameSequenceNumberForNak = NextAwaitedFrameSequenceNumber;

                                // Build a NAK frame
                                Frame nakFrame = new Frame(Constants.FrameType.Nak, DecrementSequenceNumber(NextAwaitedFrameSequenceNumber));

                                // Put the Nak frame in the high priority queue to send it as soon as possible
                                HighPriorityFrames.Enqueue(nakFrame);
                            }

                            // Check if the frame id fits in the input buffer. If it does not, we ignore its data
                            if (IsBetween(NextAwaitedAckSequenceNumber, frameReceived.Id, LastFrameToSaveSequenceNumber))
                            {
                                // we can add it to the input buffer if not already there
                                if (!inputBuffer.ContainsKey(frameReceived.Id % BufferSize))
                                {
                                    inputBuffer.Add(frameReceived.Id % BufferSize, frameReceived);
                                }
                            }

                            // Try to pass data to the superior layer (in the fileStream) if we have the next ordered frames
                            while (inputBuffer.ContainsKey(NextAwaitedFrameSequenceNumber % BufferSize))
                            {
                                // TODO Add validation for file write

                                // Write to frame data to the file
                                byte[] frameData = new byte[frameReceived.Data.Length / 8];
                                frameReceived.Data.CopyTo(frameData, 0);
                                fileStream.Write(frameData, 0, frameReceived.Data.Length / 8);
                                fileStream.Flush();

                                // Remove the frame from the input buffer
                                inputBuffer.Remove(NextAwaitedFrameSequenceNumber % BufferSize);

                                // Increment the awaited frame sequence number because this one has been treated. This also reset the LastFrameSequenceNumberForNak value so that it is not mistaken for another Frame with the same sequence number later on.
                                NextAwaitedFrameSequenceNumber = IncrementSequenceNumber(NextAwaitedFrameSequenceNumber);
                                // Reset LastFrameSequenceNumberForNak to impossible value, because we changed the NextAwaitedFrameSequenceNumber
                                LastFrameSequenceNumberForNak = MaxSequence;

                                // Start timer for an ack, but only if not already started
                                if (!AckTimer.Enabled && !sendAck)
                                {
                                    AckTimer.Start();
                                }
                            }
                        }
                        else if (frameReceived.Type == Constants.FrameType.Nak)
                        {
                            // frameReceived.Ack represent the last Acknoloedged frame by the receiver, so frameReceived.Ack + 1 is the one the Nak was aiming at.
                            UInt16 nakSequenceNumber = IncrementSequenceNumber(frameReceived.Ack);
                            
                            if (IsBetween(NextAwaitedAckSequenceNumber, nakSequenceNumber, NextFrameToSendSequenceNumber)) // valid sequence number for current window
                            {
                                if (outputBuffer.ContainsKey(nakSequenceNumber % BufferSize))
                                {
                                    // If Nak refers to a frame in the outputBuffer, this mean it is indeed a frame that we sent earlier. We need to send it again very soon
                                    HighPriorityFrames.Enqueue(outputBuffer[nakSequenceNumber % BufferSize]);
                                }
                            }
                        }

                        // Update the NextAwaitedAckSequenceNumber value with the Ack in the frame. 
                        while (IsBetween(NextAwaitedAckSequenceNumber, frameReceived.Ack, LastFrameToSaveSequenceNumber))
                        {
                            System.Timers.Timer timeoutTimer;
                            // Remove the timeout timer associated with this frame sequence number
                            if (TimeoutTimers.TryRemove(NextAwaitedAckSequenceNumber, out timeoutTimer))
                            {
                                timeoutTimer.Stop();
                            }

                            outputBuffer.Remove(NextAwaitedAckSequenceNumber % BufferSize);

                            NextAwaitedAckSequenceNumber = IncrementSequenceNumber(NextAwaitedAckSequenceNumber);
                        }
                    }
                }

                // Every time we iterate, we need to check for timeout events in the TimeoutTimers dictionnary.
                if (TimeoutTimers.Any(x => x.Value.Enabled == false)) // timeout (sender : haven't received my ack yet, resend frame)
                {
                    // Check if any timeout occured on frames that were sent
                    foreach (KeyValuePair<UInt16, System.Timers.Timer> finishedTimeoutTimer in TimeoutTimers.Where(x => x.Value.Enabled == false))
                    {
                        // Get the expired frame to resend
                        Frame frameToResend = outputBuffer[finishedTimeoutTimer.Key % BufferSize];

                        // Send as soon as possible
                        HighPriorityFrames.Enqueue(frameToResend);

                        // Remove the timeout from the dictionnary
                        System.Timers.Timer temp = new System.Timers.Timer();
                        TimeoutTimers.TryRemove(finishedTimeoutTimer.Key, out temp);
                    }
                }
            }
        }

        private void RegisterTimeout(UInt16 sequenceNumber)
        {
            // The convention is that a stopped timer means the timeout occured and we should resend the frame.
            System.Timers.Timer timeoutTimer = new System.Timers.Timer(TimeoutInMs);

            timeoutTimer.Elapsed += (sender, e) => { timeoutTimer.Stop(); };
            timeoutTimer.Start();

            TimeoutTimers.TryAdd(sequenceNumber, timeoutTimer);
        }

        private bool IsBetween(UInt16 beginning, UInt16 middle, UInt16 end)
        {
            return (beginning <= middle && middle < end) || (end < beginning && beginning <= middle) || (middle < end && end < beginning);
        }

        private UInt16 IncrementSequenceNumber(UInt16 sequenceNumber)
        {
            return (UInt16)(sequenceNumber + 1 < MaxSequence ? sequenceNumber + 1 : 0);
        }

        private UInt16 DecrementSequenceNumber(UInt16 sequenceNumber)
        {
            return (UInt16)(sequenceNumber - 1 > 0 ? sequenceNumber - 1 : MaxSequence - 1);
        }

        /// <summary>
        /// Encode the frame and send it over to the transmitter.
        /// </summary>
        /// <param name="frame"></param>
        private void SendFrame(Frame frame)
        {
            // Prepare the frame to be sent on the wire (converts to BitArray and encode for error control with Hamming)
            BitArray frameBitArray = frame.GetFrameAsByteArray();
            BitArray encodedFrameBitArray = HammingHelper.Encrypt(frameBitArray);

            // Notify subscriber that frame is being sent
            sendFrameDelegate(frame, true);

            Console.WriteLine("{5, 11} {0, 12} : id={1, 2}, type={2, 4}, ack={3, 2}, data lenght={4, 3}", "SendFrame", frame.Id, frame.Type.ToString(), frame.Ack, frame.Data.Count, stationType == Constants.Station.Dest ? "Destination" : "Source");

            // Send the data
            transmitter.SendData(encodedFrameBitArray, stationType);
        }

        private Frame BuildDataFrame(UInt16 numSequence, UInt16 ack)
        {
            // TODO Add validation for file read

            // Fill data with next file chunk
            byte[] data = new byte[DataSizeInFrame];
            int actuallyReadBytesAmount = fileStream.Read(data, 0, DataSizeInFrame);
            BitArray dataBitArray = new BitArray(data);

            return new Frame(numSequence, Constants.FrameType.Data, ack, dataBitArray);
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
                Frame frame = Frame.GetFrameFromBitArray(frameBitArray);

                // Notify subscriber that frame is being received
                sendFrameDelegate(frame, false);

                Console.WriteLine("{5, 11} {0, 12} : id={1, 2}, type={2, 4}, ack={3, 2}, data lenght={4, 3}", "ReceiveFrame", frame.Id, frame.Type.ToString(), frame.Ack, frame.Data.Count, stationType == Constants.Station.Dest ? "Destination" : "Source");

                return frame;
            }
            
            return null;
        }
    }
}
