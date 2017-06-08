using TransmissionSimulation.Helpers;
using TransmissionSimulation.Models;

namespace TransmissionSimulation.Ressources
{
    public static class Constants
    {
        public enum Station { Station1, Station2 }

        public enum FrameType { Data, Ack, Nak }

        /// <summary>
        /// Frame size in bytes
        /// </summary>
        public const int FrameSize = 128;

        /// <summary>
        /// Frame padding in bits
        /// </summary>
        public static int FramePadding = FrameSize - (int)(HammingHelper.GetDataSize(FrameSize * 8) / 8);

        /// <summary>
        /// The default delay of transfer in deciseconds (1/10 second)
        /// </summary>
        public const int DefaultDelay = 5;

        /// <summary>
        /// The maximum of error that can be inject into a frame.
        /// </summary>
        public const int MaximumOfErrorToInject = 2;


        public delegate void ShowFrameDelegate(Frame frameToShow, bool isSent);
    }
}
