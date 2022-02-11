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
                    tssClient.Text = "Client verbonden";
                    tssClient.ForeColor = Color.Green;
                }
            }
            catch (Exception)
            {
                txtMelding.AppendText("Kan geen verbinding maken!\r\n");
            }
        }

        private void bgWorkerOntvang_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                string bericht;
                try
                {
                    bericht = Reader.ReadLine();
                    if (bericht == "Disconnect")
                    {
                        txtMelding.AppendText("verbinding verbroken door server\r\n");
                        break;
                    }
                    this.txtCommunicatie.Invoke(new MethodInvoker(delegate () { txtCommunicatie.AppendText(bericht + "\r\n"); }));
                }
                catch (Exception)
                {
                    this.txtMelding.Invoke(new MethodInvoker(delegate () { txtMelding.AppendText("Kan bericht niet ontvangen.\r\n"); }));
                }
            }
        }

        private void bgWorkerOntvang_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtMelding.AppendText("Verbinding met server werd verbroken\r\n");
            btnVerbreek.Enabled = false;
            btnZoekServer.Enabled = true;
        }

        private void btnZend_Click(object sender, EventArgs e)
        {
            try
            {
                Writer.WriteLine("CLIENT>>>" + txtBericht.Text);
                txtCommunicatie.AppendText("CLIENT>>>" + txtBericht.Text + "\r\n");
            }
            catch
            {
                txtMelding.AppendText("Bericht zenden mislukt");
            }
        }

        private void btnVerbreek_Click(object sender, EventArgs e)
        {
            try
            {
                Writer.WriteLine("Disconnect");
                bgWorkerOntvang.CancelAsync();
                client.Close();
                txtMelding.AppendText("Verbinding verbroken door Client!\r\n");
                tssClient.Text = "Client niet verbonden";
                tssClient.ForeColor = Color.Red;
            }
            catch
            {
                txtMelding.AppendText("Verbinding verbreken door Client mislukt!\r\n");
            }
        }
    }   
}
