using System.Collections.Generic;
using System.Net.Sockets;

namespace AbsNet
{
    public delegate void ProcessAccept(SocketAsyncEventArgs e);

    public delegate void ClientClose(UserToken token, string error);

    public delegate byte[] LengthEncode(byte[] value);

    public delegate byte[] LengthDecode(ref List<byte> cache);

    public delegate byte[] SerializeEncode(object value);

    public delegate object SerializeDecode(byte[] value);
}
