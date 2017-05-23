using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;


namespace Cliеnt
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                SendMessageFromSocket(11000);
            }
           catch(Exception f)
            {
                Console.WriteLine(f.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }

        static void SendMessageFromSocket(int port)
        {
            // Буфер для входящих данных
            byte[] bytes = new byte[1024];

            // Соединяемся с удаленным устройством

            // Устанавливаем удаленную точку для сокета
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);

            Console.Write("Введите n: ");
            int n = int.Parse(Console.ReadLine());
            Console.Write("Введите m: ");
            int m = int.Parse(Console.ReadLine());
            string message;
            Console.WriteLine("Сокет соединяется с {0} ", sender.RemoteEndPoint.ToString());

            byte[] msg = new byte[1024];
            int[,] arr = new int[n, m] ;
            Console.WriteLine("Введите элементы: ");
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    arr[i, j]=int.Parse(Console.ReadLine());
                }
            }
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream("matrix.dat", FileMode.OpenOrCreate))
            {
                // сериализуем весь массив 
                formatter.Serialize(fs, arr);
                Console.WriteLine("Объект сериализован");
            }
            message = n.ToString() + " " + m.ToString();
            msg = Encoding.UTF8.GetBytes(message);
            // Отправляем данные через сокет
            //int bytesSent = sender.Send(msg);
            sender.SendFile("matrix.dat");

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);

            Console.WriteLine("\nОтвет от сервера: {0}\n\n", Encoding.UTF8.GetString(bytes, 0, bytesRec));

            // Используем рекурсию для неоднократного вызова SendMessageFromSocket()
            if (message.IndexOf("<TheEnd>") == -1)
                SendMessageFromSocket(port);

            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
    }
}
