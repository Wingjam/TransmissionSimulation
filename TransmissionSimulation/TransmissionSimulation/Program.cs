using System;
using System.Collections;
using System.Windows.Forms;

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
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm(progOption));
            }
            else
            {
                Console.WriteLine("Invalid input. Please refer to the help. (TransmissionSimulation.exe -help");
            }
        }

        static void Afficher(BitArray dataToPrint)
        {
            for (int i = 0; i < dataToPrint.Length; ++i)
            {
                if (i % 4 == 0)
                {
                    Console.Write(" ");
                }
                Console.Write(dataToPrint[i] ? 1 : 0);
            }
            Console.WriteLine();
        }
    }
}
