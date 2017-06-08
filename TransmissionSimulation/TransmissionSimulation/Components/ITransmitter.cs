using System.Collections;
using TransmissionSimulation.Ressources;
                            
namespace TransmissionSimulation.Components
{
    public interface ITransmitter
    {
        bool TransmitterReady(Constants.StationId station);
        void SendData(BitArray data, Constants.StationId station);
        bool DataReceived(Constants.StationId station);
        BitArray GetData(Constants.StationId station);
        void InsertRandomErrors(int errorCount, int startIndex, int endIndex);
    }
}
