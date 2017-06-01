using System;

namespace TransmissionSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            ProgramOption progOption = new ProgramOption();
            bool isValid = CommandLine.Parser.Default.ParseArgumentsStrict(args, progOption);

            if (isValid)
            {
                //Start the threads

            }
            else
            {
                Console.WriteLine("Invalid input. Please refer to the help. (TransmissionSimulation.exe -help");
            }
            Console.ReadLine();
        }
    }
}
