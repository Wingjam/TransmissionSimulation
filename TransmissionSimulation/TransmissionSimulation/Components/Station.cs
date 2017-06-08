using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TransmissionSimulation.Helpers;
using TransmissionSimulation.Models;
using TransmissionSimulation.Ressources;

namespace TransmissionSimulation.Components
{
    class Station
    {
        #region Referenced objects

        ITransmitter transmitter;
        FileStream inputFileStream;
        FileStream outputFileStream;
        Constants.ShowFrameDelegate sendFrameDelegate;

        #endregion

        #region Internal structures

        /// <summary>
        /// Input buffer that store received frames before sending them to the file stream
        /// </summary>
        Dictionary<int, Frame> InputBuffer { get; set; }

        /// <summary>
        /// Output buffer that store sent frames when waiting for an acknowledgement for them. All the frames in this buffer have not been acknowledged.
        /// </summary>
        Dictionary<int, Frame> OutputBuffer { get; set; }

        /// <summary>
        /// A Queue containing the high priority frames. These frames should be sent before any other frame, as soon as the transmitter is ready.
        /// </summary>
        Queue<Frame> HighPriorityFrames { get; set; }

        /// <summary>
        /// Dictionnary containing all the timeout timers of the frames that have been sent but not acknowledged (with the sequence number as the key). A stopped timer in this dictionnary means an expired timeout.
        /// </summary>
        ConcurrentDictionary<UInt16, System.Timers.Timer> TimeoutTimers { get; set; }

        #endregion

        #region Configuration fields

        /// <summary>
        /// Station identification
        /// </summary>
        Constants.Station StationIdenfication { get; set; }

        /// <summary>
        /// Size of the input and output buffers
        /// </summary>
        int BufferSize { get; set; }

        /// <summary>
        /// Timeout in miliseconds before resending a frame that was no acknoledged by the receiver
        /// </summary>
        int TimeoutInMs { get; set; }

        #endregion

        #region Constants

        /// <summary>
        /// Maximum sequence number for frame ids. We use the value going from 0 to MaxSequence as ids for frame in order to keep the Id field of the Frame class from being too big. We reuse ids starting from 0 when we reach the MaxSequence.
        /// </summary>
        UInt16 MaxSequence;

        /// <summary>
        /// Size of the data section of a frame in bytes
        /// </summary>
        int DataSizeInFrame;

        /// <summary>
        /// Number of bits at the end of an encoded frame to ensure the frame is of the right size.
        /// </summary>
        int EncodedFramePadding { get; set; }

        /// <summary>
        /// Timeout in milliseconds during which we hope that a Data frame will be sent so that we can add the ACK sequence number inside it. At the end of this timer, we will sent an Ack.
        /// </summary>
        int AckTimeout {
            get {
                // TODO Check if this ratio is valid 
                return TimeoutInMs / 2;
            } }

        #endregion

        #region Sender fields

        /// <summary>
        /// Sequence number of the first frame of the current window. We are awaiting an ack for this frame.
        /// </summary>
        UInt16 FirstFrameSent { get; set; }

        /// <summary>
        /// Next available sequence number to use when sending the next data frame.
        /// </summary>
        UInt16 NextFrameToSendSequenceNumber { get; set; }

        /// <summary>
        /// Boolean indicating whether we are ready to send data or not
        /// </summary>
        bool FrameReadyToSend {
            get {
                // Check if there is still frames to send or if the file is completly sent.
                bool fileNotEntirelySent = inputFileStream.Position < inputFileStream.Length;

                // We can send another frame if our output window isn't full. Valid only it we are sending a data frame. An Ack frame doesn't need this validation.
                int outputWindowSize = NextFrameToSendSequenceNumber - FirstFrameSent;
                if (outputWindowSize < 0)
                {
                    outputWindowSize += MaxSequence;
                }
                return fileNotEntirelySent && outputWindowSize < BufferSize;
            }
        }

