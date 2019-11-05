using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Data;
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

                                        int tUserId = IsMember(tId, tPassword, tUser);
                                        if (tUserId > -1)
                                        {
                                            tUser.mId = tUserId;
                                            tUser.mName = tId;
                                            tUser.mUserConnect = true;

                                            int tOccupationlength = tUser.mOccupation.Length;
                                            byte[] tOccupation = Encoding.UTF8.GetBytes(tUser.mOccupation);

                                            byte[] tHP = BitConverter.GetBytes(tUser.mHP);
                                            int tHPLength = tHP.Length;
                                            byte[] tAP = BitConverter.GetBytes(tUser.mAP);
                                            int tAPLength = tAP.Length;

                                            byte[] tBufferSend = new byte[1024];
                                            tBufferSend[0] = (byte)PROTOCOL.ACK_LOGIN;
                                            tBufferSend[1] = (byte)tUser.mId;
                                            tBufferSend[2] = (byte)tHPLength;
                                            int tOffset = 3;
                                            tHP.CopyTo(tBufferSend, tOffset);
                                            tOffset += tHPLength;
                                            tBufferSend[tOffset] = (byte)tAPLength;
                                            tOffset += 1;
                                            tAP.CopyTo(tBufferSend, tOffset);
                                            tOffset += tAPLength;
                                            tBufferSend[tOffset] = (byte)tOccupationlength;
                                            tOffset += 1;
                                            tOccupation.CopyTo(tBufferSend, tOffset);

                                            tUser.Send(tBufferSend, tBufferSend.Length);
                                        }
                                        else if (tUserId == -1) 
                                        {
                                            Console.WriteLine("No Exist User Id");

                                            tUserId = CreateUser(tId, tPassword,tUser);

                                            tUser.mId = tUserId;
                                            tUser.mName = tId;

                                            byte[] tBufferSend = new byte[4];
                                            tBufferSend[0] = (byte)PROTOCOL.ACK_CREATE_CHAR;
                                            tBufferSend[1] = (byte)tUser.mId;

                                            tUser.Send(tBufferSend, tBufferSend.Length);

                                            Console.WriteLine("Create User Id");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Password is wrong");

                                            byte[] tBufferSend = new byte[4];

                                            tBufferSend[0] = (byte)PROTOCOL.ACK_LOGIN_FAIL;

                                            tUser.Send(tBufferSend, tBufferSend.Length);
                                        }
                                    }
                                    break;
                                case PROTOCOL.REQ_CREATE_CHAR:
                                    {
                                        int tHPLength = tBuffer[1];
                                        int tOffset = 2;
                                        int tHP = BitConverter.ToInt32(tBuffer, tOffset);
                                        tOffset += sizeof(int);

                                        int tAPLength = tBuffer[tOffset];
                                        tOffset += 1;
                                        int tAP = BitConverter.ToInt32(tBuffer, tOffset);
                                        tOffset += sizeof(int); 

                                        int tOccupationlength = tBuffer[tOffset];
                                        tOffset += 1;
                                        string tOccupation = Encoding.UTF8.GetString(tBuffer, tOffset, tOccupationlength);

                                        Console.WriteLine("HP : " + tHP);
                                        Console.WriteLine("AP : " + tAP);

                                        tUser.mHP = tHP;
                                        tUser.mAP = tAP;
                                        tUser.mOccupation = tOccupation;

                                        UpdateUserInfo(tUser);

                                        //응답
                                        byte[] tBufferSend = new byte[1024];
                                        tBufferSend[0] = (byte)PROTOCOL.ACK_LOGIN;
                                        tUser.Send(tBufferSend, tBufferSend.Length);
                                    }
                                    break;
                                case PROTOCOL.REQ_CREATE_ROOM:
                                    {
                                        Console.WriteLine("REQ_CREATE_ROOM");

                                        Class_Room tRoom = new Class_Room();

                                        CreateRoom(tUser);

                                        tRoom.mId = tUser.mRoomId;
                                        tRoom.mName = tUser.mName+"'s Room";
                                        tRoom.mMasterId = tUser.mName;

                                        tUser.mReadyPlay = READY_PLAY.READY;
                                        tUser.mpRoom = tRoom;
                                        
                                        tRoom.mUsers.Add(tUser);

                                        mRooms.Add(tRoom);


                                        //응답
                                        byte[] tMasterId = Encoding.UTF8.GetBytes(tRoom.mMasterId);
                                        int tMasterIdLength = (tRoom.mMasterId.Length);

                                        byte[] tRoomName = Encoding.UTF8.GetBytes(tRoom.mName);
                                        int tRoomNameLength = tRoom.mName.Length;

                                        byte[] tBufferSend = new byte[1024];
                                        tBufferSend[0] = (byte)PROTOCOL.ACK_CREATE_ROOM;
                                        tBufferSend[1] = (byte)tRoom.mId;

                                        tBufferSend[2] = (byte)tMasterIdLength;
                                        int tOffset = 3;
                                        tMasterId.CopyTo(tBufferSend, tOffset);

                                        tBufferSend[tMasterIdLength + tOffset] = (byte)tRoomNameLength;
                                        tOffset = tOffset + tMasterIdLength + 1;
                                        tRoomName.CopyTo(tBufferSend, tOffset);

                                        tUser.Send(tBufferSend, tBufferSend.Length);
                                    }
                                    break;
                                case PROTOCOL.REQ_JOIN_ROOM:
                                    {
                                        Console.WriteLine("REQ_JOIN_ROOM");

                                        Console.WriteLine("Find Room");
                                        foreach (var r in mRooms)
                                        {
                                            Console.WriteLine(r.mUsers.Count);
                                        }
                                        Class_Room tRoom = mRooms.Find(t => t.mUsers.Count == 1);

                                        if (tRoom != null)
                                        {
                                            

                                            tUser.mReadyPlay = READY_PLAY.NOT_READY;
                                            tUser.mpRoom = tRoom;
                                            tRoom.mUsers.Add(tUser);

                                            UpdateJoinUser(tRoom, tUser);


                                            //응답
                                            byte[] tBufferSend = new byte[1024];

                                            tBufferSend[0] = (byte)PROTOCOL.ACK_JOIN_ROOM;
                                            tBufferSend[1] = (byte)tRoom.mId;
                                            tBufferSend[2] = (byte)tRoom.mUsers.Count;
                                            tBufferSend[3] = (byte)tRoom.mMasterId.Length;
                                            int tOffset = 4;
                                            byte[] tMasterId = Encoding.UTF8.GetBytes(tRoom.mMasterId);
                                            tMasterId.CopyTo(tBufferSend, tOffset);

                                            tOffset += tRoom.mMasterId.Length;

                                            tBufferSend[tOffset] = (byte)tRoom.mName.Length;
                                            tOffset += 1;
                                            byte[] tRoomName = Encoding.UTF8.GetBytes(tRoom.mName);
                                            tRoomName.CopyTo(tBufferSend, tOffset);

                                            tOffset = tOffset + tRoom.mName.Length;

                                            int ti = 0;
                                            foreach(var u in tRoom.mUsers)
                                            {
                                                tOffset += ti;
                                                int tLength = u.mName.Length;
                                                tBufferSend[tOffset] = (byte)tLength;
                                                tOffset += 1;
                                                byte[] UserName = Encoding.UTF8.GetBytes(u.mName);
                                                UserName.CopyTo(tBufferSend, tOffset);
                                                ti = tLength;
                                            }

                                            foreach (var u in tRoom.mUsers)
                                            {
                                                u.Send(tBufferSend, tBufferSend.Length);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("No Exist Room");

                                            byte[] tBufferSend = new byte[4];

                                            tBufferSend[0] = (byte)PROTOCOL.ACK_JOIN_ROOM_FAIL;

                                            tUser.Send(tBufferSend, tBufferSend.Length);
                                        }
                                    }
                                    break;
                                case PROTOCOL.REQ_READY:
                                    {
                                        Class_Room tRoom = tUser.mpRoom;

                                        if (null != tRoom)
                                        {
                                            tUser.mReadyPlay = READY_PLAY.READY;

                                            bool tIsAllUserReady = true;
                                            foreach (var t in tRoom.mUsers)
                                            {
                                                if (t.mReadyPlay == READY_PLAY.NOT_READY)
                                                {
                                                    tIsAllUserReady = false;
                                                    break;
                                                }
                                            }

                                            Console.WriteLine("user list-------");
                                            foreach (var t in tRoom.mUsers)
                                            {
                                                Console.WriteLine("user id: " + t.mId);
                                                Console.WriteLine("user ready state: " + t.mReadyPlay);
                                            }
                                            Console.WriteLine("--------------");


                                            //응답
                                            if (true == tIsAllUserReady)
                                            {
                                                Console.WriteLine("all users is ready.");

                                                byte[] tBufferSend = new byte[1024];

                                                tBufferSend[0] = (byte)PROTOCOL.ACK_ALL_READY;


                                                Class_User tMaster = tRoom.mUsers.Find(t=>t.mName==tRoom.mMasterId);

                                                tMaster.Send(tBufferSend, tBufferSend.Length);
                                                
                                            }
                                            else
                                            {
                                                byte[] tBufferSend = new byte[1024];

                                                tBufferSend[0] = (byte)PROTOCOL.ACK_READY;

                                                foreach (var u in tRoom.mUsers)
                                                {
                                                    u.Send(tBufferSend, tBufferSend.Length);
                                                }

                                                Console.WriteLine("-------all users is not ready.");
                                            }
                                        }
                                    }
                                    break;
                                case PROTOCOL.REQ_BEGIN_PLAY:
                                    {
                                        Class_Room tRoom = tUser.mpRoom;

                                        //유저정보 로드

                                        //응답
                                        byte[] tBufferSend = new byte[1024];

                                        tBufferSend[0] = (byte)PROTOCOL.ACK_BEGIN_PLAY;

                                        foreach(var u in tRoom.mUsers)
                                        {
                                            u.Send(tBufferSend, tBufferSend.Length);
                                        }
                                    }
                                    break;
                                case PROTOCOL.REQ_QUIT_GAME:
                                    {
                                        Class_Room tRoom = tUser.mpRoom;

                                        if (tUser.mName == tRoom.mMasterId)
                                        {
                                            DeleteRoom(tRoom);

                                            tUser.mpRoom = null;
                                            mRooms.Remove(tRoom);

                                            Console.WriteLine("Delete Complete");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Not Room Master");
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        static int IsMember(string tId, string tPassword,Class_User tUser)
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


                    while (tExecuteR.Read())
                    {
                        if (tExecuteR["Password"].ToString() == tPassword)
                        {
                            tResult = (int)tExecuteR["Key"];
                            tUser.mOccupation = tExecuteR["Occupation"].ToString();
                            tUser.mHP = (int)tExecuteR["HP"];
                            tUser.mAP = (int)tExecuteR["AttackAb"];
                        }
                        else
                        {
                            tResult = -2;
                        }

                        tUser.mName = tExecuteR["Id"].ToString();
                    }
                    tExecuteR.Close();
                }
            }
            catch
            {
                Console.WriteLine("No Exist User Id");
            }
            tConnection.Close();
            return tResult;
        }

        static int CreateUser(string tId, string tPassword, Class_User tUser)
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
                    string tKey = "select rpggamedb.tbluserinfo.key from rpggamedb.tbluserinfo order by rpggamedb.tbluserinfo.key desc limit 1;";
                    MySqlCommand cmd = new MySqlCommand(tKey, tConnection);
                    MySqlDataReader tExecuteR = cmd.ExecuteReader();
                    int tKey_ = 0;
                    while (tExecuteR.Read())
                    {
                        tKey_ = (int)tExecuteR["Key"] + 1;
                        Console.WriteLine("tKey_ : " + tKey_);
                        tResult = tKey_;
                    }
                    tExecuteR.Close();

                    string tQuery = "insert into tbluserinfo values(" + tKey_ + ",'" + tId + "','" + tPassword + "','null',0,0,0,0);";
                    Console.WriteLine(tQuery);
                    MySqlCommand cmd_ = new MySqlCommand(tQuery, tConnection);
                    MySqlDataReader tExecuteR_ = cmd_.ExecuteReader();
                    while (tExecuteR_.Read())
                    {
                        tUser.mName = tExecuteR["Id"].ToString();
                    }
                    tExecuteR_.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Connection is not valid.");
            }
            tConnection.Close();
            return tResult;
        }

        static void UpdateUserInfo(Class_User tUser)
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
                    string tQuery = "update tbluserinfo set Occupation='" + tUser.mOccupation + "', HP=" + tUser.mHP + ", AttackAb=" + tUser.mAP + " where tbluserinfo.key=" + tUser.mId + ";";
                    MySqlCommand cmd = new MySqlCommand(tQuery, tConnection);
                    MySqlDataReader tExecuteR = cmd.ExecuteReader();
                    tExecuteR.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Connection is not valid.");
            }
            tConnection.Close();
        }

        static void CreateRoom(Class_User tUser)
        {
            MySqlConnection tConnection = new MySqlConnection();
            tConnection.ConnectionString = "Server=192.168.0.11;port=8889;Database=rpggamedb;Uid=poong;Pwd=0950;";
            MySqlCommand cmd = new MySqlCommand();

            try
            {
                tConnection.Open();
                Console.WriteLine("tConnection is opened.");
                cmd.Connection = tConnection;

                cmd.CommandText = "CreateRoom";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("tUserId", tUser.mName);
                cmd.Parameters["tUserId"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("tRoomName", tUser.mName + "'s Room");
                cmd.Parameters["tRoomName"].Direction = ParameterDirection.Input;

                cmd.Parameters.Add(new MySqlParameter("roomid", MySqlDbType.Int32));
                cmd.Parameters["roomid"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                tUser.mRoomId = Convert.ToInt32(cmd.Parameters["roomid"].Value);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            tConnection.Close();
        }

        static void UpdateJoinUser(Class_Room tRoom, Class_User tUser)
        {
            MySqlConnection tConnection = new MySqlConnection();
            tConnection.ConnectionString = "Server=192.168.0.11;port=8889;Database=rpggamedb;Uid=poong;Pwd=0950;";
            MySqlCommand cmd = new MySqlCommand();

            try
            {
                tConnection.Open();
                Console.WriteLine("tConnection is opened.");
                cmd.Connection = tConnection;

                cmd.CommandText = "UpdateJoinUser";
                cmd.CommandType = CommandType.StoredProcedure;

                    //test
                    Console.WriteLine(tRoom.mId);
                    Console.WriteLine(tUser.mId);

                    //tRoom.mId = 1;
                    //tUser.mId = 3;

                cmd.Parameters.AddWithValue("tRoomId", tRoom.mId);
                cmd.Parameters["tRoomId"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("tUserkey", tUser.mId);
                cmd.Parameters["tUserkey"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            tConnection.Close();
        }

        static void DeleteRoom(Class_Room tRoom)
        {
            MySqlConnection tConnection = new MySqlConnection();
            tConnection.ConnectionString = "Server=192.168.0.11;port=8889;Database=rpggamedb;Uid=poong;Pwd=0950;";
            MySqlCommand cmd = new MySqlCommand();

            try
            {
                tConnection.Open();
                Console.WriteLine("tConnection is opened.");
                cmd.Connection = tConnection;

                cmd.CommandText = "DeleteRoom";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("tRoomId", tRoom.mId);
                cmd.Parameters["tRoomId"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
            }
            catch
            {
                Console.WriteLine("No Exist Room");
            }
            tConnection.Close();
        }
    }
}