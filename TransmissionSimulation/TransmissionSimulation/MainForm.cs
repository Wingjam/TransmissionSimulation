using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TransmissionSimulation.Components;
using TransmissionSimulation.Models;
using TransmissionSimulation.Ressources;

namespace TransmissionSimulation
{
    public partial class MainForm : Form
    {
        private bool envoie;
        private bool envoie2;
        private StationParameters station1Parameters;
        private StationParameters station2Parameters;
        private Station station1;
        private Station station2;
        private Thread sendThread;
        private Thread receiveThread;
        private Transmitter cable;
        private NumericUpDown[] errorsPositions;
        delegate void UpdateDataDelegate(Frame frameToShow, Constants.FrameEvent frameEvent, Constants.StationId stationId);

        public MainForm(StationParameters station1Parameters, StationParameters station2Parameters)
        {
            this.station1Parameters = station1Parameters;
            this.station2Parameters = station2Parameters;
            envoie = true;
            envoie2 = true;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            errorsPositions = new[] {
                posError1,
                posError2,
                posError3,
                posError4
            };
            foreach (NumericUpDown errorPosition in errorsPositions)
            {
                errorPosition.Maximum = Constants.FrameSize * 8;
                errorPosition.Minimum = -1;
                errorPosition.Value = -1;
            }
            numErrorCorrectible.Maximum = Constants.FrameSize * 8 / 13;
            numErrorDetectable.Maximum = Constants.FrameSize * 8 / 13;
            numIrrecoverable.Maximum = Constants.FrameSize * 8 / 13;

            // Open source files in read mode
            FileStream sourceFile1 = File.Open(station1Parameters.SourceFilePath, FileMode.Open, FileAccess.Read);
            FileStream sourceFile2 = File.Open(station2Parameters.SourceFilePath, FileMode.Open, FileAccess.Read);
            //We get the file size for the progress bar.
            transfertBar.Maximum = (int)sourceFile1.Length;


            // Open destination file in write mode
            FileStream destinationFile1 = File.Open(station1Parameters.DestinationFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            FileStream destinationFile2 = File.Open(station2Parameters.DestinationFilePath, FileMode.OpenOrCreate, FileAccess.Write);

            // Reset destination file content
            destinationFile1.SetLength(0);
            destinationFile1.Flush();
            destinationFile2.SetLength(0);
            destinationFile2.Flush();

            //Start the threads
            cable = new Transmitter();

            station1 = new Station(Constants.StationId.Station1, cable, station1Parameters.BufferSize, station1Parameters.Timeout, station1Parameters.DetectionOnly, sourceFile1, destinationFile1, ShowFrame);
            station2 = new Station(Constants.StationId.Station2, cable, station2Parameters.BufferSize, station2Parameters.Timeout, station2Parameters.DetectionOnly, sourceFile2, destinationFile2, ShowFrame);

            sendThread = new Thread(station1.Start);
            receiveThread = new Thread(station2.Start);

            txtDataSend.Font = new Font("Lucida Console", 12, FontStyle.Regular);
            txtReception.Font = new Font("Lucida Console", 12, FontStyle.Regular);

            sendThread.Start();
            receiveThread.Start();
        }

        private void btnInject_Click(object sender, EventArgs e)
        {
            if (envoie)
            {
                foreach (NumericUpDown errorPosition in errorsPositions)
                {
                    if (errorPosition.Validate() && errorPosition.Value != -1)
                    {
                        cable.IndicesInversions.Add((int)errorPosition.Value);
                    }
                }
                timerEnvoie.Start();
                envoie2 = false;
            }
        }

        private void ShowFrame(Frame frameToShow, Constants.FrameEvent frameEvent, Constants.StationId stationId)
        {
            bool isSent = frameEvent == Constants.FrameEvent.FrameSent;

            //based on msdn doc: https://msdn.microsoft.com/en-us/library/ms171728(v=vs.110).aspx
            //Thread safe call to richtextbox
            RichTextBox textBox = isSent ? txtDataSend : txtReception;
            if (textBox.InvokeRequired)
            {
                UpdateDataDelegate d = ShowFrame;
                Invoke(d, frameToShow, frameEvent, stationId);
            }
            else
            {
                //We must push the text backward because we insert at the begining.
                OnAppend(Environment.NewLine, textBox, DefaultForeColor);

                string errorMessage = "|";
                Color errorColor = DefaultForeColor;
                switch (frameEvent)
                {
                    case Constants.FrameEvent.FrameReceivedCorrupted:
                        errorMessage += "Détecté";
                        errorColor = Color.DarkOrange;
                        break;
                    case Constants.FrameEvent.FrameReceivedNotAwaited:
                        break;
                    case Constants.FrameEvent.FrameReceivedDuplicate:
                        break;
                    case Constants.FrameEvent.FrameReceivedOk:
                        break;
                    case Constants.FrameEvent.FrameReceivedCorrected:
                        errorMessage += "Corrigé";
                        errorColor = Color.DarkGreen;
                        break;
                    case Constants.FrameEvent.FrameSent:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(frameEvent), frameEvent, null);
                }
                OnAppend(errorMessage, textBox, errorColor);
                OnAppend("        ", textBox, DefaultForeColor);
                OnAppend(" | " + frameToShow.DataSize.ToString("000"), textBox, DefaultForeColor);
                OnAppend(" | " + ((Int16)frameToShow.Ack).ToString("00"), textBox, DefaultForeColor);

                switch (frameToShow.Type)
                {
                    case Constants.FrameType.Ack:
                        OnAppend("Ack ", textBox, Color.DarkGreen);
                        break;
                    case Constants.FrameType.Nak:
                        OnAppend("Nak ", textBox, Color.DarkRed);
                        break;
                    case Constants.FrameType.Data:
                        OnAppend("Data", textBox, DefaultForeColor);
                        break;
                }
                OnAppend(((Int16)frameToShow.Id).ToString("000") + " | ", textBox, DefaultForeColor);

                //Updating the progress bar.
                if (stationId == Constants.StationId.Station2 && (frameEvent == Constants.FrameEvent.FrameReceivedOk ||
                                                                  frameEvent == Constants.FrameEvent.FrameReceivedCorrected))
                {
                    transfertBar.Value += (int)frameToShow.DataSize;
                    if (transfertBar.Value == transfertBar.Maximum)
                    {
                        MessageBox.Show(this, "Tranfert complété!");
                    }
                }
            }
        }

