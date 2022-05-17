using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clientProject
{
    public partial class Form1 : Form
    {
        Socket clientSocket;
        bool connected = true;
        bool terminating = false;

        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string IP = text_ip.Text;

            int port;
            if (Int32.TryParse(text_port.Text, out port) && text_ip.Text != "" == true && text_username.Text != "")
            {
                    try
                    {
                        clientSocket.Connect(IP, port);
                        button_connect.Enabled = false;
                        button_disconnect.Enabled = true;
                        button_send.Enabled = true;
                        button_allposts.Enabled = true;
                        text_post.Enabled = true;

                        connected = true;

                        Byte[] buffer;
                        buffer = Encoding.Default.GetBytes(text_username.Text.Trim());
                        clientSocket.Send(buffer);

                        Thread receiveThread = new Thread(Receive);
                        receiveThread.Start();
                    }
                    catch { clientLogs.AppendText("Could not connect to server!\n"); }
            }
            else
            {
                if (text_ip.Text != "" == false) clientLogs.AppendText("Check the IP number!\n");
                if (text_username.Text == "") clientLogs.AppendText("Check username!\n");
                if ( Int32.TryParse(text_port.Text, out port) == false )   clientLogs.AppendText("Check the port number!\n");
            }
        }

        private void Receive()
        {
            while (connected)
            {
                try
                {
                    Byte[] buffer = new Byte[64];
                    clientSocket.Receive(buffer);

               
                    string incomingMsg = Encoding.Default.GetString(buffer);
                    string name = text_username.Text;
                    incomingMsg = incomingMsg.Substring(0, incomingMsg.IndexOf("\0"));


                    if (incomingMsg == "create")
                        clientLogs.AppendText("Hello " + name + "! You are connected! \n");


                    else if (incomingMsg == "dont")
                    {
                        clientLogs.AppendText("Please enter a valid username!\n");
                        button_connect.Enabled = true;
                        button_disconnect.Enabled = false;
                        button_send.Enabled = false;
                        button_allposts.Enabled = false;
                        text_post.Enabled = false;
                    }
                    else if ( incomingMsg.Substring(0,1) == "%")
                    {
                        string recievedMsg = incomingMsg.Substring(1);
                        char[] delims = new[] { '*' };
                        string[] splits = recievedMsg.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                              
                        string time = splits[0].Trim();
                        string usrname = splits[1].Trim();
                        string post = splits[2].Trim();
                        string msgs = splits[3].Trim();

                        clientLogs.AppendText("Showing all posts from clients:\n");
                        clientLogs.AppendText("Username: " + usrname + "\n");
                        clientLogs.AppendText("PostID: " + post + "\n");
                        clientLogs.AppendText("Post: " + msgs + "\n");
                        clientLogs.AppendText("Time: " + time + "\n\n");
                    }
                    else if (incomingMsg.Substring(0, 1) == "")
                        clientLogs.AppendText("No posts to be shown! \n");
                }
                catch
                {
                    if (!terminating && connected)
                    {
                        clientLogs.AppendText("The server has discconected!\n");
                        button_connect.Enabled = true;
                        button_disconnect.Enabled = false;
                        button_send.Enabled = false;
                        button_allposts.Enabled = false;
                        text_post.Enabled = false;
                    }
                    clientSocket.Close();
                    connected = false;
                }
            }
        }

        private void button_disconnect_Click(object sender, EventArgs e)
        {
            clientLogs.AppendText("Successfully disconnected!\n");
            button_connect.Enabled = true;
            button_disconnect.Enabled = false;
            button_send.Enabled = false;
            button_allposts.Enabled = false;
            text_post.Enabled = false;
            connected = false;
            terminating = false;
            text_post.Clear();
            clientSocket.Close();
        }

        private void button_send_Click(object sender, EventArgs e)
        {  
            string name = text_username.Text;
            string msg = text_post.Text;
            string full = name + " * " + msg;

            Byte[] buffer;
            buffer = Encoding.Default.GetBytes(full);
            clientSocket.Send(buffer);

            clientLogs.AppendText("You have successfully sent a post!\n");
            clientLogs.AppendText(name + " : " + msg + "\n");  
        }

        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            connected = false;
            terminating = true;
            Environment.Exit(0);
        }

        private void button_allposts_Click(object sender, EventArgs e)
        {
            string full = "allposts" + text_username.Text;
            Byte[] buffer;
            buffer = Encoding.Default.GetBytes(full);
            clientSocket.Send(buffer);
        }
    }
}
