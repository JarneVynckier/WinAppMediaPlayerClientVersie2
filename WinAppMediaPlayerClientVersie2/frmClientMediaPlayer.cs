using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets; //TCPClient en TCPServer verbinden
using System.Net; //IPadressen gebruiken
using System.IO; //berichten versturen en ontvangen


namespace WinAppMediaPlayerClientVersie2
{
    public partial class frmClientMediaPlayer : Form
    {
        TcpClient client; //verbinden met Server via Socket
        StreamReader Reader;//berichten ontvangen
        StreamWriter Writer;//berichten versturen
        public frmClientMediaPlayer()
        {
            InitializeComponent();
        }

        private void btnZoekServer_Click(object sender, EventArgs e)
        {
            //controle IP-adres
            IPAddress ipadres;
            int poortNr;
            if (!IPAddress.TryParse(mtxtIPadres.Text.Replace(" ", ""), out ipadres))
            {
                txtMelding.AppendText("Ongeldig IP-Adres!\r\n");
                mtxtIPadres.Focus();
                return;
            }
            if (!int.TryParse(mtxtPoortnr.Text, out poortNr))
            {
                txtMelding.AppendText("ongeldig poortnummer!\r\n");
                mtxtPoortnr.Focus();
                return;
            }
            //verbinding maken server
            try
            {
                client = new TcpClient();
                client.Connect(ipadres, poortNr);
                if (client.Connected)
                {
                    Writer = new StreamWriter(client.GetStream());
                    Reader = new StreamReader(client.GetStream());
                    Writer.AutoFlush = true;
                    bgWorkerOntvang.WorkerSupportsCancellation = true;
                    bgWorkerOntvang.RunWorkerAsync();//start ontvangen data
                    btnZoekServer.Enabled = false;
                    btnVerbreek.Enabled = true;
                    //SplitContainer1.Panel2.Enabled = true;
                }
            }
            catch (Exception)
            {
                txtMelding.AppendText("Kan geen verbinding maken!\r\n");
            }
        }
    }   
}
