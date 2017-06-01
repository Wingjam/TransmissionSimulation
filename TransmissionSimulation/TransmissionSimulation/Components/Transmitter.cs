﻿﻿﻿﻿﻿using System;
using System.Collections;
using System.Threading;
using TransmissionSimulation.Ressources;

namespace TransmissionSimulation.Components
{
    class Transmitter
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

        public Transmitter()
        {
            sendingSource = new BitArray(Constants.FrameSize*8);
            sendingDest = new BitArray(Constants.FrameSize*8);
			receivingSource = new BitArray(Constants.FrameSize * 8);
			receivingDest = new BitArray(Constants.FrameSize * 8);
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
			return (station == Constants.Station.Source) ? dataReceivedSource : dataReceivedDest;
		}

        public void SendData(BitArray data, Constants.Station station)
        {
            if (station == Constants.Station.Source)
            {
                sendingSource = data;
                readyToSendSource = false;
            }
        }

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
