using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace AbsNet
{
    public sealed class ListenServer
    {
        private Socket m_server;
        private int m_maxUser;
        private Semaphore m_maxAcceptClients;
        private ProcessAccept processAccept;
        public ListenServer(int maxUser,ProcessAccept processAccept)
        {
            m_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.m_maxUser = maxUser;
            this.processAccept = processAccept;
        }
        public void Init()
        {
            m_maxAcceptClients = new Semaphore(m_maxUser, m_maxUser);
        }

        public void Start(int port)
        {
            m_server.Bind(new IPEndPoint(IPAddress.Any, port));
            m_server.Listen(m_maxUser);
            StartAccept(null);
        }


        public void StartAccept(SocketAsyncEventArgs e)
        {
            if(e== null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptCallBack);
            }
            else
            {
                e.AcceptSocket = null;
            }
            m_maxAcceptClients.WaitOne();
            bool result = m_server.AcceptAsync(e);
            if(!result)
            {
                processAccept(e);
            }
        }

        private void AcceptCallBack(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                processAccept(e);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SemaphoreRelease()
        {
            m_maxAcceptClients.Release();
        }
    }
}
