using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Project_Actis
{
    class Actis_SQL_Class
    {
        private MyFunction _myfunction = new MyFunction();
        private SqlConnection conn = new SqlConnection("user id=sa;password=xMfXE\\K{XpLV=K*o;server=.\\SQLEXPRESS;database=OdinAccounts;connection timeout=30");
        public string searchCharacter(string[] input)
        {
            SqlCommand cmd;
            string output = "";
            bool first = false;
            switch (Convert.ToInt16(input[1]))
            {
                case 0:
                    cmd = new SqlCommand("SELECT nCharNo, sID, nUserNo FROM w00_Character..tCharacter WHERE nUserNo = (SELECT nEMID FROM tAccounts WHERE sUsername = @sUsername) AND bDeleted = 0;", conn);
                    cmd.Parameters.AddWithValue("@sUsername", input[2]);
                    conn.Open();
                    SqlDataReader dtr = cmd.ExecuteReader();
                    if (!dtr.HasRows)
                    {
                        return "NoRecords";
                    }
                    while(dtr.Read())
                    {
                        if (first)
                            output += "\t";
                        output += dtr.GetInt32(0);
                        output += "\t" + dtr.GetString(1);
                        output += "\t" + dtr.GetInt32(2);
                        first = true;
                    }
                    break;
                case 1:
                    cmd = new SqlCommand("SELECT sUsername, nCharNo, sID FROM tAccounts, w00_Character..tCharacter WHERE tCharacter.nUserNo = @nUserNo AND nEMID = nUserNo AND bDeleted = 0;", conn);
                    cmd.Parameters.AddWithValue("@nUserNo", input[2]);
                    conn.Open();
                    dtr = cmd.ExecuteReader();
                    if (!dtr.HasRows)
                    {
                        return "NoRecords";
                    }
                    while(dtr.Read())
                    {
                        if (first)
                            output += "\t";
                        output += dtr.GetString(0);
                        output += "\t" + dtr.GetInt32(1);
                        output += "\t" + dtr.GetString(2);
                        first = true;
                    }
                    break;
                case 2:
                    cmd = new SqlCommand("SELECT nCharNo, nUserNo, sUsername FROM tAccounts, w00_Character..tCharacter WHERE sID = @sID AND nEMID = nUserNo;", conn);
                    cmd.Parameters.AddWithValue("@sID", input[2]);
                    conn.Open();
                    dtr = cmd.ExecuteReader();
                    if (!dtr.HasRows)
                    {
                        return "NoRecords";
                    }
                    while(dtr.Read())
                    {
                        if (first)
                            output += "\t";
                        output += dtr.GetInt32(0);
                        output += "\t" + dtr.GetInt32(1);
                        output += "\t" + dtr.GetString(2);
                        first = true;
                    }
                    break;
                case 3:
                    cmd = new SqlCommand("SELECT sID, nUserNo, sUsername FROM tAccounts, w00_Character..tCharacter WHERE nCharNo = @nCharNo AND nEMID = nUserNo", conn);
                    cmd.Parameters.AddWithValue("@nCharNo", input[2]);
                    conn.Open();
                    dtr = cmd.ExecuteReader();
                    if (!dtr.HasRows)
                    {
                        return "NoRecords";
                    }
                    while(dtr.Read())
                    {
                        if (first)
                            output += "\t";
                        output += dtr.GetString(0);
                        output += "\t" + dtr.GetInt32(1);
                        output += "\t" + dtr.GetString(2);
                        first = true;
                    }
                    break;
            }
            return output;
        }

        public string searchCharacterBysID(string sID)
        {
            SqlCommand cmd = new SqlCommand("SELECT sID, nAdminLevel, sLoginZone, nLevel, nExp, nMoney, nFame, nPrisonMin, nPlayMin, nStrength, nConstitute, nDexterity, nIntelligence, nMentalPower, nClass, nGender, tAccounts.sUsername FROM w00_Character..tCharacter, w00_Character..tCharacterShape, tAccounts WHERE sID = @sID AND nEMID = nUserNo AND tCharacter.nCharNo = tCharacterShape.nCharNo;", conn);
            cmd.Parameters.AddWithValue("@sID", sID);
            conn.Open();
            SqlDataReader dtr = cmd.ExecuteReader();
            while (dtr.Read())
            {
                if (!dtr.HasRows)
                {
                    return "NoRecords";
                }
                return dtr.GetString(0) + "\t" + dtr.GetByte(1) + "\t" + dtr.GetString(2) + "\t" + dtr.GetInt16(3) + "\t" + dtr.GetInt64(4) + "\t" + dtr.GetInt64(5) + "\t" + dtr.GetInt32(6) + "\t" + dtr.GetInt16(7) + "\t" + dtr.GetInt32(8) + "\t" + dtr.GetByte(9) + "\t" + dtr.GetByte(10) + "\t" + dtr.GetByte(11) + "\t" + dtr.GetByte(12) + "\t" + dtr.GetByte(13) + "\t" + dtr.GetByte(14) + "\t" + dtr.GetByte(15) + "\t" + dtr.GetString(16);
            }
            return "NoRecords";
        }

        public string gameLogFromsID(string sID)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM w00_GameLog..tGameLog WHERE tGameLog.nCharNo = (SELECT nCharNo FROM w00_Character..tCharacter WHERE sID = @sID) OR tGameLog.nTargetCharNo = (SELECT nCharNo FROM w00_Character..tCharacter WHERE sID = @sID) ORDER BY dDate;", conn);
            cmd.Parameters.AddWithValue("@sID", sID);
            DataTable bufferDT = new DataTable();
            SqlDataAdapter SQLDTA = new SqlDataAdapter();
            conn.Open();
            bufferDT.TableName = "GameLog";
            SQLDTA.SelectCommand = cmd;
            SQLDTA.Fill(bufferDT);
            conn.Close();
            StringWriter sw = new StringWriter();
            bufferDT.WriteXml(sw);
            sw.Close();
            return sw.ToString();
        }

        public string inventoryFromsID(string sID)
        {
            SqlCommand cmd = new SqlCommand("SELECT nItemID, tItem.nItemKey, nOptionData, nOptionType FROM w00_Character..tItem, w00_Character..tItemOptions WHERE tItem.nOwner IN (SELECT nCharNo FROM w00_Character..tCharacter WHERE sID = @sID) AND tItem.nItemKey = tItemOptions.nItemKey;", conn);
            cmd.Parameters.AddWithValue("@sID", sID);
            DataTable bufferDT = new DataTable();
            SqlDataAdapter SQLDTA = new SqlDataAdapter();
            conn.Open();
            bufferDT.TableName = "Inventory";
            SQLDTA.SelectCommand = cmd;
            SQLDTA.Fill(bufferDT);
            conn.Close();
            StringWriter sw = new StringWriter();
            bufferDT.WriteXml(sw);
            sw.Close();
            return sw.ToString();
        }

        public string getCharIDFromNo(string inputstring)
        {
            Dictionary<string, string> charNoRT = new Dictionary<string, string>();
            string[] arr = XDocument.Parse(inputstring).Descendants("string").Select(element => element.Value).ToArray();
            foreach (string nCharNo in arr)
            {
                SqlCommand cmd = new SqlCommand("SELECT sID FROM w00_Character..tCharacter WHERE nCharNo = @nCharNo;", conn);
                cmd.Parameters.AddWithValue("@nCharNo", nCharNo);
                conn.Open();
                SqlDataReader dtr = cmd.ExecuteReader();
                while (dtr.Read())
                {
                    if (!dtr.HasRows)
                    {
                        continue;
                    }
                    charNoRT.Add(nCharNo, dtr.GetString(0));
                }
                conn.Close();
            }
            XElement root = new XElement("Root", from keyValue in charNoRT select new XElement("CharNo" + keyValue.Key, keyValue.Value));
            return root.ToString();
        }

        public bool updateCharacter(string StringInput, Int32 charNo)
        {
            string[] arr = XDocument.Parse(StringInput).Descendants("string").Select(element => element.Value).ToArray();
            SqlCommand cmd = new SqlCommand("UPDATE w00_Character..tCharacter SET sID = @sID, nAdminLevel = @nAdminLevel, sLoginZone = @sLoginZone, nLevel = @nLevel, nExp = @nExp, nMoney = @nMoney, nFame = @nFame, nPrisonMin = @nPrisonMin, nStrength = @nStrength, nConstitute = @nConstitute, nDexterity = @nDexterity, nIntelligence = @nIntelligence, nMentalPower = @nMentalPower WHERE nCharNo = @nCharNo;", conn);
            cmd.Parameters.AddWithValue("@sID", arr[0]);
            cmd.Parameters.AddWithValue("@nLevel", arr[1]);
            cmd.Parameters.AddWithValue("@nExp", arr[2]);
            cmd.Parameters.AddWithValue("@nMoney", arr[3]);
            cmd.Parameters.AddWithValue("@nAdminLevel", arr[4]);
            cmd.Parameters.AddWithValue("@sLoginZone", arr[5]);
            cmd.Parameters.AddWithValue("@nFame", arr[6]);
            cmd.Parameters.AddWithValue("@nPrisonMin", arr[7]);
            cmd.Parameters.AddWithValue("@nStrength", arr[8]);
            cmd.Parameters.AddWithValue("@nConstitute", arr[9]);
            cmd.Parameters.AddWithValue("@nDexterity", arr[10]);
            cmd.Parameters.AddWithValue("@nIntelligence", arr[11]);
            cmd.Parameters.AddWithValue("@nMentalPower", arr[12]);
            cmd.Parameters.AddWithValue("@nCharNo", charNo);
            conn.Open();
            int RETURN = cmd.ExecuteNonQuery();
            conn.Close();
            if(Convert.ToBoolean(RETURN))
            {
                cmd = new SqlCommand("UPDATE w00_Character..tCharacterShape SET nClass = @nClass, nGender = @nGender WHERE nCharNo = @nCharNo;", conn);
                cmd.Parameters.AddWithValue("@nClass", arr[13]);
                cmd.Parameters.AddWithValue("@nGender", arr[14]);
                cmd.Parameters.AddWithValue("@nCharNo", charNo);
                conn.Open();
                RETURN = cmd.ExecuteNonQuery();
                conn.Close();
            }
            return Convert.ToBoolean(RETURN);
        }

        public string selectAccount(Int16 function, string AccountID)
        {
            SqlCommand cmd;
            string buffer = "";
            if (function == 0)
            {
                cmd = new SqlCommand("SELECT nEMID, nAuthID, sEmail, dDate FROM tAccounts WHERE sUsername = @sUsername;", conn);
                cmd.Parameters.AddWithValue("@sUsername", AccountID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    buffer = Convert.ToString(reader.GetInt32(0)) + "\t" + reader.GetInt32(1) + "\t" + reader.GetString(2) + "\t" + reader.GetDateTime(3);
                }
                conn.Close();
            }
            else if(function == 1)
            {
                cmd = new SqlCommand("SELECT sUsername, nAuthID, sEmail, dDate FROM tAccounts WHERE nEMID = @nEMID;", conn);
                cmd.Parameters.AddWithValue("@nEMID", AccountID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    buffer = reader.GetString(0) + "\t" + reader.GetInt32(1) + "\t" + reader.GetString(2) + "\t" + reader.GetDateTime(3);
                }
                conn.Close();
            }
            return buffer;
        }
        
        public string selectAccountLoginLog(Int32 nEMID)
        {
            SqlCommand cmd = new SqlCommand("SELECT sIP, dDate FROM AccountLog..tAccountLoginLog WHERE nEMID = @nEMID ORDER BY dDate DESC;", conn);
            cmd.Parameters.AddWithValue("@nEMID", nEMID);
            DataTable bufferDT = new DataTable();
            SqlDataAdapter SQLDTA = new SqlDataAdapter();
            conn.Open();
            bufferDT.TableName = "LoginLog";
            SQLDTA.SelectCommand = cmd;
            SQLDTA.Fill(bufferDT);
            conn.Close();
            StringWriter sw = new StringWriter();
            bufferDT.WriteXml(sw);
            sw.Close();
            return sw.ToString();
        }

        public string selectAccountMulti(Int32 nEMID)
        {
            SqlCommand cmd = new SqlCommand("SELECT nEMID FROM AccountLog..tAccountLoginLog WHERE sIP IN (SELECT sIP FROM AccountLog..tAccountLoginLog WHERE nEMID = @nEMID) ORDER BY dDate DESC;", conn);
            cmd.Parameters.AddWithValue("@nEMID", nEMID);
            DataTable bufferDT = new DataTable();
            SqlDataAdapter SQLDTA = new SqlDataAdapter();
            conn.Open();
            bufferDT.TableName = "Multi";
            SQLDTA.SelectCommand = cmd;
            SQLDTA.Fill(bufferDT);
            conn.Close();
            StringWriter sw = new StringWriter();
            bufferDT.WriteXml(sw);
            sw.Close();
            return sw.ToString();
        }

        public bool bannAccount(string[] data, string StaffMember)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO tBans (nAGID, sCategory, sViolation, sStartDate, sEndDate, sStaff, sNote) VALUES (@nAGID, 'GMToolkit', '-', GETDATE(), @sEndDate, @sStaff, '-');", conn);
            cmd.Parameters.AddWithValue("@nAGID", data[1]);
            cmd.Parameters.AddWithValue("@sEndDate", data[2]);
            cmd.Parameters.AddWithValue("@sStaff", StaffMember);
            conn.Open();
            int RETURN = cmd.ExecuteNonQuery();
            conn.Close();
            return Convert.ToBoolean(RETURN);
        }

        public bool bannMacAccount(string[] data)
        {
            SqlCommand cmd = new SqlCommand("UPDATE NANOFS..tBlock SET nBlocked = @nBlocked WHERE sIP IN (SELECT sIP FROM AccountLog..tAccountLoginLog WHERE nEMID = @nEMID)", conn);
            cmd.Parameters.AddWithValue("@nEMID", data[1]);
            cmd.Parameters.AddWithValue("@nBlocked", data[2]);
            conn.Open();
            int RETURN = cmd.ExecuteNonQuery();
            conn.Close();
            return Convert.ToBoolean(RETURN);
        }

        public bool sendNewPassword(Int32 nEMID)
        {
            SqlCommand cmd = new SqlCommand("SELECT sEmail, sUsername FROM tAccounts WHERE nEMID = @nEMID", conn);
            cmd.Parameters.AddWithValue("@nEMID", nEMID);
            string[] data = new string[2];
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data[0] = reader.GetString(0);
                data[1] = reader.GetString(1);
            }
            string newPassword = _myfunction.RandomString(12);
            cmd = new SqlCommand("UPDATE tAccounts SET sUserPass = @sUserPass WHERE nEMID = @nEMID;", conn);
            cmd.Parameters.AddWithValue("@sUserPass", _myfunction.GetMD5Hash(newPassword).ToLower());
            cmd.Parameters.AddWithValue("@nEMID", nEMID);
            cmd.ExecuteNonQuery();
            // Command line argument must the the SMTP host.
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.Host = "mail.kernstudios.org";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("support@kernstudios.org", "Lomotu12");

            MailMessage mm = new MailMessage("support@kernstudios.org", data[0], "New password for your KeRnStudios Account", "Hello " + data[1] + Environment.NewLine + "you requested for your account a new Password. " + Environment.NewLine + Environment.NewLine + "Your new password is: " + newPassword + Environment.NewLine + Environment.NewLine + "regards, KeRn Staff Team");
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);
            return true;
        }

        public string selectCharacterInventory(Int32 charNo)
        {
            SqlCommand cmd = new SqlCommand("SELECT nStorageType, nStorage, nItemID, dDate, nOptionType, nOptionData FROM w00_Character..tItem, w00_Character..tItemOptions WHERE nOwner = @nOwner AND (nStorageType = 8 OR nStorageType = 9) AND tItem.nItemKey = tItemOptions.nItemKey ORDER BY nStorageType ASC;", conn);
            cmd.Parameters.AddWithValue("@nOwner", charNo);
            DataTable bufferDT = new DataTable();
            SqlDataAdapter SQLDTA = new SqlDataAdapter();
            conn.Open();
            bufferDT.TableName = "Inventory";
            SQLDTA.SelectCommand = cmd;
            SQLDTA.Fill(bufferDT);
            conn.Close();
            StringWriter sw = new StringWriter();
            bufferDT.WriteXml(sw);
            sw.Close();
            return sw.ToString();
        }
    }
}
