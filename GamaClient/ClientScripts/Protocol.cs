enum PROTOCOL
{
    //타이틀 화면 로그인
    ACK_CONNECT = 10, 
    REQ_LOGIN,
    ACK_LOGIN,

    //아이디 없을경우 아이디 생성
    REQ_CREATE_ID,
    ACK_CREATE_ID,

    //캐릭터가 있을경우 방 입장, 없을경우 캐릭터만드는 씬으로 전환
    REQ_JOIN_ROOM,
    ACK_JOIN_ROOM,
    ACK_CREATE_CHAR,

    //방장은 항상 준비상태, 모두 준비되면 AllReady 방장에게 송신
    REQ_READY,
    ACK_READY,
    ACK_ALL_READY,

    //게임 씬으로 전환
    REQ_BEGIN_PLAY,
    ACK_BEGIN_PLAY,
}