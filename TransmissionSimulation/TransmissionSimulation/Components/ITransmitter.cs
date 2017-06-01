using System;
using TransmissionSimulation.Ressources;
                            
namespace TransmissionSimulation.Components
{
    public interface ITransmitter
    {
        bool TransmitterReady(Constants.Station station);
        void SendData(byte[] data, Constants.Station station);
        bool DataReceived(Constants.Station station);
        byte[] GetData(Constants.Station station);
    }
}
