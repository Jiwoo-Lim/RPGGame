using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using MySql.Data.Common;
using MySql.Data.MySqlClient;

namespace RPGGameServer
{
    class RPGGameServer
    {
        static List<Class_Room> mRooms = new List<Class_Room>();

        static List<Class_User> mDeleteUsers = new List<Class_User>();

        static void Main(string[] args)
        {
            Console.WriteLine("----RPGGame Network server----");

            int tPort = 50765;
            int tConnectionCount = 2;

            Class_NetworkServer tServer = null;

            tServer = new Class_NetworkServer();
            tServer.StartServer(tPort, tConnectionCount);

            while (true)
            {
                Thread.Sleep(1);

                foreach (var tUser in tServer.mUsers)
                {
                    if (tUser.mSocketForClient != null)
                    {
                        byte[] tBuffer = new byte[1024];
                        int tRecvSize = tUser.GetFromQueue(ref tBuffer, tBuffer.Length);

                        if (tRecvSize <= 0)
                        {
                            continue;
                        }
                        else
                        {
                            PROTOCOL tProtocol = 0;
                            tProtocol = (PROTOCOL)tBuffer[0];

                            switch (tProtocol)
                            {
                                case PROTOCOL.REQ_LOGIN:
                                    if (tUser.mId <= 0)
                                    {
                                        string tId = Encoding.UTF8.GetString(tBuffer, 2, tBuffer[1]);
                                        string tPassword = Encoding.UTF8.GetString(tBuffer, tBuffer[1] + 3, tBuffer[tBuffer[1] + 2]);
                                        Console.WriteLine(tId + "\n" + tPassword);

                                        int tUserId = IsMember(tId, tPassword);
                                        if (tUserId > -1)
                                        {
                                            Class_User tTemp = new Class_User();
                                            tUser.mId = tUserId;

                                            byte[] tBufferSend = new byte[4];
                                            tBufferSend[0] = (byte)PROTOCOL.ACK_LOGIN;
                                            tBufferSend[1] = (byte)tUser.mId;

                                            tUser.Send(tBufferSend, tBufferSend.Length);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Search Fail User");

                                            CreateUser(tId, tPassword);

                                            tUser.mId = tUserId;

                                            byte[] tBufferSend = new byte[4];
                                            tBufferSend[0] = (byte)PROTOCOL.ACK_LOGIN;
                                            tBufferSend[1] = (byte)tUser.mId;

                                            tUser.Send(tBufferSend, tBufferSend.Length);

                                            Console.WriteLine("Create User Id");
                                        }
                                    }
                                    break;
                                case PROTOCOL.REQ_CREATE_ID:
                                    break;
                                case PROTOCOL.REQ_JOIN_ROOM:
                                    break;
                                case PROTOCOL.REQ_READY:
                                    break;
                                case PROTOCOL.REQ_BEGIN_PLAY:
                                    break;
                            }
                        }
                    }
                }
            }
        }

        static int IsMember(string tId, string tPassword)
        {
            int tResult = -1;

            MySqlConnection tConnection;
            string tConfigString = "Server=192.168.0.11;port=8889;Database=rpggamedb;Uid=poong;Pwd=0950;";

            tConnection = new MySqlConnection(tConfigString);

            try
            {
                tConnection.Open();
                Console.WriteLine("tConnection is opened.");

                if (null != tConnection)
                {
                    string tQuery = "SELECT * FROM tbluserinfo where id='" + tId + "';";
                    MySqlCommand cmd = new MySqlCommand(tQuery, tConnection);
                    MySqlDataReader tExecuteR = cmd.ExecuteReader();
                    string tStrDisplay = "";


                    while (tExecuteR.Read())
                    {
                        tStrDisplay += tExecuteR["Id"].ToString();
                        tStrDisplay += tExecuteR["Password"].ToString();

                        if (tExecuteR["Password"].ToString() == tPassword)
                        {
                            tResult = (int)tExecuteR["Key"];
                        }
                    }
                    tExecuteR.Close();
                }
            }
            catch
            {
                Console.WriteLine("Connection is not valid.");
            }
            return tResult;
        }

        static void CreateUser(string tId, string tPassword)
        {
            MySqlConnection tConnection;
            string tConfigString = "Server=192.168.0.11;port=8889;Database=rpggamedb;Uid=poong;Pwd=0950;";

            tConnection = new MySqlConnection(tConfigString);

            try
            {
                tConnection.Open();
                Console.WriteLine("tConnection is opened.");

                if (null != tConnection)
                {
                    string tKey = "select rpggamedb.tbluserinfo.key from rpggamedb.tbluserinfo order by rpggamedb.tbluserinfo.key desc limit 1;";
                    MySqlCommand cmd = new MySqlCommand(tKey, tConnection);
                    MySqlDataReader tExecuteR = cmd.ExecuteReader();
                    int tKey_ = 0;
                    while (tExecuteR.Read())
                    {
                        tKey_ = (int)tExecuteR["Key"]+1;
                        Console.WriteLine("tKey_ : " + tKey_);
                    }
                    tExecuteR.Close();

                    string tQuery = "insert into tbluserinfo values(" + tKey_ + ",'" + tId + "','" + tPassword + "','null',0,0,0);";
                    MySqlCommand cmd_ = new MySqlCommand(tQuery, tConnection);
                    MySqlDataReader tExecuteR_ = cmd_.ExecuteReader();
                    tExecuteR_.Close();


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Connection is not valid.");
            }
        }
    }
}
