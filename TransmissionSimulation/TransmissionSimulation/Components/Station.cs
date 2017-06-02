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
        Constants.Station type;
        ITransmitter transmitter;
        ConcurrentQueue<BitArray> buffer;
        int bufferSize;
        int timeoutInMs;
        FileStream fileStream;

        volatile bool allFrameBuffered = false;

        public Station(Constants.Station type, ITransmitter transmitter, int bufferSize, int timeoutInMs, FileStream fileStream)
        {
            this.type = type;
            this.transmitter = transmitter;
            this.bufferSize = bufferSize;
            buffer = new ConcurrentQueue<BitArray>();
            this.timeoutInMs = timeoutInMs;
            this.fileStream = fileStream;

            if (type == Constants.Station.Source)
            {
                Task fileReaderTask = new Task(() => { SendData(); });
                fileReaderTask.Start();

                Task transmitterBufferTask = new Task(() => { ContinuouslyEmptyQueue(); });
                transmitterBufferTask.Start();

                Task.WaitAll(fileReaderTask, transmitterBufferTask);
            }
            else if (type == Constants.Station.Dest)
            {
                ReceiveData();
            }
        }

        public void Start()
        {

        }

        private void ContinuouslyEmptyQueue()
        {
            while (allFrameBuffered)
            {
                while (!transmitter.TransmitterReady(type))
                {
                    Thread.Sleep(50);
                }

                // transmitter is now ready, send a frame

            }
        }

        private void SendData()
        {
            int dataSizeInFrame = 100;//HammingHelper.GetDataSize(Constants.FrameSize) / 8 - 32;
            byte[] fileReadBuffer = new byte[dataSizeInFrame];
            int numberOfFrameSent = 0;

            int numberBytesRead = fileStream.Read(fileReadBuffer, 0, dataSizeInFrame);
            while (numberBytesRead > 0)
            {
                Frame frame = new Frame(new BitArray(fileReadBuffer));

                while (IsBufferFull())
                {
                    Thread.Sleep(10);
                }

                byte[] frameInByteArray = ObjectToByteArray(frame);
                BitArray formattedFrameInByteArray = new BitArray(frameInByteArray);//HammingHelper.Encrypt(new BitArray(frameInByteArray));
                buffer.Enqueue(formattedFrameInByteArray);

                numberBytesRead = fileStream.Read(fileReadBuffer, Math.Max(numberOfFrameSent * Constants.FrameSize, (int)fileStream.Length - 1), Constants.FrameSize);
            }

            allFrameBuffered = true;
        }

        private bool IsBufferFull()
        {
            return bufferSize == buffer.Count;
        }

        private void ReceiveData()
        {

        }

        private void SendFrame()
        {

        }

        private void RetrieveFrame()
        {

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
