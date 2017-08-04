using AbsNet.Abs;
using AbsNet.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace AbsNet
{
    public class ServerCenter
    {
        public UserTokenPool userPool;
        public ListenServer listenServer;
        public AbsHandlerCenter messageHandler;

        public ServerCenter(int port,
                            int maxUser,
                            int receiveBufferSize,
                            AbsHandlerCenter messageHandler)
        {
            this.messageHandler = messageHandler;
            userPool = new UserTokenPool(maxUser);
            for (int i = 0; i < maxUser; i++)
            {
                UserToken token = new UserToken(receiveBufferSize);
                token.CC = ClientClose;
                token.LD = LengthEncoding.decode;
                token.LE = LengthEncoding.encode;
                token.SD = MessageEncoding.Decode;
                token.SE = MessageEncoding.Encode;
                token.messageHandler = messageHandler;
                userPool.Push(token);
            }
            listenServer = new ListenServer(maxUser, ProcessAccept);
            listenServer.Init();
            listenServer.Start(port);
        }

        public void ProcessAccept(SocketAsyncEventArgs e)
        {
            UserToken userToken = userPool.Pop();
            if (userToken != null)
            {
                userToken.ConnectSocket = e.AcceptSocket;
                messageHandler.ClientConnect(userToken);
                userToken.StartReceive();
                listenServer.StartAccept(e);
            }
        }

        public void ClientClose(UserToken token, string error)
        {
            if (token.ConnectSocket != null)
            {
                lock (token)
                {
                    messageHandler.ClientClose(token, error);
                    token.Close();
                    listenServer.SemaphoreRelease();
                    userPool.Push(token);
                }
            }
        }
    }
}
