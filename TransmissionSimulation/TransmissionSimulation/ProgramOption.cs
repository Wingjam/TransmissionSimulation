using CommandLine;

namespace TransmissionSimulation
{
    public class ProgramOption
    {
        [Option('s', "size", DefaultValue = 10, HelpText = "Frame size.")]
        public int FrameSize { get; set; }

        [Option('t', "timeout", DefaultValue = 5, HelpText = "Timeout in seconds of the connexion.")]
        public int Timeout { get; set; }

        [Option('c', "copy", Required = true, HelpText = "Input file to be copied.")]
        public string FileToCopie { get; set; }

        [Option('d', "destination", Required = true, HelpText = "Destination file of the copy.")]
        public string DestinationFile { get; set; }
    }
}