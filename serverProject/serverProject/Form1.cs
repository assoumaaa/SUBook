using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace serverProject
{
    public partial class Form1 : Form
    {
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<Socket> clientSockets = new List<Socket>();
        List<bool> checking_posts = new List<bool>();

        bool terminating = false, listening = false;
        int postID = 0, clients = -1;


        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }


        private void button_listen_Click(object sender, EventArgs e)
        {
            int serverPort;
            if (Int32.TryParse(box_port.Text, out serverPort))
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, serverPort);
                serverSocket.Bind(endPoint);
                serverSocket.Listen(3);
                listening = true;
                button_listen.Enabled = false;

                Thread acceptThread = new Thread(Accept);
                acceptThread.Start();

                serverLogs.AppendText("Started listening on port: " + serverPort + "\n");
            }
            else serverLogs.AppendText("Check the port number!\n");

        }


        private void Accept()
        {
           while (listening)
                {
                    try
                    {
                        Socket newClient = serverSocket.Accept();
                        clientSockets.Add(newClient);
                        clients++;

                        Thread receiveThread = new Thread(() => Receive(newClient));
                        receiveThread.Start();
                    }
                    catch
                    {
                        if (terminating) listening = false;
                        else serverLogs.AppendText("The socket stopped working!\n");
                    }
                }
        }


        private void Receive(Socket thisClient)
        {
            string msg = "", incomingMsg="", username = "";
            bool connected = true;
            checking_posts.Add(false);
            checking_posts[clients] = false;



      
            string path = @"../../postID.txt";
            if (!File.Exists((path)) )
            {  
                using (StreamWriter sw = File.CreateText(path))
                    sw.WriteLine(0);
            }
            else {
                foreach (string line in System.IO.File.ReadLines(@"../../postID.txt"))
                {
                    int number = Int32.Parse(line);
                    postID = number;  
                }
            }



            while (connected && !terminating)
            {
                try
                {
                    bool found = false;
   
                    Byte[] buffer = new Byte[64];
                    thisClient.Receive(buffer);

                    incomingMsg = Encoding.Default.GetString(buffer);
                    incomingMsg = incomingMsg.Substring(0, incomingMsg.IndexOf("\0")).Trim();


                    // first time you try to connect
                    if (checking_posts[clients] == false)
                    {
                        username = incomingMsg;
                        foreach (string line in System.IO.File.ReadLines(@"../../user-db.txt"))
                            if (line.Trim() == incomingMsg.Trim()) found = true;


                        if (found == false)
                        {
                            serverLogs.AppendText(incomingMsg + " has tried to connect to the server but cannot! \n");
                            msg = "dont";
                        }
                        else
                        {
                            serverLogs.AppendText(incomingMsg + " has connected! \n");
                            msg = "create";
                            checking_posts[clients] = true;                       
                        }

                        // sending the msg back to client to print accordingly
                        buffer = Encoding.Default.GetBytes(msg.Trim());
                        thisClient.Send(buffer);
                    }
                    // if you want to check if the response is allposts or not
                    else if ( checking_posts[clients] == true)
                    {

                        if (incomingMsg.Substring(0, 8) == "allposts")
                        {

                            string nameFromClient = incomingMsg.Substring(8).Trim();
                            string posts = "";
            
                            foreach (string line in System.IO.File.ReadLines(@"../../posts.txt"))
                            {
                                if (String.IsNullOrWhiteSpace(line) == false )
                                {
                                    char[] delims = new[] { '*' };
                                    string[] splits = line.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                                    string usrname = splits[1];

                                    Thread.Sleep(100);
                                    if (usrname != nameFromClient )
                                    {
                                        buffer = Encoding.Default.GetBytes("%" + line);
                                        thisClient.Send(buffer);
                                        posts = posts + line + "\n";
                                    }
                                }
                            }
                            if (posts == "")
                                serverLogs.AppendText("No posts to be shown to clients! \n");
                            else
                                serverLogs.AppendText("Showed all posts for " + nameFromClient + ".\n");
                        }
                        // if you want to send a post
                        else
                        {
                            char[] delims = new[] { '*' };
                            string[] splits = incomingMsg.Split(delims, StringSplitOptions.RemoveEmptyEntries);

                            string name = splits[0].Trim();
                            string post = splits[1].Trim();

                            serverLogs.AppendText(name + " has sent a post:\n");
                            serverLogs.AppendText(post + "\n");
                            string date = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");

                            using (StreamWriter file = new StreamWriter("../../posts.txt", append: true))
                                file.WriteLine(date + "*" + name + "*" + postID + "*" + post + "\n");


               
                      
                            
                          
                      

                            postID += 1;
                            string neww = postID.ToString();
                            string[] nbr = File.ReadAllLines(@"../../postID.txt");
                            System.IO.File.WriteAllText(@"../../postID.txt", neww);
                        }
                    }   
                }
                catch
                {
                    if (!terminating) serverLogs.AppendText(username + " has disconneted!\n");
                    thisClient.Close();
                    clientSockets.Remove(thisClient);
                    connected = false;
                }
            }             
        }


        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
                listening = false;
                terminating = true;
                Environment.Exit(0);
        }
    }
}

