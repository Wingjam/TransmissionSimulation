using TransmissionSimulation.Ressources;
                            
namespace TransmissionSimulation.Components
{
    public interface ITransmitter
    {
        bool TransmitterReady(Constants.Station station);
        void SendData(byte[] data, Constants.Station station);
        bool DataReceived(Constants.Station station);
        System.Collections.BitArray GetData(Constants.Station station);
    }
}
