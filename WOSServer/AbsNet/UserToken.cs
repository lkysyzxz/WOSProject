using AbsNet.Abs;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace AbsNet
{
    public class UserToken
    {
        private Socket m_connectSocket;
        private SocketAsyncEventArgs m_receiveEvent;
        private SocketAsyncEventArgs m_sendEvent;
        private List<byte> m_cache;

        public LengthDecode LD;
        public LengthEncode LE;
        public SerializeDecode SD;
        public SerializeEncode SE;
        public ClientClose CC;

        public AbsHandlerCenter messageHandler;

        //private ProcessReceive processReceive;
        //private ProcessSend processSend;
        //private ProcessClose processClose;

        private Queue<byte[]> m_sendQueue;
        private bool isWriting;
        private bool isReading;

        public Socket ConnectSocket
        {
            set
            {
                m_connectSocket = value;
            }
            get
            {
                return m_connectSocket;
            }
        }

        //public ProcessReceive ProcessReceive
        //{
        //    set
        //    {
        //        processReceive = value;
        //    }
        //}

        //public ProcessSend ProcessSend
        //{
        //    set
        //    {
        //        processSend = value;
        //    }
        //}

        //public ProcessClose ProcessClose
        //{
        //    set
        //    {
        //        processClose = value;
        //    }
        //}


        public UserToken(int bufferSize)
        {
            m_receiveEvent = new SocketAsyncEventArgs();
            m_receiveEvent.Completed += ReceiveCallBack;
            m_sendEvent = new SocketAsyncEventArgs();
            m_sendEvent.Completed += SendCallBack;

            m_receiveEvent.UserToken = this;
            m_sendEvent.UserToken = this;

            byte[] buffer = new byte[bufferSize];
            m_receiveEvent.SetBuffer(buffer, 0, buffer.Length);

            m_cache = new List<byte>();
            m_sendQueue = new Queue<byte[]>();

            isWriting = false;
            isReading = false;
        }

        public void StartReceive()
        {
            try
            {
                bool result = m_connectSocket.ReceiveAsync(m_receiveEvent);
                if (!result)
                {
                    lock (this)
                    {
                        ProcessReceive(m_receiveEvent);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Receive Message Error " + e);
            }
        }

        private void ReceiveCallBack(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                lock (this)
                {
                    if (e.LastOperation == SocketAsyncOperation.Receive)
                        ProcessReceive(e);
                }
            }
            catch (Exception ex)
            {
                CC(this, ex.Message);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                byte[] buffer = new byte[e.BytesTransferred];
                Buffer.BlockCopy(e.Buffer, 0, buffer, 0, e.BytesTransferred);
                m_cache.AddRange(buffer);
                if (!isReading)
                {
                    isReading = true;
                    OnData();
                }
                StartReceive();
            }
            else
            {
                string error = "";
                if (e.SocketError != SocketError.Success)
                {
                    error = e.SocketError.ToString();
                }
                else
                {
                    error = "远程客户端已经断开连接";
                }
                CC(this, error);
            }
        }

        private void OnData()
        {
            byte[] buffer;
            if (LD != null)
            {
                buffer = LD(ref m_cache);
                if (buffer == null)
                {
                    isReading = false;
                    return;
                }
            }
            else if (m_cache.Count != 0)
            {
                buffer = m_cache.ToArray();
                m_cache.Clear();
            }
            else
            {
                return;
            }

            if (SD == null) throw new Exception("message decode process is null");
            if (messageHandler == null) throw new Exception("message handler is null");
            object message = SD(buffer);

            //消息分发
            messageHandler.MessageReceive(this, message);
            isReading = false;
            OnData();
        }

        private void SendCallBack(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                lock (this)
                {
                    if(e.LastOperation==SocketAsyncOperation.Send)
                        ProcessSend(e);
                }
            }
            catch (Exception ex)
            {
                CC(this, ex.Message);
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                CC(this, e.SocketError.ToString());
            }
            else
            {
                SendEnd();
            }
        }

        private void SendEnd()
        {
            OnWrite();
        }

        private void OnWrite()
        {
            if (m_sendQueue.Count == 0)
            {
                isWriting = false;
                return;
            }
            byte[] buffer = m_sendQueue.Dequeue();
            m_sendEvent.SetBuffer(buffer, 0, buffer.Length);
            bool result = m_connectSocket.SendAsync(m_sendEvent);
            if (!result)
            {
                ProcessSend(m_sendEvent);
            }
        }

        public void Write(byte[] buffer)
        {
            if (m_connectSocket == null)
            {
                CC(this, "Client has disconnected");
                return;
            }
            m_sendQueue.Enqueue(buffer);
            if (!isWriting)
            {
                isWriting = true;
                OnWrite();
            }
        }


        public void Close()
        {
            try
            {
                m_sendQueue.Clear();
                m_cache.Clear();
                isReading = false;
                isWriting = false;
                m_connectSocket.Shutdown(SocketShutdown.Both);
                m_connectSocket.Close();
                m_connectSocket = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
