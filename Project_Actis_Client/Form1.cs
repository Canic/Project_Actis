using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Globalization;

namespace Project_Actis_Client
{
    public partial class Form1 : Form
    {
        private TcpClient _clientSocket;
        private Memory Memory = new Memory();
        public NetworkStream _networkStream = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Loginbtn_Click(object sender, EventArgs e)
        {
            _clientSocket = new TcpClient();
            _clientSocket.Connect(IPAddress.Parse("188.165.210.213"), 8056);
            if (_clientSocket.Connected)
            {
                SendData(Usernametxtbx.Text + "\t" + Passwordtxtbx.Text);
                string receive = ReceiveData();
                if (receive.Contains("AuthOK#" + Usernametxtbx.Text))
                {
                    Memory.AuthID = Convert.ToInt16(receive.Split('#')[2]);
                    Memory.Username = Usernametxtbx.Text;
                    Initialize();
                }
                else
                {
                    MessageBox.Show("Incorrect creditials!", "RS:0 - Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CloseConnection();
                }
            }
            else
                MessageBox.Show("Currently not connected to destination. Is the Server Online?", "RS:0 - Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Initialize()
        {
            //Permissions
            if (Memory.AuthID == 2)
            {
                ChrUpdatebtn.Enabled = false;
                ACCS_UnbannMacchkbx.Enabled = false;
            }
            else if (Memory.AuthID == 1)
            {
                MessageBox.Show("Please enable your Account first for this Tool!" + Environment.NewLine + Environment.NewLine + "sparkblast@pure-games.net" + Environment.NewLine + "kern@kernstudios.org");
                Environment.Exit(0);
            }

            MenuTreeView.Nodes.Add("");
            MenuTreeView.Nodes.Add("Account");
            CharacterTabControl.TabPages.Remove(tabPage4);
            MenuTreeView.Nodes.Add("Character");
            MenuTreeView.Nodes.Add("Guild");
            MenuTreeView.Nodes.Add("Storage");
            MenuTreeView.Nodes.Add("");
            MenuTreeView.Nodes.Add("Logged in with: " + Memory.Username);
            Loginbtn.Enabled = false;
            Loginbtn.Text = "Logged in...";
            CharacterPanel.Visible = true;

            if (!Directory.Exists("9Data"))
                Directory.CreateDirectory("9Data");
            if (!Directory.Exists("ClientDebug"))
                Directory.CreateDirectory("ClientDebug");

            SHNFile classSHN = new SHNFile("9Data/ClassName.shn");
            Memory.itemInfoSHN = new SHNFile("9Data/ItemInfo.shn");
            foreach (DataRow row in classSHN.table.Rows)
            {
                CHRE_Classcmbx.Items.Add(row["acLocalName"]);
            }
        }

        private void Charactersearchbtn_Click(object sender, EventArgs e)
        {
            if (AccountIDtxtbx.Text.Length >= 1 || AccoutnNotxtbx.Text.Length >= 1 || Charactercmbx.Text.Length >= 1 || Characternotxtbx.Text.Length >= 1)
            {
                string selected = "1\t";
                if (AccountIDtxtbx.Text.Length >= 1)
                    selected += "0\t" + AccountIDtxtbx.Text;
                else if (AccoutnNotxtbx.Text.Length >= 1)
                    selected += "1\t" + AccoutnNotxtbx.Text;
                else if (Charactercmbx.Text.Length >= 1)
                    selected += "2\t" + Charactercmbx.Text;
                else if (Characternotxtbx.Text.Length >= 1)
                    selected += "3\t" + Characternotxtbx.Text;

                SendData(selected);
                string data = ReceiveData();
                Charactercmbx.Items.Clear();
                if(data != "NoRecords")
                {
                    for (int i = 0; i < (data.Split('\t').Count() / 3); i += 3)
                    {
                        if (Convert.ToInt16(selected.Split('\t')[1]) == 0)
                        {
                            Characternotxtbx.Text = data.Split('\t')[i];
                            Charactercmbx.Items.Add(data.Split('\t')[i + 1]);
                            AccoutnNotxtbx.Text = data.Split('\t')[i + 2];
                        }
                        else if (Convert.ToInt16(selected.Split('\t')[1]) == 1)
                        {
                            AccountIDtxtbx.Text = data.Split('\t')[i];
                            Characternotxtbx.Text = data.Split('\t')[i + 1];
                            Charactercmbx.Items.Add(data.Split('\t')[i + 2]);
                        }
                        else if (Convert.ToInt16(selected.Split('\t')[1]) == 2)
                        {
                            Charactercmbx.Items.Add(Charactercmbx.Text);
                            Characternotxtbx.Text = data.Split('\t')[i];
                            AccoutnNotxtbx.Text = data.Split('\t')[i + 1];
                            AccountIDtxtbx.Text = data.Split('\t')[i + 2];
                        }
                        else if (Convert.ToInt16(selected.Split('\t')[1]) == 3)
                        {
                            Charactercmbx.Items.Add(data.Split('\t')[i]);
                            AccoutnNotxtbx.Text = data.Split('\t')[i + 1];
                            AccountIDtxtbx.Text = data.Split('\t')[i + 2];
                        }
                    }
                    Charactercmbx.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("Not found in our Database.", "EX2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void SendData(string dataTosend)
        {
            if (string.IsNullOrEmpty(dataTosend))
                return;
            NetworkStream serverStream = _clientSocket.GetStream();
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(MyFunction.Encrypt(dataTosend));
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }

        public void CloseConnection()
        {
            _clientSocket.Close();
        }

        public string ReceiveData()
        {
            StringBuilder message = new StringBuilder();
            NetworkStream serverStream = _clientSocket.GetStream();
            serverStream.ReadTimeout = 1000;
            Byte[] bufferLength = new byte[4];
            serverStream.Read(bufferLength, 0, bufferLength.Length);
            toolStripProgressBar1.Maximum = BitConverter.ToInt32(bufferLength, 0);
            while (message.Length < BitConverter.ToInt32(bufferLength, 0))
            {
                toolStripProgressBar1.Value = message.Length;
                if (serverStream.DataAvailable)
                {
                    int read = serverStream.ReadByte();
                    message.Append((char)read);
                }
            }
            return MyFunction.Decrypt(message.ToString());
        }

        private void Logoutbtn_Click(object sender, EventArgs e)
        {
            SendData("0");
            CloseConnection();
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            copyrightlbl.Text += ProductVersion;
            CharacterTabControl.SelectedTab = tabPage1;
        }

        private void Charactercmbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            SendData("2\t" + Charactercmbx.SelectedItem);
            string data = ReceiveData();
            if (data == "NoRecords")
            {
                MessageBox.Show("No records found!", "RS1 - Func3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string[] datasplit = data.Split('\t');
            //Search
            AccountIDtxtbx.Text = datasplit[16];
            //Character Editor
            CHRE_CharacterIDtxtbx.Text = datasplit[0];
            CHRE_AdminLevelnmrup.Value = Convert.ToInt32(datasplit[1]);
            CHRE_LoginZonetxtbx.Text = datasplit[2];
            CHRE_Levelnmrcup.Value = Convert.ToInt32(datasplit[3]);
            CHRE_Exptxtbx.Text = datasplit[4];
            CHRE_Moneytxtbx.Text = datasplit[5];
            CHRE_Fametxtbx.Text = datasplit[6];
            CHRE_Prisontxtbx.Text = datasplit[7];
            CHRE_PlayTimetxtbx.Text = TimeSpan.FromMinutes(Convert.ToDouble(datasplit[8])).ToString();
            CHRE_Strengthtxtbx.Text = datasplit[9];
            CHRE_Constitutetxtbx.Text = datasplit[10];
            CHRE_Dexteritytxtbx.Text = datasplit[11];
            CHRE_Intelligencetxtbx.Text = datasplit[12];
            CHRE_MentalPowertxtbx.Text = datasplit[13];
            CHRE_Classcmbx.SelectedIndex = Convert.ToInt32(datasplit[14]);
            CHRE_Gendercmbx.SelectedIndex = Convert.ToInt32(datasplit[15]);
        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            if(Charactercmbx.Text.Length != 0)
            {
                SendData("3\t" + Charactercmbx.Text);
                string GameLog = ReceiveData();
                string Inventory = ReceiveData();
                DataTable gamelogdt = new DataTable();
                DataTable inventorydt = new DataTable();
                StringReader theReader = new StringReader(GameLog);
                DataSet theDataSet = new DataSet();
                theDataSet.ReadXml(theReader);
                gamelogdt = theDataSet.Tables[0];
                theReader = new StringReader(Inventory);
                theDataSet.ReadXml(theReader);
                inventorydt = theDataSet.Tables[0];
                GameLog_Parser gmp = new GameLog_Parser();
                GALOG_Logtxtbx.Text = String.Join(Environment.NewLine, gmp.Main(gamelogdt, inventorydt));

                if (gmp.charNO.Count != 0)
                {
                    SendData("4\t" + MyFunction.CreateXML(gmp.charNO));
                    string xml = ReceiveData();
                    XElement root = XElement.Parse(xml);
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    foreach (XElement el in root.Elements())
                        dict.Add(el.Name.LocalName.Replace("CharNo", ""), el.Value);
                    foreach(string charNo in gmp.charNO)
                    {
                        GALOG_Logtxtbx.Text = GALOG_Logtxtbx.Text.Replace("#nCharNo#" + charNo, dict[charNo]);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please search first for a Character!", "EX3", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GALOG_Savebtn_Click(object sender, EventArgs e)
        {
            File.WriteAllText(Charactercmbx.Text + " - GameLog.txt", GALOG_Logtxtbx.Text);
            MessageBox.Show("Saved GameLog successfully.", "S1", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ChrUpdatebtn_Click(object sender, EventArgs e)
        {
            List<string> updateDetails = new List<string>();
            updateDetails.Add(CHRE_CharacterIDtxtbx.Text);
            updateDetails.Add(CHRE_Levelnmrcup.Value.ToString());
            updateDetails.Add(CHRE_Exptxtbx.Text);
            updateDetails.Add(CHRE_Moneytxtbx.Text);
            updateDetails.Add(CHRE_AdminLevelnmrup.Value.ToString());
            updateDetails.Add(CHRE_LoginZonetxtbx.Text);
            updateDetails.Add(CHRE_Fametxtbx.Text);
            updateDetails.Add(CHRE_Prisontxtbx.Text);
            updateDetails.Add(CHRE_Strengthtxtbx.Text);
            updateDetails.Add(CHRE_Constitutetxtbx.Text);
            updateDetails.Add(CHRE_Dexteritytxtbx.Text);
            updateDetails.Add(CHRE_Intelligencetxtbx.Text);
            updateDetails.Add(CHRE_MentalPowertxtbx.Text);
            updateDetails.Add(CHRE_Classcmbx.SelectedIndex.ToString());
            updateDetails.Add(CHRE_Gendercmbx.SelectedIndex.ToString());
            SendData("5\t" + Characternotxtbx.Text + "\t" + MyFunction.CreateXML(updateDetails));
            if (Convert.ToBoolean(ReceiveData()))
                MessageBox.Show("Successfully updating Character!", "SC1", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Error on updating Character!", "ER1", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MenuTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            CharacterTabControl.TabPages.Remove(tabPage1);
            CharacterTabControl.TabPages.Remove(tabPage2);
            CharacterTabControl.TabPages.Remove(tabPage4);
            CharacterTabControl.TabPages.Remove(tabPage5);
            switch(e.Node.Text)
            {
                case "Account":
                    CharacterTabControl.TabPages.Add(tabPage4);
                    CharacterTabControl.SelectedTab = tabPage4;
                    break;
                case "Character":
                    CharacterTabControl.TabPages.Add(tabPage1);
                    CharacterTabControl.TabPages.Add(tabPage2);
                    CharacterTabControl.TabPages.Add(tabPage5);
                    CharacterTabControl.SelectedTab = tabPage1;
                    break;
                case "Guild":
                    break;
            }
        }

        private void ACC_SearchBtn_Click(object sender, EventArgs e)
        {
            if (ACC_Searchtxtbx.Text.Length != 0)
            {
                SendData("6\t0\t" + ACC_Searchtxtbx.Text);
                string[] data = ReceiveData().Split('\t');
                ACC_SearchNotxtbx.Text = data[0];
                ACCI_AuthIDtxtbx.Text = data[1];
                ACCI_Mailtxtbx.Text = data[2];
                ACCI_CrtDateTimepick.Value = DateTime.Parse(data[3], new CultureInfo("en-US", true));
                SendData("7\t" + ACC_SearchNotxtbx.Text);
            }
            else if(ACC_SearchNotxtbx.Text.Length != 0)
            {
                SendData("6\t1\t" + ACC_SearchNotxtbx.Text);
                string[] data = ReceiveData().Split('\t');
                ACC_Searchtxtbx.Text = data[0];
                ACCI_AuthIDtxtbx.Text = data[1];
                ACCI_Mailtxtbx.Text = data[2];
                ACCI_CrtDateTimepick.Value = DateTime.Parse(data[3], new CultureInfo("en-US", true));
                SendData("7\t" + ACC_SearchNotxtbx.Text);
                string loginLog = ReceiveData();
                string multiAccount = ReceiveData();
                StringReader theReader = new StringReader(loginLog);
                DataSet theDataSet = new DataSet();
                theDataSet.ReadXml(theReader);
                ACCL_LoginLogdtgridview.DataSource = theDataSet.Tables[0];
                theReader = new StringReader(multiAccount);
                theDataSet.ReadXml(theReader);
                ACC_MultiplieAccountsdtgridview.DataSource = theDataSet.Tables[0];
            }
            else
            {
                MessageBox.Show("Please enter a valid Account ID or Account number!", "EX1", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ACCS_Actionbtn_Click(object sender, EventArgs e)
        {
            if (ACC_SearchNotxtbx.Text.Length != 0)
            {
                if (ACCS_Bannchkbx.Checked)
                {
                    if (ACCS_Timetxtbx.Text.Length != 0)
                    {
                        SendData("8\t" + ACC_SearchNotxtbx.Text + "\t" + ACCS_Timetxtbx.Text);
                        string rs = ReceiveData();
                        if (Convert.ToBoolean(rs))
                            MessageBox.Show("Successfully performed action!", "SU1", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show("Error on performed action!", "SU1", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                        MessageBox.Show("Please insert a length how long the account should be banned!" + Environment.NewLine + "FORMAT: 2015.05.20" + Environment.NewLine + "YYYY.MM.DD", "EX1", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (ACCS_BannMacchkbx.Checked)
                {
                    SendData("9\t" + ACC_SearchNotxtbx.Text + "\t1");

                }
                else if (ACCS_sendMailchkbx.Checked)
                {
                    SendData("10\t" + ACC_SearchNotxtbx.Text);
                }
                else if(ACCS_UnbannMacchkbx.Checked)
                {
                    //TODO: Add ERROR receive
                    SendData("9\t" + ACC_SearchNotxtbx.Text + "\t0");
                }
                else
                {
                    MessageBox.Show("Please check first one dialog box to perform a action!", "EX1", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                MessageBox.Show("Please search first for a Character!", "EX1", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void tabPage5_Enter(object sender, EventArgs e)
        {
            if(Characternotxtbx.Text.Length != 0)
            {
                SendData("11\t" + Characternotxtbx.Text);
                string receive = ReceiveData();
                StringReader theReader = new StringReader(receive);
                DataSet theDataSet = new DataSet();
                theDataSet.ReadXml(theReader);
                for (int i = 0; i < theDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow[] searchRow = Memory.itemInfoSHN.table.Select("ID = '" + theDataSet.Tables[0].Rows[i]["nItemID"] + "'");
                    if (searchRow.Count() != 0)
                        theDataSet.Tables[0].Rows[i]["nItemID"] = searchRow[0]["Name"];
                    else
                        theDataSet.Tables[0].Rows[i]["nItemID"] = theDataSet.Tables[0].Rows[i]["nItemID"];
                }
                CHI_InventoryDTGV.DataSource = theDataSet.Tables[0];
            }
            else
                MessageBox.Show("Please search first for a Character!", "EX1", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}