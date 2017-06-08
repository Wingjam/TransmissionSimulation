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
        private BitArray sendingStation1;
        private BitArray sendingStation2;
        private BitArray receivingStation2;
        private BitArray receivingStation1;
        private bool readyToSendStation1;
        private bool dataReceivedStation2;
        private bool readyToSendStation2;
        private bool dataReceivedStation1;
        private uint bitInversions;

        public List<int> IndicesInversions { get; set; }
        
        public uint BitInversions { set => bitInversions = value; }
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
            bitInversions = 0;
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
                    if (i < IndicesInversions.Count)
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
            return (station == Constants.Station.Station1) ? ReadyToSendStation1 && !DataReceivedStation2 : ReadyToSendStation2 && !DataReceivedStation1;
        }

        /// <summary>
        /// Checks if data was received for the station.
        /// </summary>
        /// <returns><c>true</c>, if received was dataed, <c>false</c> otherwise.</returns>
        /// <param name="station">Station that checks for data received.</param>
        public bool DataReceived(Constants.Station station)
        {
            return (station == Constants.Station.Station1) ? DataReceivedStation1 : DataReceivedStation2;
        }

        /// <summary>
        /// Sends the data from the station.
        /// </summary>
        /// <param name="data">Data to send.</param>
        /// <param name="station">Station to send data from.</param>
        public void SendData(BitArray data, Constants.Station station)
        {
            mutex.WaitOne();


            if (station == Constants.Station.Station1 && !DataReceivedStation2)
            {
                sendingStation1 = data;
                ReadyToSendStation1 = false;
            }
            else if (station == Constants.Station.Station2 && !DataReceivedStation1)
            {
                sendingStation2 = data;
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
        public BitArray GetData(Constants.Station station)
        {
            mutex.WaitOne();

            BitArray data = null;

            if (DataReceived(station))
            {
                if (station == Constants.Station.Station1)
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
