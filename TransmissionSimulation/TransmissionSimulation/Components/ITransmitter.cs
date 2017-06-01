using System;
using TransmissionSimulation.Ressources;
                            
namespace TransmissionSimulation.Components
{
    public interface ITransmitter
    {
        bool TransmitterReady(Constants.Station station);
    }
}
