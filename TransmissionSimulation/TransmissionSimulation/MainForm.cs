using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly ProgramOption progOption;
        private Thread sendThread;
        private Thread receiveThread;
        private Transmitter cable;
        private NumericUpDown[] errorsPositions;
        delegate void UpdateDataDelegate(Frame frameToShow, bool isSent);

        public MainForm(ProgramOption progOption)
        {
            this.progOption = progOption;
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

            FileStream fileToCopie = File.Open(progOption.FileToCopie, FileMode.Open, FileAccess.Read);
            FileStream destinationFile = File.Open(progOption.DestinationFile, FileMode.OpenOrCreate, FileAccess.Write);
            destinationFile.SetLength(0);
            destinationFile.Flush();
            //Start the threads
            cable = new Transmitter();
            Station sendStation = new Station(Constants.Station.Station1, cable, progOption.BufferSize, progOption.Timeout * 1000, fileToCopie, ShowFrame);
            Station receiveStation = new Station(Constants.Station.Station2, cable, progOption.BufferSize, progOption.Timeout * 1000, destinationFile, ShowFrame);
            sendThread = new Thread(sendStation.Start);
            receiveThread = new Thread(receiveStation.Start);

            txtDataSend.Font = new Font("Lucida Console", 12, FontStyle.Regular);
            txtReception.Font = new Font("Lucida Console", 12, FontStyle.Regular);

            sendThread.Start();
            receiveThread.Start();
        }

        private void btnInject_Click(object sender, EventArgs e)
        {
            foreach (NumericUpDown errorPosition in errorsPositions)
            {
                if (errorPosition.Validate() && errorPosition.Value != -1)
                {
                    cable.IndicesInversions.Add((int)errorPosition.Value);
                }
            }
        }

        private void ShowFrame(Frame frameToShow, bool isSent)
        {
            //based on msdn doc: https://msdn.microsoft.com/en-us/library/ms171728(v=vs.110).aspx
            RichTextBox textBox = isSent ? txtDataSend : txtReception;
            if (textBox.InvokeRequired)
            {
                UpdateDataDelegate d = ShowFrame;
                Invoke(d, frameToShow, isSent);
            }
            else
            {
                OnAppend(frameToShow.ToString(), textBox);
                OnAppend(Environment.NewLine, textBox);
            }
            //TODO voir avec félix pour avoir un niveau de progression
            //On doit prendre le size initiale du fichier, et faire une proportion avec la taille de chaque trame
        }

        //Prevent autoscroll in richtextbox
        //https://stackoverflow.com/questions/626988/prevent-autoscrolling-in-richtextbox
        //taken on: 08-06-2017
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);
        const int WM_USER = 0x400;
        const int EM_HIDESELECTION = WM_USER + 63;

        void OnAppend(string text, RichTextBox textBox)
        {
            bool focused = textBox.Focused;
            //backup initial selection
            int selection = textBox.SelectionStart;
            int length = textBox.SelectionLength;
            //allow autoscroll if selection is at end of text
            bool autoscroll = selection == textBox.Text.Length;

            if (!autoscroll)
            {
                //shift focus from RichTextBox to some other control
                if (focused)
                {
                    dataSendGroup.Focus();
                }
                //hide selection
                SendMessage(textBox.Handle, EM_HIDESELECTION, 1, 0);
            }

            textBox.AppendText(text);

            if (!autoscroll)
            {
                //restore initial selection
                textBox.SelectionStart = selection;
                textBox.SelectionLength = length;
                //unhide selection
                SendMessage(textBox.Handle, EM_HIDESELECTION, 0, 0);
                //restore focus to RichTextBox
                if (focused)
                {
                    textBox.Focus();
                }
            }
        }

        private void btnInjectTypeError_Click(object sender, EventArgs e)
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
        }
    }
}
