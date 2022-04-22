using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;
using System.Windows.Threading;
using System.Threading;

namespace Chat_Socket
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket socket = null;
        //DispatcherTimer dTimer = null;

        Thread thread = null;

        string Babbo = "127.0.0.1";
        string Mamma = "10.0.0.2";
        string Nonno = "10.0.0.3";
        string Nonna = "10.0.0.4";


        public MainWindow()
        {
            InitializeComponent();

            Agenda();

            //con "InterNetwork" definisco la comunicazione con degli indirizzi ipv4
            //con "Dgram" definisco l'utilizzo del protocollo UDP
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //inizializzo il thread e lo avvio
            thread = new Thread(new ThreadStart(aggiornamento_dTimer));
            thread.Start();


            IPAddress local_address = IPAddress.Any; //indico il MITTENTE
            IPEndPoint local_endpoint = new IPEndPoint(local_address.MapToIPv4(), 64000);

            lblProva.Content = "porta 64000";

            socket.Bind(local_endpoint);

            //dTimer = new DispatcherTimer(); //creo l'oggetto

            //dTimer.Tick += new EventHandler(aggiornamento_dTimer); //allo scadere del timer (tempo) esegui il METODO con le sue istruzioni
            //dTimer.Interval = new TimeSpan(0, 0, 0, 0, 250); //tempo di intervallo
            //dTimer.Start();
        }

        private void Agenda()
        {
            cmbAgenda.Items.Add("Babbo - " + Babbo);
            cmbAgenda.Items.Add("Mamma - " + Mamma);
            cmbAgenda.Items.Add("Nonno - " + Nonno);
            cmbAgenda.Items.Add("Nonna - " + Nonna);
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            if (txtPorta.Text == "64000")
            {
                MessageBox.Show("La porta inserita NON è valida essendo quella di appartenenza.");
            }
            else
            {
                IPAddress remote_address = IPAddress.Parse(txtIP.Text); //"prendo l'ip DESTINATARIO" 
                IPEndPoint remote_endpoint = new IPEndPoint(remote_address, int.Parse(txtPorta.Text));

                byte[] messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text);
                socket.SendTo(messaggio, remote_endpoint);
            }

        }

        private void aggiornamento_dTimer(/*object sender, EventArgs e (COMMENTATO PER IL FUNZIONAMENTO DEL THREAD)*/)
        {

            while(true)
            {
                int nBytes = 0;

                if ((nBytes = socket.Available) > 0)
                {
                    //ricezione dei caratteri in attesa
                    byte[] buffer = new byte[nBytes];

                    EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    nBytes = socket.ReceiveFrom(buffer, ref remoteEndPoint);

                    string from = ((IPEndPoint)remoteEndPoint).Address.ToString();

                    string messaggio = Encoding.UTF8.GetString(buffer, 0, nBytes);

                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        lstMessaggi.Items.Add(messaggio);
                    }));

                }

                Thread.Sleep(1);
            }
        }

        private void cmbAgenda_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtIP.Text = cmbAgenda.SelectedItem.ToString().Split('-')[1].Trim();
        }
    }
}