        #endregion

        #region Receiver fields

        /// <summary>
        /// Sequence number of the next frame to receive. We are expecting this frame next, otherwise we will send a Nak
        /// </summary>
        UInt16 NextFrameToReceive { get; set; }

        /// <summary>
        /// Sequence number of the last frame to receive in the current window. Any frame with a higher sequence number will be rejected.
        /// </summary>
        UInt16 LastFrameToReceive
        {
            get { return (UInt16)(NextFrameToReceive + BufferSize); }
        }

        /// <summary>
        /// Sequence number of the last frame for which we sent a Nak. Will be reseted to MaxSequence when modifying NextFrameToReceive or no Nak sent.
        /// </summary>
        UInt16 LastFrameSequenceNumberForNak { get; set; }

        /// <summary>
        /// Boolean indicating that no nak was sent for the current frame to receive.
        /// </summary>
        bool NoNakSentForNextAwaitedFrame
        {
            get { return LastFrameSequenceNumberForNak != NextFrameToReceive; }
        }

        /// <summary>
        /// Timer that enforce that we send an ACK frame if no Data frame is sent fast enough to communicate the Ack  
        /// </summary>
        System.Timers.Timer AckTimer { get; set; }
        volatile bool sendAck;

        #endregion

        #region Constructors

        public Station(
            Constants.Station stationType,
            ITransmitter transmitter,
            int bufferSize,
            int timeoutInMs,
            FileStream inputFileStream,
            FileStream outputFileStream,
            Constants.ShowFrameDelegate sendFrame)
        {
            this.StationIdenfication = stationType;
            this.transmitter = transmitter;
            this.BufferSize = bufferSize;
            InputBuffer = new Dictionary<int, Frame>();
            OutputBuffer = new Dictionary<int, Frame>();
            this.TimeoutInMs = timeoutInMs;
            this.inputFileStream = inputFileStream;
            this.outputFileStream = outputFileStream;
            this.sendFrameDelegate = sendFrame;

            // Initialize constants
            int frameSizeBeforeHamming = HammingHelper.GetDataSize(Constants.FrameSize * 8) / 8;
            int realBitCount = HammingHelper.GetTotalSize(frameSizeBeforeHamming * 8);
            EncodedFramePadding = Constants.FrameSize * 8 - realBitCount;
            int frameHeaderSize = Frame.HeaderSize();
            DataSizeInFrame = frameSizeBeforeHamming - frameHeaderSize;
            MaxSequence = (UInt16)(bufferSize * 2 - 1);

            // Initialize fields
            FirstFrameSent = 0;
            NextFrameToSendSequenceNumber = 0;
            NextFrameToReceive = 0;

            // Since frames cannot use MaxSequence as a sequence number, we use it for LastFrameSequenceNumberForNak.
            // This is meant to ensure that no Frame matches this field if there was no Nak sent.
            LastFrameSequenceNumberForNak = MaxSequence;

            // Initialise ack timer logic
            AckTimer = new System.Timers.Timer(AckTimeout);
            // When the timer ends, we need to inform the program that we need to send an Ack now. Also stop the timer.
            AckTimer.Elapsed += (sender, e) => { sendAck = true; AckTimer.Stop(); Console.WriteLine("ack timer elapsed"); };
            sendAck = false;

            HighPriorityFrames = new Queue<Frame>();
            TimeoutTimers = new ConcurrentDictionary<UInt16, System.Timers.Timer>();
        }

        #endregion

        #region Protocol 6 main logic

        /// <summary>
        /// Starts the Station
        /// </summary>
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

