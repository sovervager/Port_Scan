using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Collections;

class SocketClient
{
    [Obsolete]
    static void Main(string[] args)
    {
       if(args.Length < 2)                                                                                  //проверяем количество аргументов
        {
            Console.WriteLine("Not enogh parameters");
            Console.WriteLine("TextFile1 and port");
        }
        else
        {
            try                                                                                            //стараемся обезопасить от ошибок взятия файла, для этого используем try + catch
            {
                StreamReader ip_file = new StreamReader(args[0]);                                          //считываем файлы и вписываем в массивы
                StreamReader port_file = new StreamReader(args[1]);             
                ArrayList ips = new ArrayList();
                ArrayList pts = new ArrayList();
                String line;
                while ((line = ip_file.ReadLine()) != null) ips.Add(line);
                while ((line = port_file.ReadLine()) != null) pts.Add(line);
                foreach (String ip in ips) {                                                               //проходим по всем портам для каждого адреса
                    foreach (String port in pts)
                    {
                        try                                                                                                 // соединение с удаленным устройством
                               {                                                                                                   // устанавливаем удаленную тконечную точку для сокета
                                   IPHostEntry ipHost = Dns.Resolve(ip);                                                //используем класс Dns и его метод Resolve для того что бы получить адресс того имени которое передали
                                   IPAddress ipAddr = ipHost.AddressList[0];                                             //из массива адрессов вытаскиваем первый элемент масива 
                                   int p = 0;                                                                       //для преобразования символьного в строковое
                                   Int32.TryParse(port, out p);
                                   IPEndPoint ipEndPoint = new IPEndPoint (ipAddr, p);                                                     //создается точка подключения

                                   Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);    // создаем сокет и соединяем его с удаленной конечной точкой для этого используем клас Socket
                                   IAsyncResult result = sender.BeginConnect(ipEndPoint, null, sender);
                                   bool success = result.AsyncWaitHandle.WaitOne(1000, true);                                      //ставим ограничение в 100мс, если установить соеденение не удается, то происходит разрыв для того что бы не ждать долго, результат сохранится в переменную success/
                                   if (sender.Connected)
                                   {

                                       Console.WriteLine("Socket connected to {0}",  sender.RemoteEndPoint.ToString());            //Если все прошло успешно, то выводится сообщение о том что сокет подключен

                                                                                                                                   //освобождаем сокет
                                       sender.Shutdown(SocketShutdown.Both);
                                       sender.EndConnect(result);
                                       sender.Close();
                                   }
                                   else                                                                                            //прописываем условие если конекта не удалось
                                   {
                                       Console.WriteLine("Socket not connected to {0}",  sender.RemoteEndPoint.ToString());
                                       sender.Shutdown(SocketShutdown.Both);
                                       sender.Close();
                                   }
                               }
                               catch (Exception e)
                        {

                        }                                                                               //сработает если что то пошло не так(не получилось соеденится с EndPoint(или другие проблемы с сокетом))
                    }
                } 
            } 
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
       Console.ReadLine();
    }
}