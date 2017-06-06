using System;
using System.Collections;
using static TransmissionSimulation.Ressources.Constants;

namespace TransmissionSimulation.Models
{
    class Frame
    {
        static public int HeaderSize()
        {
            // id + ack + type
            return sizeof(UInt16) * 2 + sizeof(FrameType);
        }

        /// <summary>
        /// Default constructor creating a Data Frame with an id set to 0
        /// </summary>
        public Frame() : this(0, FrameType.Data, 0, null)
        { }

        /// <summary>
        /// Constructor with FrameType only
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="ack"></param>
        public Frame(UInt16 id, FrameType type, UInt16 ack) : this(id, type, ack, null)
        {
        }

        /// <summary>
        /// Constructor for Ack and Nak frames
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ack"></param>
        public Frame(FrameType type, UInt16 ack) : this(0, type, ack, null)
        {
        }

        /// <summary>
        /// Constructor containg all the information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="ack"></param>
        /// <param name="data"></param>
        public Frame(UInt16 id, FrameType type, UInt16 ack, BitArray data)
        {
            Id = id;
            Type = type;
            Data = data;
            Ack = ack;
        }

        public UInt16 Id { get; set; }

        public FrameType Type { get; set; }

        public BitArray Data { get; set; }

        public UInt16 Ack { get; set; }
    }
}
