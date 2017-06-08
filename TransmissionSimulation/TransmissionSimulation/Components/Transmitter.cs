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
        private BitArray sendingSource;
        private BitArray sendingDest;
        private BitArray receivingDest;
        private BitArray receivingSource;
        private bool readyToSendSource;
        private bool dataReceivedDest;
        private bool readyToSendDest;
        private bool dataReceivedSource;
        private uint bitInversions;

        public List<int> IndicesInversions { get; set; }
        
        public uint BitInversions { set => bitInversions = value; }
        public bool ReadyToSendSource
        {
            get => readyToSendSource;
            set
            {
                readyToSendSource = value;
                Task.Run(() => TransferFrame());
                //TransferFrame();
            }
        }
        public bool DataReceivedDest
        {
            get => dataReceivedDest;
            set
            {
                dataReceivedDest = value;
				Task.Run(() => TransferFrame());
				//TransferFrame();
            }
        }
        public bool ReadyToSendDest
        {
            get => readyToSendDest;
            set
            {
                readyToSendDest = value;
				Task.Run(() => TransferFrame());
				//TransferFrame();
            }
        }
        public bool DataReceivedSource
        {
            get => dataReceivedSource;
            set
            {
                dataReceivedSource = value;
				Task.Run(() => TransferFrame());
				//TransferFrame();
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
            readyToSendSource = true;
            dataReceivedDest = false;
            readyToSendDest = true;
            dataReceivedSource = false;
            bitInversions = 0;
            IndicesInversions = new List<int>();
        }

        /// <summary>
        /// Transfers the frame if needed
        /// </summary>
        private void TransferFrame()
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
            return (station == Constants.Station.Source) ? ReadyToSendSource && !DataReceivedDest : ReadyToSendDest && !DataReceivedSource;
        }

        /// <summary>
        /// Checks if data was received for the station.
        /// </summary>
        /// <returns><c>true</c>, if received was dataed, <c>false</c> otherwise.</returns>
        /// <param name="station">Station that checks for data received.</param>
        public bool DataReceived(Constants.Station station)
        {
            return (station == Constants.Station.Source) ? DataReceivedSource : DataReceivedDest;
        }

        /// <summary>
        /// Sends the data from the station.
        /// </summary>
        /// <param name="data">Data to send.</param>
        /// <param name="station">Station to send data from.</param>
        public void SendData(BitArray data, Constants.Station station)
        {
            mutex.WaitOne();


            if (station == Constants.Station.Source && !DataReceivedDest)
            {
                sendingSource = data;
                ReadyToSendSource = false;
            }
            else if (station == Constants.Station.Dest && !DataReceivedSource)
            {
                sendingDest = data;
                ReadyToSendDest = false;
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
                if (station == Constants.Station.Source)
                {
                    data = receivingSource;
					DataReceivedSource = false;
                }
                else
                {
                    data = receivingDest;
					DataReceivedDest = false;
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
