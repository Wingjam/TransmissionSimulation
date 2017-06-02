using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using TransmissionSimulation.Ressources;

namespace TransmissionSimulation.Components
{
    class Transmitter : ITransmitter
    {
        private static Mutex mutex = new Mutex();
        private BitArray sendingSource;
        private BitArray sendingDest;
        private BitArray receivingDest;
        private BitArray receivingSource;
        private bool readyToSendSource;
        private bool dataReceivedDest;
        private bool readyToSendDest;
        private bool dataReceivedSource;
        private uint bitInversions;

        public uint BitInversions { set => bitInversions = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TransmissionSimulation.Components.Transmitter"/> class.
        /// </summary>
        public Transmitter()
        {
            sendingSource = null;
            sendingDest = null;
            receivingSource = null;
            receivingDest = null;

            Task.Factory.StartNew(() => CheckForTransferConditions(), TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Checks for transfer conditions.
        /// Loops in a separate thread.
        /// </summary>
        private void CheckForTransferConditions()
        {
            while (true)
            {
                mutex.WaitOne();

                //Check for boolean conditions
                if (!readyToSendSource && !dataReceivedDest)
                {
                    BitArray transferData = sendingSource;
                    sendingSource = null;
                    Thread.Sleep(Constants.DefaultDelay * 10); //deciseconds to milliseconds
                    transferData = InjectError(transferData);
                    receivingDest = transferData;
                    readyToSendSource = true;
                    dataReceivedDest = true;
                }
                else if (!readyToSendDest && !dataReceivedSource)
                {
                    BitArray transferData = sendingDest;
                    sendingDest = null;
                    Thread.Sleep(Constants.DefaultDelay*10);
                    transferData = InjectError(transferData);
                    receivingSource = transferData;
                    readyToSendDest = true;
                    dataReceivedSource = true;
                }

                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Injects errors randomly into the data to transfer based on the BitInversions property.
        /// </summary>
        /// <returns>Modified, partially erronous data.</returns>
        /// <param name="transferData">Data to insert errors into.</param>
        private BitArray InjectError(BitArray transferData)
        {
            for (int i = 0; i < bitInversions; ++i)
            {
                int pos = new Random().Next(0, transferData.Length);
                transferData[pos] = !transferData[pos];
            }

            bitInversions = 0;

            return transferData;
        }

        /// <summary>
        /// Checks if the transmitter is ready to send data for the station.
        /// </summary>
        /// <returns><c>true</c>, if transmitter ready is ready for the station, <c>false</c> otherwise.</returns>
        /// <param name="station">Station to check for transmitter ready.</param>
        public bool TransmitterReady(Constants.Station station)
        {
            return (station == Constants.Station.Source) ? readyToSendSource : readyToSendDest;
        }

        /// <summary>
        /// Checks if data was received for the station.
        /// </summary>
        /// <returns><c>true</c>, if received was dataed, <c>false</c> otherwise.</returns>
        /// <param name="station">Station that checks for data received.</param>
        public bool DataReceived(Constants.Station station)
        {
            return (station == Constants.Station.Source) ? dataReceivedSource : dataReceivedDest;
        }

        /// <summary>
        /// Sends the data from the station.
        /// </summary>
        /// <param name="data">Data to send.</param>
        /// <param name="station">Station tto send data from.</param>
        public void SendData(BitArray data, Constants.Station station)
        {
            if (station == Constants.Station.Source)
            {
                sendingSource = data;
                readyToSendSource = false;
            }
            else
            {
                sendingDest = data;
                readyToSendDest = false;
            }
        }

        /// <summary>
        /// Gets the data available for the station.
        /// </summary>
        /// <returns>The data received.</returns>
        /// <param name="station">Station that gets available data.</param>
        public BitArray GetData(Constants.Station station)
        {
            BitArray data = new BitArray(Constants.FrameSize * 8);

            if (DataReceived(station))
            {
                if (station == Constants.Station.Source)
                {
                    data = receivingSource;
                    dataReceivedSource = false;
                }
                else
                {
                    data = receivingDest;
                    dataReceivedDest = false;
                }
            }
            else
            {
                throw new InvalidOperationException("Data was not ready, use DataReceived() first.");
            }

            return data;
        }
    }
}