                bool transmitterReady = transmitter.TransmitterReady(StationIdenfication);
                bool transmitterDataReceived = transmitter.DataReceived(StationIdenfication);
                if (transmitterReady && HighPriorityFrames.Count > 0) // - high priority frame ready (examples : nak is ready to be sent, we need to resend a frame for which a nak was received, we need to resend a frame for which the timeout occured)
                {
                    // Gets next high priority frame
                    Frame frame = HighPriorityFrames.Dequeue();

                    // Update frame Ack to latest Ack
                    frame.Ack = DecrementSequenceNumber(NextFrameToReceive);

                    if (frame.Type != Constants.FrameType.Data || OutputBuffer.ContainsKey(frame.Id % BufferSize))
                    {
                        // Send the frame
                        SendFrame(frame);
                    }
                }
                else if (transmitterReady && FrameReadyToSend) // - ready to send on wire and frame to send available
                {
                    // TODO check this, we need to send the right Ack, especially when there is no Ack at all!!! The sender must not send something invalid
                    UInt16 ack = DecrementSequenceNumber(NextFrameToReceive);
                    Frame nextFrame = BuildDataFrame(NextFrameToSendSequenceNumber, ack);

                    // if we are sending a frame, we have the Ack in it, so we do not need to send the ack later anymore. We stop the timer.
                    AckTimer.Stop();

                    // Makes sure that we are not sending an Ack later, it is included in the current frame. Important because the AckTimer could have ended before we canceled it.
                    sendAck = false;

                    // Mark the frame as sent and keep a reference on it in the outputBuffer to show that we are awaiting an Ack for this frame.
                    OutputBuffer.Add(nextFrame.Id % BufferSize, nextFrame);

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
                    Frame ackFrame = new Frame(Constants.FrameType.Ack, DecrementSequenceNumber(NextFrameToReceive));

                    // TODO Check to make sure we do not need to Stop the timer.
                    // Early reflexion : should not do it -> if we are here it's because the timer ticked and flipped the SendAck to true, so it stopped itself. Logically, we shouldn't start it again if the SendAck is true, so we know it's off.
                    // AckTimer.Stop();

                    // Inform the program that we did send the Ack
                    sendAck = false;

                    SendFrame(ackFrame);
                }
                else if (transmitterDataReceived) // data received on wire (correct or corrupt)
                {
                    Frame frameReceived = GetReceivedFrame();

                    if (frameReceived == null)
                    {
                        // The frame was corrupted, we prepare a NAK, but only if we have not sent another one already
                        if (NoNakSentForNextAwaitedFrame)
                        {
                            // Set the LastFrameSequenceNumberForNak to the currently awaited frame's sequence number
                            LastFrameSequenceNumberForNak = NextFrameToReceive;

                            // Build a Nak frame
                            Frame nakFrame = new Frame(Constants.FrameType.Nak, DecrementSequenceNumber(NextFrameToReceive));

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
                            if (frameReceived.Id != NextFrameToReceive && NoNakSentForNextAwaitedFrame)
                            {
                                // Set the LastFrameSequenceNumberForNak to the currently awaited frame's sequence number
                                LastFrameSequenceNumberForNak = NextFrameToReceive;

                                // Build a NAK frame
                                Frame nakFrame = new Frame(Constants.FrameType.Nak, DecrementSequenceNumber(NextFrameToReceive));

                                // Put the Nak frame in the high priority queue to send it as soon as possible
                                HighPriorityFrames.Enqueue(nakFrame);
                            }

                            // Check if the frame id fits in the input buffer. If it does not, we ignore its data
                            if (IsBetween(NextFrameToReceive, frameReceived.Id, LastFrameToReceive))
                            {
                                // we can add it to the input buffer if not already there
                                if (!InputBuffer.ContainsKey(frameReceived.Id % BufferSize))
                                {
                                    InputBuffer.Add(frameReceived.Id % BufferSize, frameReceived);
                                }
                            }

                            // Try to pass data to the superior layer (in the fileStream) if we have the next ordered frames
                            while (InputBuffer.ContainsKey(NextFrameToReceive % BufferSize))
                            {
                                // TODO Add validation for file write

                                // Write to frame data to the file
                                byte[] frameData = new byte[frameReceived.Data.Length / 8];
                                frameReceived.Data.CopyTo(frameData, 0);
                                outputFileStream.Write(frameData, 0, (int)frameReceived.DataSize);
                                outputFileStream.Flush();

                                // Remove the frame from the input buffer
                                InputBuffer.Remove(NextFrameToReceive % BufferSize);

                                // Increment the awaited frame sequence number because this one has been treated. This also reset the LastFrameSequenceNumberForNak value so that it is not mistaken for another Frame with the same sequence number later on.
                                NextFrameToReceive = IncrementSequenceNumber(NextFrameToReceive);
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
                            
                            if (IsBetween(FirstFrameSent, nakSequenceNumber, NextFrameToSendSequenceNumber)) // valid sequence number for current window
                            {
                                if (OutputBuffer.ContainsKey(nakSequenceNumber % BufferSize))
                                {
                                    // If Nak refers to a frame in the outputBuffer, this mean it is indeed a frame that we sent earlier. We need to send it again very soon
                                    HighPriorityFrames.Enqueue(OutputBuffer[nakSequenceNumber % BufferSize]);
                                }
                            }
                        }

                        // Update the NextAwaitedAckSequenceNumber value with the Ack in the frame. 
                        while (IsBetween(FirstFrameSent, frameReceived.Ack, NextFrameToSendSequenceNumber))
                        {
                            System.Timers.Timer timeoutTimer;
                            // Remove the timeout timer associated with this frame sequence number
                            if (TimeoutTimers.TryRemove(FirstFrameSent, out timeoutTimer))
                            {
                                timeoutTimer.Stop();
                            }

                            OutputBuffer.Remove(FirstFrameSent % BufferSize);

                            FirstFrameSent = IncrementSequenceNumber(FirstFrameSent);
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
                        Frame frameToResend = OutputBuffer[finishedTimeoutTimer.Key % BufferSize];

                        // Send as soon as possible
                        HighPriorityFrames.Enqueue(frameToResend);

                        // Remove the timeout from the dictionnary
                        System.Timers.Timer temp = new System.Timers.Timer();
                        TimeoutTimers.TryRemove(finishedTimeoutTimer.Key, out temp);
                    }
                }
            }
        }

