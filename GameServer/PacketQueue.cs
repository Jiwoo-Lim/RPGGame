using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

public class PacketQueue
{
    // 패킷 저장 정보.
    struct SPacketInfo
    {
        public int offset;
        public int size;
    };

    //
    private MemoryStream mStreamBuffer;

    private List<SPacketInfo> mOffsetList;

    private int mOffset = 0;


    // 생성자 
    public PacketQueue()
    {
        mStreamBuffer = new MemoryStream();
        mOffsetList = new List<SPacketInfo>();
    }

    // 큐에 패킷 정보를 put한다. rear에 넣는다.
    public int Enqueue(byte[] data, int size)
    {
        SPacketInfo info = new SPacketInfo();

        info.offset = mOffset;
        info.size = size;

        // 패킷 저장 정보를 보존.
        mOffsetList.Add(info);

        // 패킷 데이터를 보존.
        mStreamBuffer.Position = mOffset;
        mStreamBuffer.Write(data, 0, size);
        mStreamBuffer.Flush();
        mOffset += size;

        return size;
    }

    // 큐에서 패킷 정보를 get한다. front에서 가져온다.
    public int Dequeue(ref byte[] buffer, int size)
    {

        if (mOffsetList.Count <= 0)
        {
            return -1;
        }

        SPacketInfo info = mOffsetList[0];

        // 버퍼에서 해당하는 패킷 데이터를 가져옵니다.
        int dataSize = Math.Min(size, info.size);
        mStreamBuffer.Position = info.offset;
        int recvSize = mStreamBuffer.Read(buffer, 0, dataSize);

        // 큐 데이터를 가져왔으므로 선두 요소를 삭제.
        if (recvSize > 0)
        {
            mOffsetList.RemoveAt(0);
        }

        // 모든 큐 데이터를 가져왔을 때는 스트림을 클리어해서 메모리를 절약합니다. 
        if (mOffsetList.Count == 0)
        {
            Clear();
            mOffset = 0;
        }

        return recvSize;
    }

    //큐를 깨끗이 모두 비운다.
    public void Clear()
    {
        byte[] buffer = mStreamBuffer.GetBuffer();
        Array.Clear(buffer, 0, buffer.Length);

        mStreamBuffer.Position = 0;
        mStreamBuffer.SetLength(0);
    }
}