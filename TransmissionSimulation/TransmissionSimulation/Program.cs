using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using TransmissionSimulation.Models;

namespace TransmissionSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            // Station 1 parameters
            StationParameters station1Parameters = new StationParameters();

            // Station 2 parameters
            StationParameters station2Parameters = new StationParameters();

            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                // Station 1 parameters read
                station1Parameters.BufferSize = Convert.ToInt32(appSettings.Get("BufferSize1") ?? "10");
                station1Parameters.Timeout = Convert.ToInt32(appSettings.Get("Timeout1") ?? "5000");
                station1Parameters.DetectionOnly = Convert.ToInt32(appSettings.Get("DetectionOnly1") ?? "0") == 1;
                station1Parameters.SourceFilePath = appSettings.Get("SourceFile1") ?? "test1.sent.txt";
                station1Parameters.DestinationFilePath = appSettings.Get("DestinationFile1") ?? "test1.received.txt";

                // Station 2 parameters read
                station2Parameters.BufferSize = Convert.ToInt32(appSettings.Get("BufferSize2") ?? "10");
                station2Parameters.Timeout = Convert.ToInt32(appSettings.Get("Timeout2") ?? "5000");
                station2Parameters.DetectionOnly = Convert.ToInt32(appSettings.Get("DetectionOnly2") ?? "0") == 1;
                station2Parameters.SourceFilePath = appSettings.Get("SourceFile2") ?? "test2.sent.txt";
                station2Parameters.DestinationFilePath = appSettings.Get("DestinationFile2") ?? "test2.received.txt";

                Console.WriteLine("AppSettings correctly read.");
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings.");
            }

            // Only valid if both source files exists and destination files are different from one another (cannot write to same file from 2 different stations)
            bool isValid = File.Exists(station1Parameters.SourceFilePath) && File.Exists(station2Parameters.SourceFilePath)
                && new FileInfo(station1Parameters.DestinationFilePath)?.FullName != new FileInfo(station2Parameters.DestinationFilePath)?.FullName;

            if (isValid)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm(station1Parameters, station2Parameters));
            }
            else
            {
                Console.WriteLine("The path to the file to copy is not valid.");
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