        /// <summary>
        /// Register a timeout for the specified sequence number. The timer will signal that we never had an Ack for the sequence number provided.
        /// </summary>
        /// <param name="sequenceNumber"></param>
        private void RegisterTimeout(UInt16 sequenceNumber)
        {
            // The convention is that a stopped timer means the timeout occured and we should resend the frame.
            System.Timers.Timer timeoutTimer = new System.Timers.Timer(TimeoutInMs);

            timeoutTimer.Elapsed += (sender, e) => { timeoutTimer.Stop(); Console.WriteLine("timeout : {0}", sequenceNumber); };
            timeoutTimer.Start();

            TimeoutTimers.TryAdd(sequenceNumber, timeoutTimer);
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Check if middle is between beginning and end in a circular range
        /// </summary>
        /// <param name="beginning"></param>
        /// <param name="middle"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private bool IsBetween(UInt16 beginning, UInt16 middle, UInt16 end)
        {
            return (beginning <= middle && middle < end) || (end < beginning && beginning <= middle) || (middle < end && end < beginning);
        }

        /// <summary>
        /// Increment a sequence number and ensure that we never go past MaxSequence - 1, we instead go back to 0.
        /// </summary>
        /// <param name="sequenceNumber"></param>
        /// <returns></returns>
        private UInt16 IncrementSequenceNumber(UInt16 sequenceNumber)
        {
            return (UInt16)(sequenceNumber + 1 < MaxSequence ? sequenceNumber + 1 : 0);
        }

        /// <summary>
        /// Decrement a sequence number and ensure that we never fall below 0, we instead go back to MaxSequence - 1
        /// </summary>
        /// <param name="sequenceNumber"></param>
        /// <returns></returns>
        private UInt16 DecrementSequenceNumber(UInt16 sequenceNumber)
        {
            return (UInt16)(sequenceNumber - 1 >= 0 ? sequenceNumber - 1 : MaxSequence - 1);
        }

        #endregion

        #region Frame management methods

        /// <summary>
        /// Builds a frame with the next data to send
        /// </summary>
        /// <param name="numSequence"></param>
        /// <param name="ack"></param>
        /// <returns></returns>
        private Frame BuildDataFrame(UInt16 numSequence, UInt16 ack)
        {
            // TODO Add validation for file read

            // Fill data with next file chunk
            byte[] data = new byte[DataSizeInFrame];
            int actuallyReadBytesAmount = inputFileStream.Read(data, 0, DataSizeInFrame);
            BitArray dataBitArray = new BitArray(data);

            return new Frame(numSequence, Constants.FrameType.Data, ack, dataBitArray, (UInt32)actuallyReadBytesAmount);
        }

        /// <summary>
        /// Encode the frame and send it over to the transmitter.
        /// </summary>
        /// <param name="frame"></param>
        private void SendFrame(Frame frame)
        {
            // Prepare the frame to be sent on the wire (converts to BitArray and encode for error control with Hamming)
            BitArray frameBitArray = frame.GetFrameAsByteArray();
            Tuple<BitArray, HammingHelper.ReturnType> tuple = HammingHelper.EncryptManager(frameBitArray, HammingHelper.Mode.CORRECT, EncodedFramePadding);
            BitArray encodedFrameBitArray = tuple.Item1;

            // Notify subscriber that frame is being sent
            sendFrameDelegate(frame, true);

            Console.WriteLine("{5, 11} {0, 12} : id={1, 2}, type={2, 4}, ack={3, 2}, data lenght={4, 3}={6, 3}", "SendFrame", frame.Id, frame.Type.ToString(), frame.Ack, frame.Data.Count / 8, StationIdenfication == Constants.Station.Station1 ? "Station 1" : "Station 2", frame.DataSize);

            // Send the data
            transmitter.SendData(encodedFrameBitArray, StationIdenfication);
        }

        /// <summary>
        /// Get the frame from the transmitter.
        /// </summary>
        /// <returns>The decoded Frame. Null if Frame was corrupted or there was no data in the transmitter.</returns>
        private Frame GetReceivedFrame()
        {
            if (transmitter.DataReceived(StationIdenfication))
            {
                // there is indeed a data, we are going to get it
                BitArray encodedFrameBitArray = transmitter.GetData(StationIdenfication);

                // ****************************************************
                // TODO Check if data is corrupted (NEED JONATHAN TO ADD A SERVICE TO ITS HAMMING HELPER)
                // ****************************************************
                bool isCorrupted = false;
                if (isCorrupted)
                {
                    return null;
                }

                // Decode the frame
                Tuple<BitArray, HammingHelper.ReturnType> tuple = HammingHelper.DecryptManager(encodedFrameBitArray, HammingHelper.Mode.CORRECT, EncodedFramePadding);
                BitArray frameBitArray = tuple.Item1;

                // Converts BitArray to Frame
                Frame frame = Frame.GetFrameFromBitArray(frameBitArray);

                // Notify subscriber that frame is being received
                sendFrameDelegate(frame, false);

                Console.WriteLine("{5, 11} {0, 12} : id={1, 2}, type={2, 4}, ack={3, 2}, data lenght={4, 3}={6, 3}", "ReceiveFrame", frame.Id, frame.Type.ToString(), frame.Ack, frame.Data.Count / 8, StationIdenfication == Constants.Station.Station1 ? "Station 1" : "Station 2", frame.DataSize);

                return frame;
            }
            
            return null;
        }

        #endregion
    }
}
