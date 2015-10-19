using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Actis_Client
{
    class GameLog_Parser
    {
        public List<string> charNO = new List<string>();
        public List<string> Main(DataTable dt, DataTable nItem)
        {
            SHNFile nSHNItem = new SHNFile("9Data/ItemInfo.shn");
            SHNFile nActiveSkill = new SHNFile("9Data/ActiveSkill.shn");
            List<string> output = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                switch (Convert.ToInt32(row["nType"]))
                {
                    case 0:
                        output.Add("UNKNOWN");
                        break;
                    case 1:
                        output.Add(row["dDate"] + "\t" + "ADD xy");
                        break;
                    case 2:
                        output.Add(row["dDate"] + "\t" + "ADD_REQ");
                        break;
                    case 3:
                        output.Add(row["dDate"] + "\t" + "ADD_ACK");
                        break;
                    case 10:
                        output.Add(row["dDate"] + "\t" + "LOGON " + row["sMap"] + " x" + row["nMapX"] + " y" + row["nMapY"]);
                        break;
                    case 11:
                        output.Add(row["dDate"] + "\t" + "LOGOFF " + row["sMap"] + " x" + row["nMapX"] + " y" + row["nMapY"]);
                        break;
                    case 15:
                        output.Add(row["dDate"] + "\t" + "LINK " + row["sMap"] + " x" + row["nMapX"] + " y" + row["nMapY"]);
                        break;
                    case 16:
                        break;
                    //output.Add("ZONE_LOGOFF " & row["sMap") & " " & row["nMapX") & " " & row["nMapY")
                    // Disbaled because of UNKNOWN Spam :D
                    case 17:
                        output.Add(row["dDate"] + "\t" + "CREATE_AVATAR");
                        break;
                    case 18:
                        output.Add(row["dDate"] + "\t" + "DELETE_AVATAR");
                        break;
                    case 20:
                        output.Add(row["dDate"] + "\t" + "HIT");
                        break;
                    case 21:
                        output.Add(row["dDate"] + "\t" + "MOVE");
                        break;
                    case 25:
                        output.Add(row["dDate"] + "\t" + "PRISON");
                        break;
                    case 26:
                        output.Add(row["dDate"] + "\t" + "PRISON_RELEASE");
                        break;
                    case 30:
                        output.Add(row["dDate"] + "\t" + "LEVEL_UP " + row["sMap"] + " " + row["nMapX"] + " " + row["nMapY"] + " Level = " + row["nInt1"]);
                        break;
                    case 31:
                        output.Add(row["dDate"] + "\t" + "LEVEL_DOWN " + row["sMap"] + " " + row["nMapX"] + " " + row["nMapY"] + " Level = " + row["nInt1"]);
                        break;
                    case 32:
                        //####### Need testing
                        output.Add(row["dDate"] + "\t" + "CHANGE_CLASS ");
                        break;
                    case 40:
                        break;
                    //Unknown
                    case 41:
                        if (!charNO.Contains(row["nTargetCharNo"]))
                            charNO.Add(Convert.ToString(row["nTargetCharNo"]));
                        output.Add(row["dDate"] + "\t" + "PKED #nCharNo#" + row["nTargetCharNo"] + " on Map: " + row["sMap"]);
                        break;
                    case 42:
                        break;
                    case 43:
                        break;
                    case 44:
                        break;
                    case 45:
                        break;
                    case 50:
                        try
                        {
                            DataRow[] row1 = nActiveSkill.table.Select("ID = " + row["nInt1"]);
                            output.Add(row["dDate"] + "\t" + "SKILL_LEARN SKILL = " + row1[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 51:
                        try
                        {
                            DataRow[] row1 = nActiveSkill.table.Select("ID = " + row["nInt1"]);
                            output.Add(row["dDate"] + "\t" + "SKILL_DELETE SKILL = " + row1[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 52:
                        try
                        {
                            DataRow[] row1 = nActiveSkill.table.Select("ID = " + row["nInt1"]);
                            output.Add(row["dDate"] + "\t" + "SKILL_USE SKILL = " + row1[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 55:
                        output.Add(row["dDate"] + "\t" + "STATE_SET = " + row["nInt1"]);
                        break;
                    case 56:
                        output.Add(row["dDate"] + "\t" + "STATE_CLEAR = " + row["nInt1"]);
                        break;
                    case 57:
                        try
                        {
                            DataRow[] row1 = nItem.Select("nItemKey = " + row["nItemKey"]);
                            DataRow[] row2 = nSHNItem.table.Select("ID = " + row1[0]["nItemID"]);
                            output.Add(row["dDate"] + "\t" + "CHARGED_BUFF_SET " + row2[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 58:
                        try
                        {
                            DataRow[] row1 = nItem.Select("nItemKey = " + row["nItemKey"]);
                            DataRow[] row2 = nSHNItem.table.Select("ID = " + row1[0]["nItemID"]);
                            output.Add(row["dDate"] + "\t" + "CHARGED_BUFF_CLR " + row2[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 60:
                        output.Add(row["dDate"] + "\t" + "QUEST_GET QuestNo = " + row["nInt1"] + " QuestProgress = " + row["nInt2"]);
                        break;
                    case 61:
                        output.Add(row["dDate"] + "\t" + "QUEST_DONE QuestNo = " + row["nInt1"] + " QuestProgress = " + row["nInt2"]);
                        break;
                    case 62:
                        output.Add(row["dDate"] + "\t" + "QUEST_PROGRESS QuestNo = " + row["nInt1"] + " QuestProgress = " + row["nInt2"]);
                        break;
                    case 63:
                        output.Add(row["dDate"] + "\t" + "QUEST_ITEM_GET QuestNo = " + row["nInt1"] + " QuestProgress = " + row["nInt2"]);
                        break;
                    case 64:
                        output.Add(row["dDate"] + "\t" + "QUEST_GIVEUP QuestNo = " + row["nInt1"]);
                        break;
                    case 65:

                        break;
                    case 66:
                        break;
                    case 69:
                        try
                        {
                            DataRow[] row1 = nItem.Select("nItemKey = " + row["nItemKey"]);
                            DataRow[] row2 = nSHNItem.table.Select("ID = " + row1[0]["nItemID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_BREAK Name: " + row2[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 70:
                        try
                        {
                            DataRow[] row1 = nItem.Select("nItemKey = " + row["nItemKey"]);
                            DataRow[] row2 = nSHNItem.table.Select("ID = " + row1[0]["nItemID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_TAKE Name: " + row2[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 71:
                        try
                        {
                            DataRow[] row1 = nItem.Select("nItemKey = " + row["nItemKey"]);
                            DataRow[] row2 = nSHNItem.table.Select("ID = " + row1[0]["nItemID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_DROP Name: " + row2[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 72:
                        try
                        {
                            DataRow[] row1 = nItem.Select("nItemKey = " + row["nItemKey"]);
                            DataRow[] row2 = nSHNItem.table.Select("ID = " + row1[0]["nItemID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_BUY Name: " + row2[0]["Name"] + " COUNT = " + row["nInt1"] + " BUY PRICE = " + row["nInt2"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 73:
                        try
                        {
                            DataRow[] row1 = nItem.Select("nItemKey = " + row["nItemKey"]);
                            DataRow[] row2 = nSHNItem.table.Select("ID = " + row1[0]["nItemID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_SELL Name: " + row2[0]["Name"] + " COUNT = " + row["nInt1"] + " SELL PRICE = " + row["nInt2"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 74:
                        try
                        {
                            DataRow[] row1 = nSHNItem.table.Select("ID = " + row["nTargetID"]);
                            if (!charNO.Contains(row["nCharNo"]))
                                charNO.Add(Convert.ToString(row["nCharNo"]));
                            if (!charNO.Contains(row["nTargetCharNo"]))
                                 charNO.Add(Convert.ToString(row["nTargetCharNo"]));
                            output.Add(row["dDate"] + "\t" + "ITEM_TRADE Name: " + row1[0]["Name"] + " From Character = #nCharNo#" + row["nCharNo"] + " To Character = #nCharNo#" + row["nTargetCharNo"] + " Count: " + row["nInt1"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 75:
                        try
                        {
                            DataRow[] row1 = nItem.Select("nItemKey = " + row["nItemKey"]);
                            DataRow[] row2 = nSHNItem.table.Select("ID = " + row1[0]["nItemID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_USE Name: " + row2[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 76:
                        try
                        {
                            DataRow[] row1 = nItem.Select("nItemKey = " + row["nItemKey"]);
                            DataRow[] row2 = nSHNItem.table.Select("ID = " + row1[0]["nItemID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_INVEN_MOVE Name: " + row2[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 77:
                        try
                        {
                            DataRow[] row1 = nItem.Select("nItemKey = " + row["nItemKey"]);
                            DataRow[] row2 = nSHNItem.table.Select("ID = " + row1[0]["nItemID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_EQUIP Name: " + row2[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 78:
                        try
                        {
                            DataRow[] row1 = nItem.Select("nItemKey = " + row["nItemKey"]);
                            DataRow[] row2 = nSHNItem.table.Select("ID = " + row1[0]["nItemID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_UNEQUIP Name: " + row2[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 79:
                        try
                        {
                            DataRow[] row1 = nItem.Select("nItemKey = " + row["nItemKey"]);
                            DataRow[] row2 = nSHNItem.table.Select("ID = " + row1[0]["nItemID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_CREATE Name: " + row2[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 80:
                        try
                        {
                            DataRow[] row1 = nItem.Select("nItemKey = " + row["nItemKey"]);
                            DataRow[] row2 = nSHNItem.table.Select("ID = " + row1[0]["nItemID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_STORE_IN Name: " + row2[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 81:
                        try
                        {
                            DataRow[] row1 = nItem.Select("nItemKey = " + row["nItemKey"]);
                            DataRow[] row2 = nSHNItem.table.Select("ID = " + row1[0]["nItemID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_STORE_OUT Name: " + row2[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 82:
                        try
                        {
                            DataRow[] row1 = nItem.Select("nItemKey = " + row["nItemKey"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_UPGRADE Name: " + row1[0]["Name"] + " FROM +" + row["nInt1"] + " TO +" + row["nInt2"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 84:
                        break;
                    case 85:
                        break;
                    case 86:
                        try
                        {
                            DataRow[] row1 = nSHNItem.table.Select("ID = " + row["nTargetID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_MERGE Name: " + row1[0]["Name"] + " OLD nItemKey = " + row["nBigint1"] + " NEW nItemKey = " + row["nItemKey"] + " FROM " + row["nInt1"] + " TO " + row["nInt2"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 87:
                        try
                        {
                            DataRow[] row1 = nSHNItem.table.Select("ID = " + row["nTargetID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_SPLITT Name: " + row1[0]["Name"] + " Item1 nItemKey = " + row["nBigint1"] + " COUNT: " + row["nInt1"] + "Item2 nItemKey = " + row["nItemKey"] + " COUNT: " + row["nInt2"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 88:
                        break;
                    case 89:
                        break;
                    case 90:
                        break;
                    case 91:
                        try
                        {
                            DataRow[] row1 = nSHNItem.table.Select("ID = " + row["nTargetID"]);
                            if (!charNO.Contains(row["nTargetCharNo"]))
                                charNO.Add(Convert.ToString(row["nTargetCharNo"]));
                            output.Add(row["dDate"] + "\t" + "ITEM_BOOTH_BUY Name: " + row1[0]["Name"] + " FROM Character = #nCharNo#" + row["nTargetCharNo"] + " Sell Price = " + row["nInt2"] + " Count = " + row["nInt1"] + " nItemKey = " + row["nItemKey"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 92:
                        try
                        {
                            DataRow[] row1 = nSHNItem.table.Select("ID = " + row["nTargetID"]);
                            if (!charNO.Contains(row["nTargetCharNo"]))
                                charNO.Add(Convert.ToString(row["nTargetCharNo"]));
                            output.Add(row["dDate"] + "\t" + "ITEM_BOOTH_SELL Name: " + row1[0]["Name"] + " Target Character = #nCharNo#" + row["nTargetCharNo"] + " Sell Price = " + row["nInt2"] + " Count = " + row["nInt1"] + " nItemKey = " + row["nItemKey"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 83:
                        break;
                    case 93:
                        break;
                    case 94:
                        output.Add(row["dDate"] + "\t" + "MONEY_DEPOSIT Amount = " + row["nInt1"]);
                        break;
                    case 95:
                        output.Add(row["dDate"] + "\t" + "MONEY_WITHDRAW Amount = " + row["nInt1"]);
                        break;
                    case 96:
                        if (!charNO.Contains(row["nInt1"]))
                            charNO.Add(Convert.ToString(row["nInt1"]));
                        output.Add(row["dDate"] + "\t" + "MONEY_TRADE_INCOME Amount = " + row["nBigint1"] + " FROM Character = #nCharNo#" + row["nInt1"]);
                        break;
                    case 97:
                        if (!charNO.Contains(row["nInt1"]))
                            charNO.Add(Convert.ToString(row["nInt1"]));
                        output.Add(row["dDate"] + "\t" + "MONEY_TRADE_OUTGO Amount = " + row["nBigint1"] + " TO Character = #nCharNo#" + row["nInt1"]);
                        break;
                    case 98:
                        break;
                    case 100:
                        break;
                    case 101:
                        break;
                    case 110:
                        break;
                    case 111:
                        break;
                    case 112:
                        break;
                    case 113:
                        break;
                    case 114:
                        break;
                    case 115:
                        break;
                    case 120:
                        break;
                    case 121:
                        break;
                    case 130:
                        break;
                    case 131:
                        break;
                    case 140:
                        if (!charNO.Contains(row["nCharNo"]))
                            charNO.Add(Convert.ToString(row["nCharNo"]));
                        output.Add(row["dDate"] + "\t" + "GUILD_CREATE = " + row["sMap"] + " by Character #nCharNo#" + row["nCharNo"]);
                        break;
                    case 141:
                        break;
                    case 142:
                        break;
                    case 143:
                        break;
                    case 144:
                        break;
                    case 145:
                        break;
                    case 146:
                        break;
                    case 160:
                        break;
                    case 161:
                        break;
                    case 162:
                        break;
                    case 163:
                        break;
                    case 170:
                        break;
                    case 171:
                        break;
                    case 172:
                        break;
                    case 173:
                        break;
                    case 174:
                        break;
                    case 190:
                        output.Add(row["dDate"] + "\t" + "CHARGE_WITHDRAW = " + row["nInt2"]);
                        break;
                    case 200:
                        break;
                    case 250:
                        break;
                    case 270:
                        break;
                    case 280:
                        break;
                    case 281:
                        break;
                    case 282:
                        try
                        {
                            DataRow[] row1 = nItem.Select("nItemKey = " + row["nItemKey"]);
                            DataRow[] row2 = nSHNItem.table.Select("ID = " + row1[0]["nItemID"]);
                            if (row2[0]["ID"] == row1[0]["nItemID"])
                            {
                                int Count = 0;
                                if (Convert.ToInt32(row1[0]["nOptionData"]) == 0)
                                    Count = 1;
                                output.Add(row["dDate"] + "\t" + "QREWARD_ITEM Name: " + row2[0]["Name"] + " Count = " + Count);
                            }
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 283:
                        break;
                    case 284:
                        break;
                    case 285:
                        break;
                    case 286:
                        break;
                    case 287:
                        break;
                    case 300:
                        break;
                    case 310:
                        break;
                    case 320:
                        break;
                    case 330:
                        break;
                    case 400:
                        break;
                    case 490:
                        break;
                    case 500:
                        break;
                    case 501:
                        break;
                    case 502:
                        break;
                    case 510:
                        try
                        {
                            DataRow[] row1 = nSHNItem.table.Select("ID = " + row["nTargetID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_TAKE_INVEN_EXT Name: " + row1[0]["Name"] + " Count: " + row["nInt1"] + " nItemKey = " + row["nItemKey"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 511:
                        try
                        {
                            DataRow[] row1 = nSHNItem.table.Select("ID = " + row["nTargetID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_DROP_INVEN_EXT Name: " + row1[0]["Name"] + " Count: " + row["nInt1"] + " nItemKey = " + row["nItemKey"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 512:
                        try
                        {
                            DataRow[] row1 = nSHNItem.table.Select("ID = " + row["nTargetID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_BUY_INVEN_EXT Name: " + row1[0]["Name"] + " Count: " + row["nInt1"] + " nItemKey = " + row["nItemKey"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 513:
                        try
                        {
                            DataRow[] row1 = nSHNItem.table.Select("ID = " + row["nTargetID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_SELL_INVEN_EXT Name: " + row1[0]["Name"] + " Count: " + row["nInt1"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 514:
                        try
                        {
                            DataRow[] row1 = nSHNItem.table.Select("ID = " + row["nTargetID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_USE_INVEN_EXT Name: " + row1[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 515:
                        try
                        {
                            DataRow[] row1 = nItem.Select("nItemKey = " + row["nItemKey"]);
                            DataRow[] row2 = nSHNItem.table.Select("ID = " + row1[0]["nItemID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_INVEN_MOVE_INVEN_EXT Name: " + row2[0]["Name"] + " ItemKey: " + row["nItemKey"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 516:
                        try
                        {
                            DataRow[] row1 = nSHNItem.table.Select("ID = " + row["nTargetID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_EQUIP_INVEN_EXT Name: " + row1[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 517:
                        try
                        {
                            DataRow[] row1 = nSHNItem.table.Select("ID = " + row["nTargetID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_UNEQUIP_INVEN_EXT Name: " + row1[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 518:
                        try
                        {
                            DataRow[] row1 = nSHNItem.table.Select("ID = " + row["nTargetID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_CREATE_INVEN_EXT Name: " + row1[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 519:
                        try
                        {
                            DataRow[] row1 = nSHNItem.table.Select("ID = " + row["nTargetID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_STORE_IN_INVEN_EXT Name: " + row1[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 520:
                        try
                        {
                            DataRow[] row1 = nSHNItem.table.Select("ID = " + row["nTargetID"]);
                            output.Add(row["dDate"] + "\t" + "ITEM_STORE_OUT_INVEN_EXT Name: " + row1[0]["Name"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 600:
                        if (!charNO.Contains(row["nCharNo"]))
                            charNO.Add(Convert.ToString(row["nCharNo"]));
                        if (!charNO.Contains(row["nTargetCharNo"]))
                            charNO.Add(Convert.ToString(row["nTargetCharNo"]));
                        output.Add(row["dDate"] + "\t" + "PROPOSE ACEEPT FROM #nCharNo#" + row["nCharNo"] + " TO #nCharNo#" + row["nTargetCharNo"]);
                        break;
                    case 601:
                        if (!charNO.Contains(row["nCharNo"]))
                            charNO.Add(Convert.ToString(row["nCharNo"]));
                        if (!charNO.Contains(row["nTargetCharNo"]))
                            charNO.Add(Convert.ToString(row["nTargetCharNo"]));
                        output.Add(row["dDate"] + "\t" + "PROPOSE END FROM #nCharNo#" + row["nCharNo"] + " TO #nCharNo#" + row["nTargetCharNo"]);
                        break;
                    case 602:
                        if (!charNO.Contains(row["nCharNo"]))
                            charNO.Add(Convert.ToString(row["nCharNo"]));
                        if (!charNO.Contains(row["nTargetCharNo"]))
                            charNO.Add(Convert.ToString(row["nTargetCharNo"]));
                        output.Add(row["dDate"] + "\t" + "DIVORCE REQUEST FROM #nCharNo#" + row["nCharNo"] + " TO #nCharNo#" + row["nTargetCharNo"]);
                        break;
                    case 603:
                        if (!charNO.Contains(row["nCharNo"]))
                            charNO.Add(Convert.ToString(row["nCharNo"]));
                        if (!charNO.Contains(row["nTargetCharNo"]))
                            charNO.Add(Convert.ToString(row["nTargetCharNo"]));
                        output.Add(row["dDate"] + "\t" + "DIVORCE EXECUTE FROM #nCharNo#" + row["nCharNo"] + " TO #nCharNo#" + row["nTargetCharNo"]);
                        break;
                    case 604:
                        if (!charNO.Contains(row["nCharNo"]))
                            charNO.Add(Convert.ToString(row["nCharNo"]));
                        if (!charNO.Contains(row["nTargetCharNo"]))
                            charNO.Add(Convert.ToString(row["nTargetCharNo"]));
                        output.Add(row["dDate"] + "\t" + "DIVORCE CANCEL FROM #nCharNo#" + row["nCharNo"] + " TO #nCharNo#" + row["nTargetCharNo"]);
                        break;
                    case 605:
                        if (!charNO.Contains(row["nCharNo"]))
                            charNO.Add(Convert.ToString(row["nCharNo"]));
                        if (!charNO.Contains(row["nTargetCharNo"]))
                            charNO.Add(Convert.ToString(row["nTargetCharNo"]));
                        output.Add(row["dDate"] + "\t" + "WEDDING HALL RESERVE FROM #nCharNo#" + row["nCharNo"] + " TO #nCharNo#" + row["nTargetCharNo"]);
                        break;
                    case 606:
                        if (!charNO.Contains(row["nCharNo"]))
                            charNO.Add(Convert.ToString(row["nCharNo"]));
                        if (!charNO.Contains(row["nTargetCharNo"]))
                            charNO.Add(Convert.ToString(row["nTargetCharNo"]));
                        output.Add(row["dDate"] + "\t" + "WEDDING HALL START FROM #nCharNo#" + row["nCharNo"] + " TO #nCharNo#" + row["nTargetCharNo"]);
                        break;
                    case 607:
                        if (!charNO.Contains(row["nCharNo"]))
                            charNO.Add(Convert.ToString(row["nCharNo"]));
                        if (!charNO.Contains(row["nTargetCharNo"]))
                            charNO.Add(Convert.ToString(row["nTargetCharNo"]));
                        output.Add(row["dDate"] + "\t" + "WEDDING HALL CANCEL FROM #nCharNo#" + row["nCharNo"] + " TO #nCharNo#" + row["nTargetCharNo"]);
                        break;
                    case 608:
                        if (!charNO.Contains(row["nCharNo"]))
                            charNO.Add(Convert.ToString(row["nCharNo"]));
                        if (!charNO.Contains(row["nTargetCharNo"]))
                            charNO.Add(Convert.ToString(row["nTargetCharNo"]));
                        output.Add(row["dDate"] + "\t" + "MARRIED FROM #nCharNo#" + row["nCharNo"] + " TO #nCharNo#" + row["nTargetCharNo"]);
                        break;
                    case 910:
                        break;
                    case 911:
                        break;
                    case 912:
                        break;
                    case 913:
                        break;
                    case 920:
                        break;
                    case 1000:
                        break;
                    case 1001:
                        break;
                    case 1002:
                        break;
                    case 1003:
                        break;
                    case 1010:
                        break;
                    case 1011:
                        break;
                    case 1015:
                        break;
                    case 1016:
                        break;
                    case 1017:
                        break;
                    case 1018:
                        break;
                    case 1020:
                        break;
                    case 1021:
                        break;
                    case 1025:
                        break;
                    case 1026:
                        break;
                    case 1030:
                        break;
                    case 1031:
                        break;
                    case 1032:
                        break;
                    case 1040:
                        break;
                    case 1041:
                        break;
                    case 1042:
                        break;
                    case 1043:
                        break;
                    case 1044:
                        break;
                    case 1045:
                        break;
                    case 1050:
                        break;
                    case 1051:
                        break;
                    case 1052:
                        break;
                    case 1055:
                        break;
                    case 1056:
                        break;
                    case 1057:
                        break;
                    case 1058:
                        break;
                    case 1060:
                        break;
                    case 1061:
                        break;
                    case 1062:
                        break;
                    case 1063:
                        break;
                    case 1064:
                        break;
                    case 1065:
                        break;
                    case 1066:
                        break;
                    case 1069:
                        break;
                    case 1070:
                        break;
                    case 1071:
                        break;
                    case 1072:
                        break;
                    case 1073:
                        break;
                    case 1074:
                        try
                        {
                            DataRow[] row1 = nSHNItem.table.Select("ID = " + row["nTargetID"]);
                            if (!charNO.Contains(row["nTargetCharNo"]))
                                charNO.Add(Convert.ToString(row["nTargetCharNo"]));
                            output.Add(row["dDate"] + "\t" + "ITEM_TRADE Name: " + row1[0]["Name"] + " Target Character = #nCharNo#" + row["nTargetCharNo"] + " Count: " + row["nInt1"] + " " + row["nItemKey"]);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("ClientDebug/Exception.txt", DateTime.Now + "| " + ex.Message);
                        }
                        break;
                    case 1075:
                        break;
                    case 1076:
                        break;
                    case 1077:
                        break;
                    case 1078:
                        break;
                    case 1079:
                        break;
                    case 1080:
                        break;
                    case 1081:
                        break;
                    case 1082:
                        break;
                    case 1084:
                        break;
                    case 1085:
                        break;
                    case 1086:
                        break;
                    case 1087:
                        break;
                    case 1088:
                        break;
                    case 1089:
                        break;
                    case 1090:
                        break;
                    case 1091:
                        break;
                    case 1092:
                        break;
                    case 1083:
                        break;
                    case 1093:
                        break;
                    case 1094:
                        break;
                    case 1095:
                        break;
                    case 1096:
                        break;
                    case 1097:
                        break;
                    case 1098:
                        break;
                    case 700:
                        break;
                    case 701:
                        break;
                    case 1110:
                        break;
                    case 1111:
                        break;
                    case 1112:
                        break;
                    case 1113:
                        break;
                    case 1114:
                        break;
                    case 1115:
                        break;
                    case 1120:
                        break;
                    case 1121:
                        break;
                    case 1130:
                        break;
                    case 1131:
                        break;
                    case 1140:
                        break;
                    case 1141:
                        break;
                    case 1142:
                        break;
                    case 1143:
                        break;
                    case 1144:
                        break;
                    case 1145:
                        break;
                    case 1146:
                        break;
                    case 1160:
                        break;
                    case 1161:
                        break;
                    case 1162:
                        break;
                    case 1163:
                        break;
                    case 1170:
                        break;
                    case 1171:
                        break;
                    case 1172:
                        break;
                    case 1173:
                        break;
                    case 1174:
                        break;
                    case 1190:
                        break;
                    case 1200:
                        break;
                    case 1250:
                        break;
                    case 1270:
                        break;
                    case 1280:
                        break;
                    case 1281:
                        break;
                    case 1282:
                        break;
                    case 1283:
                        break;
                    case 1284:
                        break;
                    case 1285:
                        break;
                    case 1286:
                        break;
                    case 1287:
                        break;
                    case 1300:
                        break;
                    case 1310:
                        break;
                    case 1320:
                        break;
                    case 1330:
                        break;
                    case 1400:
                        break;
                    case 1490:
                        break;
                    case 1500:
                        break;
                    case 1501:
                        break;
                    case 1502:
                        break;
                    case 1510:
                        break;
                    case 1511:
                        break;
                    case 1512:
                        break;
                    case 1513:
                        break;
                    case 1514:
                        break;
                    case 1515:
                        break;
                    case 1516:
                        break;
                    case 1517:
                        break;
                    case 1518:
                        break;
                    case 1519:
                        break;
                    case 1520:
                        break;
                    case 1600:
                        break;
                    case 1601:
                        break;
                    case 1602:
                        break;
                    case 1603:
                        break;
                    case 1604:
                        break;
                    case 1605:
                        break;
                    case 1606:
                        break;
                    case 1607:
                        break;
                    case 1608:
                        break;
                    case 1910:
                        break;
                    case 1911:
                        break;
                    case 1912:
                        break;
                    case 1913:
                        break;
                    case 1920:
                        break;
                    default:
                        output.Add("UNKNOWN ID => " + row["nType"]);
                        break;
                }
            }
            output.Add("-------END--------" + DateTime.Now + "-------END--------" + Environment.NewLine);
            return output;
        }
    }
}
