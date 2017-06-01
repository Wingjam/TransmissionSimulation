using System.Collections;
﻿using TransmissionSimulation.Ressources;
                            
namespace TransmissionSimulation.Components
{
    public interface ITransmitter
    {
        bool TransmitterReady(Constants.Station station);
        void SendData(BitArray data, Constants.Station station);
        bool DataReceived(Constants.Station station);
        BitArray GetData(Constants.Station station);
    }
}
