using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using static TransmissionSimulation.Ressources.Constants;

namespace TransmissionSimulation.Models
{
    [Serializable]
    public class Frame
    {
        static public int HeaderSize()
        {
            // (id + ack) + (type + datasize)
            return sizeof(UInt16) * 2 + sizeof(UInt32) * 2;
        }

        /// <summary>
        /// Default constructor creating a Data Frame with an id set to 0
        /// </summary>
        public Frame() : this(0, FrameType.Data, 0, new BitArray(0), 0)
        { }

        /// <summary>
        /// Constructor with FrameType only
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="ack"></param>
        public Frame(UInt16 id, FrameType type, UInt16 ack) : this(id, type, ack, new BitArray(0), 0)
        {
        }

        /// <summary>
        /// Constructor for Ack and Nak frames
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ack"></param>
        public Frame(FrameType type, UInt16 ack) : this(0, type, ack, new BitArray(0), 0)
        {
        }

        /// <summary>
        /// Constructor containg all the information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="ack"></param>
        /// <param name="data"></param>
        /// <param name="dataSize">Data size in bytes</param>
        public Frame(UInt16 id, FrameType type, UInt16 ack, BitArray data, UInt32 dataSize)
        {
            Id = id;
            Type = type;
            Data = data;
            Ack = ack;
            DataSize = dataSize;
        }

        public UInt16 Id { get; set; }

        public FrameType Type { get; set; }

        public UInt16 Ack { get; set; }

        public UInt32 DataSize { get; set; }

        public BitArray Data { get; set; }

        public BitArray GetFrameAsByteArray()
        {
            byte[] frame = new byte[Frame.HeaderSize() + Data.Count / 8];

            BitConverter.GetBytes((UInt16)Id).CopyTo(frame, 0);
            BitConverter.GetBytes((UInt32)Type).CopyTo(frame, sizeof(UInt16));
            BitConverter.GetBytes((UInt16)Ack).CopyTo(frame, sizeof(UInt16) + sizeof(UInt32));
            BitConverter.GetBytes((UInt32)DataSize).CopyTo(frame, sizeof(UInt16) * 2 + sizeof(UInt32));

            Data.CopyTo(frame, HeaderSize());

            return new BitArray(frame);
        }

        public static Frame GetFrameFromBitArray(BitArray bitArray)
        {
            Frame frame = new Frame();
            byte[] frameByteArray = new byte[bitArray.Length];
            bitArray.CopyTo(frameByteArray, 0);

            frame.Id = (UInt16)BitConverter.ToInt16(frameByteArray, 0);
            frame.Type = (FrameType)BitConverter.ToInt32(frameByteArray, sizeof(UInt16));
            frame.Ack = (UInt16)BitConverter.ToInt16(frameByteArray, sizeof(UInt16) + sizeof(UInt32));
            frame.DataSize = (UInt32)BitConverter.ToInt32(frameByteArray, sizeof(UInt16) * 2 + sizeof(UInt32));

            int dataSize = bitArray.Count / 8 - HeaderSize();
            byte[] data = new byte[dataSize];
            Array.Copy(frameByteArray, HeaderSize(), data, 0, dataSize);
            frame.Data = new BitArray(data);

            return frame;
        }
    }
}
