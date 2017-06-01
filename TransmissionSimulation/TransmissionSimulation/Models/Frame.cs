using System.Collections;

namespace TransmissionSimulation.Models
{
    class Frame
    {
        private static ulong lastId = 0;

        private ulong id;
        private BitArray data;
        private bool isAck;
        private bool isNak;

        /// <summary>
        /// Constructor with the id auto generated. Used for transmitting information
        /// </summary>
        /// <param name="data"></param>
        public Frame(BitArray data) : this(LastId(), data, false, false)
        {
        }

        /// <summary>
        /// Constructor for the response, giving the id of the frame.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="isAck"></param>
        public Frame(ulong id, BitArray data, bool isAck) : this(id, data, isAck, !isAck)
        {
        }

        /// <summary>
        /// Constructor containg all the information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="isAck"></param>
        /// <param name="isNak"></param>
        private Frame(ulong id, BitArray data, bool isAck, bool isNak)
        {
            this.id = id;
            this.data = data;
            this.isAck = isAck;
            this.isNak = isNak;
        }

        public ulong Id => id;

        public BitArray Data => data;

        public bool IsAck => isAck;

        public bool IsNak => isNak;

        private static ulong LastId()
        {
            return lastId++;
        }
    }
}
