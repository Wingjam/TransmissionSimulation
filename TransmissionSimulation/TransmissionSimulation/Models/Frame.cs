using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using static TransmissionSimulation.Ressources.Constants;

namespace TransmissionSimulation.Models
{
    [Serializable]
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
        public Frame() : this(0, FrameType.Data, 0, new BitArray(0))
        { }

        /// <summary>
        /// Constructor with FrameType only
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="ack"></param>
        public Frame(UInt16 id, FrameType type, UInt16 ack) : this(id, type, ack, new BitArray(0))
        {
        }

        /// <summary>
        /// Constructor for Ack and Nak frames
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ack"></param>
        public Frame(FrameType type, UInt16 ack) : this(0, type, ack, new BitArray(0))
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

        public BitArray GetFrameAsByteArray()
        {
            byte[] frame = new byte[Frame.HeaderSize() + Data.Count / 8];

            BitConverter.GetBytes((UInt16)Id).CopyTo(frame, 0);
            BitConverter.GetBytes((Int32)Type).CopyTo(frame, sizeof(UInt16));
            BitConverter.GetBytes((UInt16)Ack).CopyTo(frame, sizeof(UInt16) + sizeof(Int32));
                
            Data.CopyTo(frame, Frame.HeaderSize());

            return new BitArray(frame);
        }

        public static Frame GetFrameFromBitArray(BitArray bitArray)
        {
            Frame frame = new Frame();
            byte[] frameByteArray = new byte[bitArray.Length];
            bitArray.CopyTo(frameByteArray, 0);
            
            frame.Id = (UInt16)BitConverter.ToInt16(frameByteArray, 0);
            frame.Type = (FrameType)BitConverter.ToInt32(frameByteArray, sizeof(UInt16));
            frame.Ack = (UInt16)BitConverter.ToInt16(frameByteArray, sizeof(UInt16) + sizeof(Int32));

            int dataSize = bitArray.Count / 8 - HeaderSize();
            byte[] data = new byte[dataSize];
            Array.Copy(frameByteArray, HeaderSize(), data, 0, dataSize);
            frame.Data = new BitArray(data);

            return frame;
        }
    }
}
