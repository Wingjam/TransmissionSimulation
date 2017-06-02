﻿﻿using System;
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
        /// The default delay of transfer in deciseconds (1/10 second)
        /// </summary>
        public const int DefaultDelay = 5;

        /// <summary>
        /// The maximum of error that can be inject into a frame.
        /// </summary>
        public const int MaximumOfErrorToInject = 2;
    }
}
