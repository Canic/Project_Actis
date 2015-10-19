using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Data.SqlClient;
using System.Threading;

namespace Project_Actis
{
    class Program
    {
        public static List<Memory> StaffList = new List<Memory>();
        public static TcpListener _listener;

        static void Main(string[] args)
        {
            Console.Title = "KeRn GameMaster Toolkit - Project Actis!";
            _listener = new TcpListener(IPAddress.Any, 8056);
            _listener.Start();
            WaitForClientConnect();
            Thread consoleInput = new Thread(new ThreadStart(consoleThread));
            consoleInput.Start();
        }

        public static bool removeClient(string IPAddress)
        {
            var item = StaffList.First(kvp => kvp.IPAddress == IPAddress);
            if (StaffList.Remove(item))
                return true;
            else
                return false;

        }

        private static void consoleThread()
        {
            while (true)
            {
                string consoleInput = Console.ReadLine();
                if (consoleInput == "exit")
                    Environment.Exit(0);
            }
        }

        private static void WaitForClientConnect()
        {
            _listener.BeginAcceptTcpClient(OnClientConnect, _listener);
        }

        private static void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                WaitForClientConnect();
                TcpClient clientSocket = default(TcpClient);
                clientSocket = _listener.EndAcceptTcpClient(asyn);
                HandleClientRequest clientReq = new HandleClientRequest(clientSocket);
                clientReq.StartClient();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class HandleClientRequest
    {
        TcpClient _clientSocket;
        MyFunction _myFunction = new MyFunction();
        NetworkStream _networkStream = null;

        public HandleClientRequest(TcpClient clientConnected)
        {
            this._clientSocket = clientConnected;
        }
        public void StartClient()
        {
            _networkStream = _clientSocket.GetStream();
            _myFunction.WriteLog("New Connection from " + ((IPEndPoint)_clientSocket.Client.RemoteEndPoint).Address.ToString(), ConsoleColor.Yellow);
            WaitForRequest();
        }

        public void WaitForRequest()
        {
            try
            {
                byte[] buffer = new byte[_clientSocket.ReceiveBufferSize];

                _networkStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
            }
            catch
            {
                Program.removeClient(((IPEndPoint)_clientSocket.Client.RemoteEndPoint).Address.ToString());
                _myFunction.WriteLog("Forcibly closed connection and delete IP from Dictionary.", ConsoleColor.Yellow);
            }
        }

        private void ReadCallback(IAsyncResult result)
        {
            NetworkStream networkStream = _clientSocket.GetStream();
            networkStream.ReadTimeout = 100;
            Actis_SQL_Class _SQLClass = new Actis_SQL_Class();
            try
            {
                int read = networkStream.EndRead(result);
                if (read == 0)
                {
                    Disconect(networkStream);
                    return;
                }

                byte[] buffer = result.AsyncState as byte[];
                string[] data = _myFunction.Decrypt(Encoding.Default.GetString(buffer, 0, read)).Split('\t');

                if (Program.StaffList.Where(kdv => kdv.IPAddress == ((IPEndPoint)_clientSocket.Client.RemoteEndPoint).Address.ToString()).Count() == 1)
                {
                    switch (Convert.ToInt16(data[0]))
                    {
                        case 0:
                            string tmpStaff = Program.StaffList.First(kdv => kdv.IPAddress == ((IPEndPoint)_clientSocket.Client.RemoteEndPoint).Address.ToString()).Username;
                            _myFunction.WriteFileLog("Successfully logged out", tmpStaff);
                            _myFunction.WriteLog("Successfully logged out " + tmpStaff, ConsoleColor.Cyan);
                            Program.removeClient(((IPEndPoint)_clientSocket.Client.RemoteEndPoint).Address.ToString());
                            break;
                        case 1:
                            _myFunction.WriteFileLog("Searched for Character by function: " + data[1] + " for Character: " + data[2], Program.StaffList.First(kdv => kdv.IPAddress == ((IPEndPoint)_clientSocket.Client.RemoteEndPoint).Address.ToString()).Username);
                            sendToClient(_SQLClass.searchCharacter(data), networkStream);
                            break;
                        case 2:
                            sendToClient(_SQLClass.searchCharacterBysID(data[1]), networkStream);
                            break;
                        case 3:
                            sendToClient(_SQLClass.gameLogFromsID(data[1]), networkStream);
                            sendToClient(_SQLClass.inventoryFromsID(data[1]), networkStream);
                            break;
                        case 4:
                            sendToClient(_SQLClass.getCharIDFromNo(data[1]), networkStream);
                            break;
                        case 5:
                            if (Program.StaffList.First(kdv => kdv.IPAddress == ((IPEndPoint)_clientSocket.Client.RemoteEndPoint).Address.ToString()).AuthID > 2)
                                sendToClient(Convert.ToString(_SQLClass.updateCharacter(data[2], Convert.ToInt32(data[1]))), networkStream);
                            else
                                sendToClient("false", networkStream);
                            break;
                        case 6:
                            sendToClient(_SQLClass.selectAccount(Convert.ToInt16(data[1]), data[2]), networkStream);
                            break;
                        case 7:
                            sendToClient(_SQLClass.selectAccountLoginLog(Convert.ToInt32(data[1])), networkStream);
                            sendToClient(_SQLClass.selectAccountMulti(Convert.ToInt32(data[1])), networkStream);
                            break;
                        case 8:
                            sendToClient(Convert.ToString(_SQLClass.bannAccount(data, Program.StaffList.First(kdv => kdv.IPAddress == ((IPEndPoint)_clientSocket.Client.RemoteEndPoint).Address.ToString()).Username)), networkStream);
                            break;
                        case 9:
                            if (Program.StaffList.First(kdv => kdv.IPAddress == ((IPEndPoint)_clientSocket.Client.RemoteEndPoint).Address.ToString()).AuthID > 2 && Convert.ToInt16(data[2]) == 0)
                                sendToClient(Convert.ToString(_SQLClass.bannMacAccount(data)), networkStream);
                            else if (Convert.ToInt16(data[2]) == 1)
                                sendToClient(Convert.ToString(_SQLClass.bannMacAccount(data)), networkStream);
                            else
                                sendToClient("ERROR", networkStream);
                            break;
                        case 10:
                            sendToClient(Convert.ToString(_SQLClass.sendNewPassword(Convert.ToInt32(data[1]))), networkStream);
                            break;
                        case 11:
                            sendToClient(_SQLClass.selectCharacterInventory(Convert.ToInt32(data[1])), networkStream);
                            break;
                        default:
                            _myFunction.WriteLog("Unknown Case..!", ConsoleColor.Blue);
                            break;
                    }
                }
                else
                {
                    if (data.Count() == 2)
                    {
                        SqlConnection conn = new SqlConnection("user id=sa;password=xMfXE\\K{XpLV=K*o;server=.\\SQLEXPRESS;database=OdinAccounts;connection timeout=30");
                        SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM tAccounts WHERE sUsername = @sUsername AND sUserPass = @sUserPass;", conn);
                        cmd.Parameters.AddWithValue("@sUsername", data[0]);
                        cmd.Parameters.AddWithValue("@sUserPass", data[1]);

                        conn.Open();
                        int respone = (int)cmd.ExecuteScalar();
                        if (respone == 0)
                        {
                            sendToClient("RS0", networkStream);
                        }
                        else
                        {
                            cmd = new SqlCommand("SELECT nAuthID FROM tAccounts WHERE sUsername = @sUsername;", conn);
                            cmd.Parameters.AddWithValue("@sUsername", data[0]);
                            respone = (int)cmd.ExecuteScalar();
                            conn.Close();

                            if (respone >= 2)
                            {
                                sendToClient("AuthOK#" + data[0] + "#" + respone, networkStream);
                                Memory item = new Memory();
                                item.AuthID = Convert.ToInt16(respone);
                                item.IPAddress = ((IPEndPoint)_clientSocket.Client.RemoteEndPoint).Address.ToString();
                                item.Username = data[0];
                                Program.StaffList.Add(item);
                                _myFunction.WriteLog("Successfully logged in with ID: " + data[0], ConsoleColor.Cyan);
                                _myFunction.WriteFileLog("Successfully logged in!", Program.StaffList.First(kdv => kdv.IPAddress == ((IPEndPoint)_clientSocket.Client.RemoteEndPoint).Address.ToString()).Username);
                            }
                            else
                                sendToClient("RS0", networkStream);
                        }
                    }
                    else
                    {
                        //Dafuq? Cheating.
                        Disconect(networkStream);
                        _myFunction.WriteLog(((IPEndPoint)_clientSocket.Client.RemoteEndPoint).Address.ToString() + " tryed to cheat me. WOW!", ConsoleColor.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            this.WaitForRequest();
        }

        private void Disconect(NetworkStream nstream)
        {
            _networkStream.Close();
            _clientSocket.Close();
        }

        private void sendToClient(string Text, NetworkStream nstream)
        {
            Byte[] sendBytes = Encoding.ASCII.GetBytes(_myFunction.Encrypt(Text));
            Byte[] merge = new byte[sendBytes.Length + 4];
            BitConverter.GetBytes(sendBytes.Length).CopyTo(merge, 0);
            sendBytes.CopyTo(merge, 4);
            _networkStream.Write(merge, 0, merge.Length);
            _networkStream.Flush();
        }
    }
}

class Memory
{
    public string IPAddress { get; set; }
    public string Username { get; set; }
    public Int16 AuthID { get; set; }
}
