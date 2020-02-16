/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
using IFramework.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace IFramework_Demo
{
	public class SocExample:UnityEngine.MonoBehaviour
	{
        void packetTest()
        {
            Packet pack = new Packet(1,11,2,Encoding.Default.GetBytes("haha"));
            byte[] buf = pack.Pack();
          
            Packet pac = new Packet();
            pac.UnPack(buf, 0, buf.Length);
            PacketReader p = new PacketReader(128);
            p.Set(buf, 0, buf.Length);
            p.Set(buf, 0, buf.Length);

            List<Packet> ps = p.Get();
            Log.L(ps.Count);
        }
        void Tcp()
        {
            TcpServerToken token = new TcpServerToken(2, 2048);
            token.onAcccept += (tok) =>
            {
                Log.L(token.curConCount);
                Log.L("Accept " + tok.endPoint);

            };
            token.onReceive += (tok, seg) =>
            {
                //Log.I("Rec " + tok.EndPoint);
                byte[] buffer = new byte[seg.count];
                Array.Copy(seg.buffer, seg.offset, buffer, 0, seg.count);
                Log.L(Encoding.UTF8.GetString(buffer) + " SS ");
                token.SendAsync( tok,seg, true);
            };

            token.onDisConnect += (tok) =>
            {
                Log.L("Dis " + tok.endPoint);
                Log.L(token.curConCount);
            };
            token.Start(8888);
            // token.Stop();
            TcpClientToken c = new TcpClientToken(2048, 12);
            c.onReceive += (tok, seg) =>
            {
                //Log.I("Rec " + tok.EndPoint);
                byte[] buffer = new byte[seg.count];
                Array.Copy(seg.buffer, seg.offset, buffer, 0, seg.count);
                Thread.Sleep(1000);
                Log.L(Encoding.UTF8.GetString(buffer) + " cc ");
                c.SendAsync(seg, true);
            };
            c.ConnectAsync(8888, "127.0.0.1");
            Log.L(c.connected);
            byte[] bu = Encoding.UTF8.GetBytes("123");
            c.SendAsync(new BufferSegment( bu, 0, bu.Length));
            Thread.Sleep(1000);

            c.DisConnect();
            while (true)
            {
                if (c.connected)
                {
                    c.SendAsync(new BufferSegment( bu, 0, bu.Length));
                }
                Thread.Sleep(100);
            }
        }
        void Udp()
        {
            UdpServerToken s = new UdpServerToken(4096,32);
            s.onReceive += (tok,seg) =>
            {
                byte[] buffer = new byte[seg.count];
                Array.Copy(seg.buffer, seg.offset, buffer, 0, seg.count);
                Log.L("SS Rec " + tok.endPoint + " " + Encoding.UTF8.GetString(buffer));
                s.SendAsync(seg, tok.endPoint);
            };

            s.Start(8888);
            // s.Stop();
            UdpClientToken c = new UdpClientToken(2048, 10);
            c.onReceive += (tok,seg) =>
            {
                byte[] buffer = new byte[seg.count];
                Array.Copy(seg.buffer, seg.offset, buffer, 0, seg.count);
                Log.L("CC Rec" + Encoding.UTF8.GetString(buffer));
                c.Send(seg);
            };
            bool con = c.Connect(8888, "127.0.0.1");
            byte[] buff = Encoding.UTF8.GetBytes("12323");
            if (con)
            {
                c.Send(new BufferSegment( buff, 0, buff.Length));
            }

        }
        private void Start()
        {
            packetTest();
            Udp();
          //  Log.L(c.IsConnected);
            //byte[] bu = Encoding.UTF8.GetBytes("123");
            //c.SendAsync(new BufferSegment(bu, 0, bu.Length));
        }

        private static void WebSocketDemo()
        {
            WSServerToken wsService = new WSServerToken();
            wsService.onAccept = new OnAccept((SocketToken sToken) => {
                Console.WriteLine("accepted:" + sToken.endPoint);
            });
            wsService.onDisConnect = new OnDisConnect((SocketToken sToken) => {
                Console.WriteLine("disconnect:" + sToken.endPoint.ToString());
            });
            wsService.onRecieveString = new OnReceivedString((SocketToken sToken, string content) => {

                Console.WriteLine("receive:" + content);
                wsService.Send(sToken, "hello websocket client! you said:" + content);

            });
            wsService.onReceieve = new OnReceieve((SocketToken token, BufferSegment session) => {
                Console.WriteLine("receive bytes:" + session.count);
            });
            bool isOk = wsService.Start(65531);
            if (isOk)
            {
                Console.WriteLine("waiting for accept...");

                WSClientToken client = new WSClientToken();
                client.onConnect = new OnConnect((SocketToken sToken, bool isConnected) => {
                    Console.WriteLine("connected websocket server...");
                });
                client.onReceivedString = new OnReceivedString((SocketToken sToken, string msg) => {
                    Console.WriteLine(msg);
                });

                isOk = client.Connect("ws://127.0.0.1:65531");
                if (isOk)
                {
                    client.Send("hello websocket");
                }
            }
        }
    }
}
