using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TransmissionSimulation.Ressources;

namespace TransmissionSimulation
{
    public partial class MainForm : Form
    {
        private ProgramOption progOption;
        public MainForm(ProgramOption progOption)
        {
            this.progOption = progOption;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            nbErrors.Maximum = Constants.MaximumOfErrorToInject;

            //Start the threads
            //TODO ajouter dequoi pour envoyer des erreurs a raph

        }

        private void btnInject_Click(object sender, EventArgs e)
        {
            //TODO call l'injection d'erreur(s) a raph
        }
    }
}
