enum PROTOCOL
{
    //TitleScene Start
    //타이틀 화면 로그인 아이디 없을경우 아이디 생성
    ACK_CONNECT = 10,
    REQ_LOGIN,
    ACK_LOGIN,
    //TitleScene End

    //CreateCharactorScene Start
    //캐릭터가 없을경우 캐릭터만드는 씬으로 전환
    ACK_CREATE_CHAR,
    //생성할 캐릭터를 생성
    REQ_CREATE_CHAR,
    //CreateCharactorScene End

    //RoomSelectScene Start
    //생성된 방이 없을 경우
    REQ_CREATE_ROOM,
    ACK_CREATE_ROOM,
    
    //생성된 방이 있을 경우 입장
    REQ_JOIN_ROOM,
    ACK_JOIN_ROOM,
    //RoomSelectScene End

    //RoomScene Start
    //방장은 항상 준비상태, 모두 준비되면 AllReady 방장에게 송신
    REQ_READY,
    ACK_READY,
    ACK_ALL_READY,

    //게임 씬으로 전환
    REQ_BEGIN_PLAY,
    ACK_BEGIN_PLAY,
    //RoomScene End
}