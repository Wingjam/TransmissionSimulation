using CommandLine;

namespace TransmissionSimulation
{
    public class ProgramOption
    {
        [Option('s', "size", DefaultValue = 10, HelpText = "Buffer size.")]
        public int BufferSize { get; set; }

        [Option('t', "timeout", DefaultValue = 5, HelpText = "Timeout in seconds of the connexion.")]
        public int Timeout { get; set; }

        [Option('e', "Detect", DefaultValue = true, HelpText = "If true, only detect the errors. If false, correct the errors.")]
        public bool DetectionOnly { get; set; }

        [Option('c', "copy", Required = true, HelpText = "Input file to be copied.")]
        public string FileToCopie { get; set; }

        [Option('d', "destination", Required = true, HelpText = "Destination file of the copy.")]
        public string DestinationFile { get; set; }
    }
}