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
        private ArrayList indicesInversions;

        public ArrayList IndicesInversions { set => indicesInversions = value; }
        public uint BitInversions { set => bitInversions = value; }
        public bool ReadyToSendSource
        {
            get => readyToSendSource;
            set
            {
                readyToSendSource = value;
                CheckForTransferConditions();
            }
        }
        public bool DataReceivedDest
        {
            get => dataReceivedDest;
            set
            {
                dataReceivedDest = value;
                CheckForTransferConditions();
            }
        }
        public bool ReadyToSendDest
        {
            get => readyToSendDest;
            set
            {
                readyToSendDest = value;
                CheckForTransferConditions();
            }
        }
        public bool DataReceivedSource
        {
            get => dataReceivedSource;
            set
            {
                dataReceivedSource = value;
                CheckForTransferConditions();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TransmissionSimulation.Components.Transmitter"/> class.
        /// </summary>
        public Transmitter()
        {
            sendingSource = null;
            sendingDest = null;
            receivingSource = null;
            receivingDest = null;
        }

        /// <summary>
        /// Checks for transfer conditions.
        /// Loops in a separate thread.
        /// </summary>
        private void CheckForTransferConditions()
        {
            mutex.WaitOne();

            //Source to Destination
            if (!readyToSendSource && !dataReceivedDest)
            {
                BitArray transferData = sendingSource;
                sendingSource = null;
                Thread.Sleep(Constants.DefaultDelay * 100); //deciseconds to milliseconds
                transferData = InjectErrors(transferData);
                receivingDest = transferData;
                readyToSendSource = true;
                dataReceivedDest = true;
            }
            //Destination to Source
            else if (!readyToSendDest && !dataReceivedSource)
            {
                BitArray transferData = sendingDest;
                sendingDest = null;
                Thread.Sleep(Constants.DefaultDelay * 100);
                transferData = InjectErrors(transferData);
                receivingSource = transferData;
                readyToSendDest = true;
                dataReceivedSource = true;
            }

            mutex.ReleaseMutex();
        }

		/// <summary>
		/// Injects errors into the data to transfer based on the desired positions from IndicesInversions.
		/// </summary>
		/// <returns>Modified, partially erronous data.</returns>
		/// <param name="transferData">Data to insert errors into.</param>
		private BitArray InjectErrors(BitArray transferData)
		{
            if (indicesInversions != null)
            {
                foreach(var i in indicesInversions)
                {
                    transferData[(int)i] = !transferData[(int)i];
                }
            }

			indicesInversions = null;

			return transferData;
		}

        /// <summary>
        /// Injects errors randomly into the data to transfer based on the BitInversions property.
        /// </summary>
        /// <returns>Modified, partially erronous data.</returns>
        /// <param name="transferData">Data to insert errors into.</param>
        private BitArray InjectErrorsRandomly(BitArray transferData)
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
            BitArray data = null;

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