        void OnAppend(string text, RichTextBox textBox, Color textColor)
        {
            textBox.SelectionColor = textColor;
            textBox.Text = text + textBox.Text;
            textBox.SelectionColor = textBox.ForeColor;
        }

        private void btnInjectTypeError_Click(object sender, EventArgs e)
        {
            if (envoie2)
            {
                int errorNumber = 0;
                if (numErrorCorrectible.Validate())
                {
                    for (int i = (int)numErrorCorrectible.Value; i > 0; --i)
                    {
                        cable.InsertRandomErrors(1, errorNumber * 13, (errorNumber + 1) * 13);
                        ++errorNumber;
                    }
                }
                if (numErrorDetectable.Validate())
                {
                    for (int i = (int)numErrorDetectable.Value; i > 0; --i)
                    {
                        cable.InsertRandomErrors(2, errorNumber * 13, (errorNumber + 1) * 13);
                        ++errorNumber;
                    }
                }
                if (numIrrecoverable.Validate())
                {
                    for (int i = (int)numIrrecoverable.Value; i > 0; --i)
                    {
                        cable.InsertRandomErrors(3, errorNumber * 13, (errorNumber + 1) * 13);
                        ++errorNumber;
                    }
                }
                timerEnvoie2.Start();
                envoie2 = false;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Stoping the two stattion
            station1.Stop();
            station2.Stop();
        }

        private void timerEnvoie_Tick(object sender, EventArgs e)
        {
            timerEnvoie.Stop();
            envoie = true;
        }

        private void timerEnvoie2_Tick(object sender, EventArgs e)
        {
            timerEnvoie2.Stop();
            envoie2 = true;
        }
    }
}
