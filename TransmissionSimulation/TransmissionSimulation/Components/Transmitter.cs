﻿﻿﻿﻿using System;
using System.Threading;
using TransmissionSimulation.Ressources;

namespace TransmissionSimulation.Components
{
    class Transmitter
    {
        private static Mutex mutex = new Mutex();
        private byte[] sendingSource;
		private byte[] sendingDest;
        private byte[] receivingDest;
        private byte[] receivingSource;
        private bool readyToSendSource;
        private bool dataReceivedDest;
        private bool readyToSendDest;
        private bool dataReceivedSource;

        public Transmitter()
        {
            sendingSource = new byte[Constants.FrameSize];
            sendingDest = new byte[Constants.FrameSize];
			receivingSource = new byte[Constants.FrameSize];
			receivingDest = new byte[Constants.FrameSize];
        }

        public void InjectError()
        {
            
        }

        public bool TransmitterReady(Constants.Station station)
        {
            return (station == Constants.Station.Source) ? readyToSendSource : readyToSendDest;
        }

		public bool DataReceived(Constants.Station station)
		{
			return (station == Constants.Station.Source) ? dataReceivedDest : dataReceivedSource;
		}

        public void SendData(byte[] data, Constants.Station station)
        {
            if (station == Constants.Station.Source)
            {
                sendingSource = data;
                readyToSendSource = false;
            }
        }

        public byte[] GetData(Constants.Station station)
        {
            byte[] data = new byte[Constants.FrameSize];

            if (DataReceived(station))
            {
                data = (station == Constants.Station.Source) ? receivingSource : receivingDest;
            }
            else
            {
                throw new InvalidOperationException("Data was not ready, use DataReceived() first.");
            }

            return data;
        }
	}
}
