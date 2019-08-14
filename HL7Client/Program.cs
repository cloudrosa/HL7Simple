using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHapiTools.Base.Util;

namespace HL7Client
{
    class Program
    {
        private static Socket socketClient { get; set; }

        static void Main(string[] args)
        {
            try
            {
                socketClient = new Socket(SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                IPEndPoint point = new IPEndPoint(ip, 2020);
                socketClient.Connect(point);


                Thread threadSend = new Thread(Send);
                threadSend.IsBackground = true;
                threadSend.Start();

                Thread threadRecv = new Thread(Reve);
                threadRecv.IsBackground = true;
                threadRecv.Start();

                Console.WriteLine("发送成功");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            Console.ReadKey();
        }

        static void Send()
        {
            string content = @"MSH|^~\&|PEIS||EAI||20190320182547||ZMG^Z30|PEIS_3468011|P|2.4|||AL|AL|CHN  PID|1901090088|0000558984|0000558984^^^^IDCard~431002198702161010^^^^IdentifyNO||张洵||19870216|M||||||||||||||||||||||||||||||  PV1|1|T|^^^2400^健康管理中心|||||||||||||0|001921^李东霞|01|1901090088|||||||||0||||||||||||||||20190320182547|||||||0  ORC|NW|T1484237^^1901090088^o|T190109008800095159|1901090088|1||||20190320182547|001921||001921^李东霞|||20190320182547||2400^健康管理中心  OBR|4|||F00000095159^一般健康体检^WZ|||||||||||||||||||45|  ZDS||||F00000017022|||||||||||||||||||||||1||||||||||||||||||1||  ";
            content = MLLP.CreateMLLPMessage(content);

            var buffer = Encoding.UTF8.GetBytes(content);

            var temp = socketClient.Send(buffer);

            Console.WriteLine(temp);
            Console.WriteLine("发送成功");
        }

        static void Reve()
        {
            while (true)
            {
                try
                {
                    //定义一个1M的内存缓冲区，用于临时性存储接收到的消息  
                    byte[] arrRecvmsg = new byte[1024 * 1024];
                    //将客户端套接字接收到的数据存入内存缓冲区，并获取长度  
                    int length = socketClient.Receive(arrRecvmsg);
                    //将套接字获取到的字符数组转换为人可以看懂的字符串  
                    string strRevMsg = Encoding.UTF8.GetString(arrRecvmsg, 0, length);
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\r\n" + strRevMsg + "\r\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}
