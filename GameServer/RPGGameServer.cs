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
    public class RPGGameServer
    {
        static List<Class_Room> mRooms = new List<Class_Room>();

        static List<Class_User> mDeleteUsers = new List<Class_User>();

        static MySqlConnection tConnection = new MySqlConnection();

        public static Class_NetworkServer tServer = null;

        static void Main(string[] args)
        {
            Console.WriteLine("----RPGGame Network server----");

            int tPort = 50765;
            int tConnectionCount = 2;

            

            tServer = new Class_NetworkServer();
            tServer.StartServer(tPort, tConnectionCount);

            tConnection.ConnectionString = "Server=192.168.0.21;port=8889;Database=rpggamedb;Uid=Jiwoo;Pwd=1963;";

            tConnection.Open();

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
                                            UserConnect(tUser);

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
                                            tOffset += sizeof(int);
                                            tBufferSend[tOffset] = (byte)tAPLength;
                                            tOffset += 1;
                                            tAP.CopyTo(tBufferSend, tOffset);
                                            tOffset += sizeof(int);
                                            tBufferSend[tOffset] = (byte)tOccupationlength;
                                            tOffset += 1;
                                            tOccupation.CopyTo(tBufferSend, tOffset);

                                            tUser.Send(tBufferSend, tBufferSend.Length);
                                        }
                                        else if (tUserId == -1)
                                        {
                                            Console.WriteLine("No Exist User Id");

                                            tUserId = CreateUser(tId, tPassword, tUser);
                                            if (tUserId > -1)
                                            {
                                                byte[] tBufferSend = new byte[4];
                                                tBufferSend[0] = (byte)PROTOCOL.ACK_CREATE_CHAR;
                                                tBufferSend[1] = (byte)tUser.mId;

                                                tUser.Send(tBufferSend, tBufferSend.Length);

                                                Console.WriteLine("Create User Id");
                                            }
                                            else
                                            {
                                                byte[] tBufferSend = new byte[4];
                                                tBufferSend[0] = (byte)PROTOCOL.ACK_CREATE_CHAR;
                                                tBufferSend[1] = (byte)tUser.mId;

                                                tUser.Send(tBufferSend, tBufferSend.Length);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Password is wrong OR Already Connectioning");

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
                                        tRoom.mName = tUser.mName + "'s Room";
                                        tRoom.mMasterId = tUser.mName;

                                        tUser.mReadyPlay = READY_PLAY.READY;
                                        tUser.mpRoom = tRoom;
                                        tUser.mIsMaster = true;

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

                                            tOffset += tRoom.mName.Length;

                                            int ti = 0;
                                            foreach (var u in tRoom.mUsers)
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


                                                Class_User tMaster = tRoom.mUsers.Find(t => t.mName == tRoom.mMasterId);

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

                                        Class_User tOtherUser = tRoom.mUsers.Find(u => u.mIsMaster == false);

                                        //응답
                                        //방장정보를 게스트에게 송신
                                        byte[] tHP = BitConverter.GetBytes(tUser.mHP);
                                        int tHPLength = tHP.Length;
                                        byte[] tAP = BitConverter.GetBytes(tUser.mAP);
                                        int tAPLength = tAP.Length;
                                        byte[] tOccupation = Encoding.UTF8.GetBytes(tUser.mOccupation);
                                        int tOccupationLength = tUser.mOccupation.Length;

                                        byte[] tBufferSend = new byte[1024];
                                        tBufferSend[0] = (byte)PROTOCOL.ACK_BEGIN_PLAY;
                                        tBufferSend[1] = (byte)tUser.mId;

                                        tBufferSend[2] = (byte)tHPLength;
                                        int tOffset = 3;
                                        tHP.CopyTo(tBufferSend, tOffset);
                                        tOffset += sizeof(int);

                                        tBufferSend[tOffset] = (byte)tAPLength;
                                        tOffset += 1;
                                        tAP.CopyTo(tBufferSend, tOffset);
                                        tOffset += sizeof(int);

                                        
                                        tBufferSend[tOffset] = (byte)tOccupationLength;
                                        tOffset += 1;
                                        tOccupation.CopyTo(tBufferSend, tOffset);

                                        tOtherUser.Send(tBufferSend, tBufferSend.Length);
                                        //방장정보를 게스트에게 송신


                                        //게스트정보를 방장에게 송신
                                        byte[] tHP_ = BitConverter.GetBytes(tOtherUser.mHP);
                                        int tHPLength_ = tHP_.Length;
                                        byte[] tAP_ = BitConverter.GetBytes(tOtherUser.mAP);
                                        int tAPLength_ = tAP_.Length;
                                        byte[] tOccupation_ = Encoding.UTF8.GetBytes(tOtherUser.mOccupation);
                                        int tOccupationLength_ = tOtherUser.mOccupation.Length;


                                        byte[] tBufferSend_ = new byte[1024];
                                        tBufferSend_[0] = (byte)PROTOCOL.ACK_BEGIN_PLAY;
                                        tBufferSend_[1] = (byte)tOtherUser.mId;

                                        tBufferSend_[2] = (byte)tHPLength_;
                                        int tOffset_ = 3;
                                        tHP_.CopyTo(tBufferSend_, tOffset_);
                                        tOffset_ += sizeof(int);

                                        tBufferSend_[tOffset_] = (byte)tAPLength_;
                                        tOffset_ += 1;
                                        tAP_.CopyTo(tBufferSend_, tOffset_);
                                        tOffset_ += sizeof(int);


                                        tBufferSend_[tOffset_] = (byte)tOccupationLength_;
                                        tOffset_ += 1;
                                        tOccupation_.CopyTo(tBufferSend_, tOffset_);

                                        tUser.Send(tBufferSend_, tBufferSend_.Length);
                                        //게스트정보를 방장에게 송신
                                    }
                                    break;
                                case PROTOCOL.REQ_START_GAME:
                                    {
                                        if(tUser.mIsMaster==true)
                                        {
                                            Class_Room tRoom = tUser.mpRoom;

                                            byte[] tBufferSend = new byte[1024];

                                            tBufferSend[0] = (byte)PROTOCOL.ACK_START_GAME;
                                            tBufferSend[1] = (byte)tUser.mId;

                                            foreach (var u in tRoom.mUsers)
                                            {
                                                u.Send(tBufferSend, tBufferSend.Length);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Not Room Master");
                                        }
                                    }
                                    break;
                                case PROTOCOL.REQ_TURN_OVER:
                                    {
                                        Class_Room tRoom = tUser.mpRoom;

                                        byte[] tBufferSend = new byte[1024];

                                        tBufferSend[0] = (byte)PROTOCOL.ACK_TURN_OVER;
                                        tBufferSend[1] = (byte)tUser.mId;

                                        foreach(var u in tRoom.mUsers)
                                        {
                                            u.Send(tBufferSend, tBufferSend.Length);
                                        }
                                    }
                                    break;
                                case PROTOCOL.REQ_GAME_CLEAR:
                                    {
                                        //데이터베이스에 클리어에대한 보상 업데이트
                                        ClearUserUpdate(tUser);
                                        //데이터베이스에 클리어에대한 보상 업데이트

                                        //유저에게 보여줘야할 1등의 데이터 로드
                                        Class_User tFirstUser = new Class_User();

                                        FirstUserLoad(tFirstUser);
                                        //유저에게 보여줘야할 1등의 데이터 로드

                                        //응답
                                        byte[] tHP = BitConverter.GetBytes(tFirstUser.mHP);
                                        int tHPLength = tHP.Length;
                                        byte[] tAP = BitConverter.GetBytes(tFirstUser.mAP);
                                        int tAPLength = tAP.Length;
                                        byte[] tName = Encoding.UTF8.GetBytes(tFirstUser.mName);
                                        int tNameLength = tFirstUser.mName.Length;
                                        byte[] tOccupation = Encoding.UTF8.GetBytes(tFirstUser.mOccupation);
                                        int tOccupationLength = tFirstUser.mOccupation.Length;

                                        byte[] tBufferSend = new byte[1024];

                                        tBufferSend[0] = (byte)PROTOCOL.ACK_GAME_CLEAR;
                                        tBufferSend[1] = (byte)tFirstUser.mClearCount;

                                        tBufferSend[2] = (byte)tHPLength;
                                        int tOffset = 3;
                                        tHP.CopyTo(tBufferSend, tOffset);
                                        tOffset += sizeof(int);

                                        tBufferSend[tOffset] = (byte)tAPLength;
                                        tOffset += 1;
                                        tAP.CopyTo(tBufferSend, tOffset);
                                        tOffset += sizeof(int);

                                        tBufferSend[tOffset] = (byte)tNameLength;
                                        tOffset += 1;
                                        tName.CopyTo(tBufferSend, tOffset);
                                        tOffset += tNameLength;

                                        tBufferSend[tOffset] = (byte)tOccupationLength;
                                        tOffset += 1;
                                        tOccupation.CopyTo(tBufferSend, tOffset);

                                        tUser.Send(tBufferSend, tBufferSend.Length);
                                    }
                                    break;
                                case PROTOCOL.REQ_GAME_FAIL:
                                    {
                                        byte[] tBufferSend = new byte[1024];

                                        tBufferSend[0] = (byte)PROTOCOL.ACK_GAME_FAIL;

                                        tUser.Send(tBufferSend, tBufferSend.Length);
                                    }
                                    break;
                                case PROTOCOL.REQ_QUIT_GAME:
                                    {
                                        //ryu
                                        lock (tServer.lockObject)
                                        {
                                            DeleteMyUserInfo(tUser);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
                //임계영역
                //mutex
                lock (tServer.lockObject)
                {
                    tServer.mUsers.RemoveAll(mDeleteUsers.Contains);
                    mDeleteUsers.RemoveAll(mDeleteUsers.Contains);
                }
            }
            tConnection.Close();
        }

        public static void DeleteMyUserInfo(Class_User tUser)
        {
            tUser.mUserConnect = false;

            if (tUser.mpRoom != null)
            {
                Class_Room tRoom = tUser.mpRoom;
                

                if (tUser.mName == tRoom.mMasterId)
                {
                    Class_User tOtherUser = tRoom.mUsers.Find(u => u.mIsMaster == false);

                    //방장이 접속종료 할 시에 게스트도 강제 게임종료시킴.
                    byte[] tBufferSend = new byte[1024];
                    tBufferSend[0] = (byte)PROTOCOL.ACK_QUIT_GAME;
                    if (tOtherUser != null)
                    {
                        tOtherUser.Send(tBufferSend, tBufferSend.Length);
                    }
                    //방장이 접속종료 할 시에 게스트도 강제 게임종료시킴.

                    DeleteRoom(tRoom);

                    tUser.mpRoom = null;
                    mRooms.Remove(tRoom);
                    mDeleteUsers.Add(tUser);
                    Console.WriteLine("Delete Complete");
                }
                else
                {
                    ExitRoom(tUser);
                    tRoom.mUsers.Remove(tUser);
                    mDeleteUsers.Add(tUser);
                    Console.WriteLine("Not Room Master");
                }
            }
            else
            {
                ExitRoom(tUser);
                mDeleteUsers.Add(tUser);
                Console.WriteLine("User End Connected");
            }
        }

        public static int IsMember(string tId, string tPassword,Class_User tUser)
        {
            int tResult = -1;

            try
            {
                if (null != tConnection)
                {
                    string tQuery = "SELECT * FROM tbluserinfo where id='" + tId + "';";
                    MySqlCommand cmd = new MySqlCommand(tQuery, tConnection);
                    MySqlDataReader tExecuteR = cmd.ExecuteReader();

                    while (tExecuteR.Read())
                    {
                        if (tExecuteR["Password"].ToString() == tPassword&&(bool)tExecuteR["Connect"]==false)
                        {
                            tUser.mId = (int)tExecuteR["Key"];
                            tUser.mOccupation = tExecuteR["Occupation"].ToString();
                            tUser.mHP = (int)tExecuteR["HP"];
                            tUser.mAP = (int)tExecuteR["AttackAb"];
                            tUser.mName = tId;

                            if (tUser.mOccupation == "null")
                            {
                                tExecuteR.Close();
                                return tResult;
                            }
                            tResult = tUser.mId;
                        }
                        else
                        {
                            tResult = -2;
                        }
                    }
                    tExecuteR.Close();
                }
            }
            catch
            {
                Console.WriteLine("No Exist User Id");
            }
            return tResult;
        }

        public static void UserConnect(Class_User tUser)
        {
            try
            {
                if (null != tConnection)
                {
                    string tQuery = "Update tbluserinfo set Connect=true where id='" + tUser.mName + "';";
                    MySqlCommand cmd = new MySqlCommand(tQuery, tConnection);
                    MySqlDataReader tExecuteR = cmd.ExecuteReader();

                    tExecuteR.Close();
                }
            }
            catch
            {
                Console.WriteLine("No Exist User Id");
            }
        }

        public static int CreateUser(string tId, string tPassword, Class_User tUser)
        {
            int tResult = -1;

            try
            {
                if (null != tConnection)
                {
                    if (tUser.mOccupation != "null")
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

                        string tQuery = "insert into tbluserinfo values(" + tKey_ + ",'" + tId + "','" + tPassword + "','null',0,0,0,0,true);";
                        Console.WriteLine(tQuery);
                        MySqlCommand cmd_ = new MySqlCommand(tQuery, tConnection);
                        MySqlDataReader tExecuteR_ = cmd_.ExecuteReader();
                        while (tExecuteR_.Read())
                        {
                            tUser.mId = (int)tExecuteR["Key"];
                            tUser.mName = tId;
                        }
                        tExecuteR_.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Connection is not valid.");
            }
            return tResult;
        }

        public static void UpdateUserInfo(Class_User tUser)
        {
            try
            {
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
        }

        public static void CreateRoom(Class_User tUser)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
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
        }

        public static void UpdateJoinUser(Class_Room tRoom, Class_User tUser)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = tConnection;

                cmd.CommandText = "UpdateJoinUser";
                cmd.CommandType = CommandType.StoredProcedure;

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
        }

        public static void DeleteRoom(Class_Room tRoom)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();

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
        }

        public static void ExitRoom(Class_User tUser)
        {
            try
            {
                if (null != tConnection)
                {
                    string tQuery = "update tbluserinfo set Room_Id=0,Connect=false where tbluserinfo.key=" + tUser.mId + ";";
                    MySqlCommand cmd = new MySqlCommand(tQuery, tConnection);
                    MySqlDataReader tExecuteR = cmd.ExecuteReader();
                    tExecuteR.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void ClearUserUpdate(Class_User tUser)
        {
            try
            {
                if (null != tConnection)
                {
                    if (tUser.mOccupation == "Warrior")
                    {
                        string tQuery = "update tbluserinfo set ClearCount=ClearCount+1,HP=HP+50,AttackAb=AttackAb+30 where tbluserinfo.key=" + tUser.mId + ";";
                        MySqlCommand cmd = new MySqlCommand(tQuery, tConnection);
                        MySqlDataReader tExecuteR = cmd.ExecuteReader();
                        tExecuteR.Close();
                    }
                    else
                    {
                        string tQuery = "update tbluserinfo set ClearCount=ClearCount+1,HP=HP+30,AttackAb=AttackAb+50 where tbluserinfo.key=" + tUser.mId + ";";
                        MySqlCommand cmd = new MySqlCommand(tQuery, tConnection);
                        MySqlDataReader tExecuteR = cmd.ExecuteReader();
                        tExecuteR.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void FirstUserLoad(Class_User tFirstUser)
        {
            try
            {
                if (null != tConnection)
                {
                    string tQuery = "Select * from tbluserinfo order by tbluserinfo.ClearCount desc limit 1";
                    MySqlCommand cmd = new MySqlCommand(tQuery, tConnection);
                    MySqlDataReader tExecuteR = cmd.ExecuteReader();

                    while (tExecuteR.Read())
                    {
                        tFirstUser.mName = tExecuteR["Id"].ToString();
                        tFirstUser.mOccupation = tExecuteR["Occupation"].ToString();
                        tFirstUser.mHP = (int)tExecuteR["HP"];
                        tFirstUser.mAP = (int)tExecuteR["AttackAb"];
                        tFirstUser.mClearCount = (int)tExecuteR["ClearCount"];
                    }

                    tExecuteR.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}