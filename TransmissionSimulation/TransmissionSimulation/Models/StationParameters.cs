using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransmissionSimulation.Models
{
    public class StationParameters
    {
        public int BufferSize { get; set; }

        /// <summary>
        /// Timeout in milliseconds
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// True means we detect errors, false means we correct errors
        /// </summary>
        public bool DetectionOnly { get; set; }

        public string SourceFilePath { get; set; }

        public string DestinationFilePath { get; set; }
    }
}
