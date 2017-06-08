﻿﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TransmissionSimulation.Ressources;

namespace TransmissionSimulation.Components
{
    public class Transmitter : ITransmitter
    {
        private static Mutex mutex = new Mutex();
        private static Random random;
        private BitArray sendingStation1;
        private BitArray sendingStation2;
        private BitArray receivingStation2;
        private BitArray receivingStation1;
        private bool readyToSendStation1;
        private bool dataReceivedStation2;
        private bool readyToSendStation2;
        private bool dataReceivedStation1;
        private Tuple<int, Tuple<int, int>> nextRandomError;

        public List<int> IndicesInversions { get; set; }
        public Tuple<int, Tuple<int, int>> NextRandomError { get => nextRandomError; }


        public bool ReadyToSendStation1
        {
            get => readyToSendStation1;
            set
            {
                readyToSendStation1 = value;
                Task.Run(() => TransferFrame());
                //TransferFrame();
            }
        }
        public bool DataReceivedStation2
        {
            get => dataReceivedStation2;
            set
            {
                dataReceivedStation2 = value;
				Task.Run(() => TransferFrame());
				//TransferFrame();
            }
        }
        public bool ReadyToSendStation2
        {
            get => readyToSendStation2;
            set
            {
                readyToSendStation2 = value;
				Task.Run(() => TransferFrame());
				//TransferFrame();
            }
        }
        public bool DataReceivedStation1
        {
            get => dataReceivedStation1;
            set
            {
                dataReceivedStation1 = value;
				Task.Run(() => TransferFrame());
				//TransferFrame();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TransmissionSimulation.Components.Transmitter"/> class.
        /// </summary>
        public Transmitter()
        {
            sendingStation1 = null;
            sendingStation2 = null;
            receivingStation1 = null;
            receivingStation2 = null;
            readyToSendStation1 = true;
            dataReceivedStation2 = false;
            readyToSendStation2 = true;
            dataReceivedStation1 = false;
            nextRandomError = null;
            random = new Random();
            IndicesInversions = new List<int>();
        }

        /// <summary>
        /// Transfers the frame if needed
        /// </summary>
        private void TransferFrame()
        {
            mutex.WaitOne();

            //Station1 to Station2
            if (!readyToSendStation1 && !dataReceivedStation2)
            {
                BitArray transferData = sendingStation1;
                sendingStation1 = null;
                Thread.Sleep(Constants.DefaultDelay * 100); //deciseconds to milliseconds
                if (nextRandomError != null)
                    transferData = InjectErrorsRandomly(transferData);
                else
                    transferData = InjectErrors(transferData);
                receivingStation2 = transferData;
                readyToSendStation1 = true;
                dataReceivedStation2 = true;
            }
            //Station2 to Station1
            else if (!readyToSendStation2 && !dataReceivedStation1)
            {
                BitArray transferData = sendingStation2;
                sendingStation2 = null;
                Thread.Sleep(Constants.DefaultDelay * 100);
                if (nextRandomError != null)
                    transferData = InjectErrorsRandomly(transferData);
                else
                    transferData = InjectErrors(transferData);
                receivingStation1 = transferData;
                readyToSendStation2 = true;
                dataReceivedStation1 = true;
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
            if (IndicesInversions != null && IndicesInversions.Count > 0)
            {
                foreach(var i in IndicesInversions)
                {
                    if (i < transferData.Count)
                        transferData[(int)i] = !transferData[(int)i];
                }
            }

			IndicesInversions.Clear();

			return transferData;
		}

        /// <summary>
        /// Injects errors randomly into the data to transfer based on the BitInversions property.
        /// </summary>
        /// <returns>Modified, partially erronous data.</returns>
        /// <param name="transferData">Data to insert errors into.</param>
        private BitArray InjectErrorsRandomly(BitArray transferData)
        {
            int dataLength = transferData.Length;
            int endIndex = (nextRandomError.Item2.Item2 > dataLength) ? dataLength : nextRandomError.Item2.Item2;
            int startIndex = nextRandomError.Item2.Item1;
            List<int> listIndex = new List<int>();

            for (int i = 0; i < nextRandomError.Item1; ++i)
            {
                bool newRandomIndexFound = false;
                while (!newRandomIndexFound && !(listIndex.Count >= endIndex-startIndex))
                {
                    int pos = random.Next(startIndex, endIndex);
                    if (!listIndex.Contains(pos))
                    {
                        listIndex.Add(pos);
                        newRandomIndexFound = true;
                    }
                }
            }

            foreach(int pos in listIndex)
            {
                transferData[dataLength - 1 - pos] = !transferData[dataLength - 1 - pos];
            }

            nextRandomError = null;

            return transferData;
        }

        /// <summary>
        /// Specify errors to be insert randomly on next data transfer
        /// </summary>
        /// <param name="errorCount">Number of errors</param>
        /// <param name="startIndex">Position in data bitarray to start error insertion (included)</param>
        /// <param name="endIndex">Position in data bitarray to end error insertion (excluded)</param>
        public void InsertRandomErrors(int errorCount, int startIndex, int endIndex)
        {
            if (startIndex > endIndex)
                throw new FormatException("Start index must not be greater than end index.");

            if (errorCount >= 0 && startIndex >= 0 && endIndex >= 0)
                nextRandomError = new Tuple<int, Tuple<int, int>>(errorCount, new Tuple<int, int>(startIndex, endIndex));
            else
                throw new FormatException("Error count and indexes must be greater than 0.");
        }

        /// <summary>
        /// Checks if the transmitter is ready to send data for the station.
        /// </summary>
        /// <returns><c>true</c>, if transmitter ready is ready for the station, <c>false</c> otherwise.</returns>
        /// <param name="station">Station to check for transmitter ready.</param>
        public bool TransmitterReady(Constants.StationId station)
        {
            return (station == Constants.StationId.Station1) ? ReadyToSendStation1 && !DataReceivedStation2 : ReadyToSendStation2 && !DataReceivedStation1;
        }

        /// <summary>
        /// Checks if data was received for the station.
        /// </summary>
        /// <returns><c>true</c>, if data was received, <c>false</c> otherwise.</returns>
        /// <param name="station">Station that checks for data received.</param>
        public bool DataReceived(Constants.StationId station)
        {
            return (station == Constants.StationId.Station1) ? DataReceivedStation1 : DataReceivedStation2;
        }

        /// <summary>
        /// Sends the data from the station.
        /// </summary>
        /// <param name="data">Data to send.</param>
        /// <param name="station">Station to send data from.</param>
        public void SendData(BitArray data, Constants.StationId station)
        {
            mutex.WaitOne();

            if (station == Constants.StationId.Station1 && !DataReceivedStation2)
            {
                sendingStation1 = new BitArray(data);
                ReadyToSendStation1 = false;
            }
            else if (station == Constants.StationId.Station2 && !DataReceivedStation1)
            {
                sendingStation2 = new BitArray(data);
                ReadyToSendStation2 = false;
            }
            else
            {
                throw new InvalidOperationException("Station was not ready to send, use GetData() first.");
            }

            mutex.ReleaseMutex();
        }

        /// <summary>
        /// Gets the data available for the station.
        /// </summary>
        /// <returns>The data received.</returns>
        /// <param name="station">Station that gets available data.</param>
        public BitArray GetData(Constants.StationId station)
        {
            mutex.WaitOne();

            BitArray data = null;

            if (DataReceived(station))
            {
                if (station == Constants.StationId.Station1)
                {
                    data = receivingStation1;
					DataReceivedStation1 = false;
                }
                else
                {
                    data = receivingStation2;
					DataReceivedStation2 = false;
                }
            }
            else
            {
                throw new InvalidOperationException("Data was not ready, use DataReceived() first.");
            }

            mutex.ReleaseMutex();

            return data;
        }
    }
}
