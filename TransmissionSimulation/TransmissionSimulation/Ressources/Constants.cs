﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransmissionSimulation.Ressources
{
    public static class Constants
    {
        public enum Station { Source, Dest }

        /// <summary>
        /// Frame size in bytes
        /// </summary>
        public const int FrameSize = 128;

        /// <summary>
        /// The default delay of transfer in seconds
        /// </summary>
        public const int DefaultDelay = 5;
    }
}
