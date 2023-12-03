using System.Net;
using System.Net.Sockets;
using System.Text;


namespace task_1
{
    internal class Program
    {
        private static CancellationTokenSource cts = new CancellationTokenSource();
        private static CancellationToken ct = cts.Token;
        private const string serverName = "Server";

        public static async Task Server()
        {
            Dictionary<string, IPEndPoint> UsersList = new Dictionary<string, IPEndPoint>();
            UdpClient udpServer = new UdpClient(12345);
            IPEndPoint localRemouteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Console.WriteLine("Ожидаем сообщение от пользователя:");

            while (!ct.IsCancellationRequested)
            {
                await Task.Run(() =>
                 {

                     try
                     {
                         byte[] buffer = udpServer.Receive(ref localRemouteEndPoint);
                         string data = Encoding.ASCII.GetString(buffer);
                         var message = Message.MessageFromJson(data);
                         Console.WriteLine($"Получено сообщение от {message.ToName}," +
                         $" время получения {message.DateMessage}, ");
                         Console.WriteLine(message.TextMessage);

                         StringBuilder answer = new StringBuilder("Сообщение получено");


                         if (message.ToName == serverName)
                         {
                             if (message.TextMessage == "register")
                             {
                                 UsersList.Add(message.NickName, new IPEndPoint(localRemouteEndPoint.Address, localRemouteEndPoint.Port));
                                 answer.Append($"Пользователь {message.NickName} добавлен.");
                             }
                             if (message.TextMessage == "delete")
                             {
                                 UsersList.Remove(message.NickName);
                                 answer.Append($"Пользователь {message.NickName} удалён.");
                             }
                             if (message.TextMessage == "list")
                             {
                                 foreach (var i in UsersList)
                                 {
                                     answer.Append($"Пользователь: {i.Key}, IP: {i.Value} \n");
                                 }
                             }
                         }
                         else
                         {
                             if (UsersList.TryGetValue(message.ToName, out IPEndPoint? ep))
                             {
                                 var answerMessage = new Message()
                                 {
                                     DateMessage = DateTime.Now,
                                     NickName = message.NickName,
                                     TextMessage = answer.ToString()
                                 };

                                 var answerData = answerMessage.MessageToJson();
                                 byte[] bytes = Encoding.ASCII.GetBytes(answerData);
                                 udpServer.Send(bytes, bytes.Length, localRemouteEndPoint);
                                 answer.Append("Сообщение переслано (клиенту)!");
                             }
                             else
                             {
                                 answer.Append("Такого пользователя не существует!");
                             }
                         }

                         var replyMessageJson = new Message()
                         {
                             DateMessage = DateTime.Now,
                             NickName = serverName,
                             TextMessage = answer.ToString()
                         }.MessageToJson();

                         byte[] replyBytes = Encoding.ASCII.GetBytes(replyMessageJson);

                         udpServer.Send(replyBytes, replyBytes.Length, localRemouteEndPoint);
                         Console.WriteLine("Ответ отправлен.");

                         if (message.TextMessage == "Exit")
                         {
                             Console.WriteLine("Для завершения работы сервера нажмите Enter!");

                             Console.ReadLine();

                             cts.Cancel();

                             Console.WriteLine("Сервер отключён!");

                             Environment.Exit(0);
                         }

                     }
                     catch (Exception e)
                     {
                         Console.WriteLine(e);
                     }
                 });
            }
            ct.ThrowIfCancellationRequested();
        }

        public static async Task Client(string name, string ip)
        {

            UdpClient udpClient = new UdpClient();
            IPEndPoint localRemouteEndPoint = new IPEndPoint(IPAddress.Parse(ip), 12345);
            while (true)
            {
                string[] message = Console.ReadLine().Split(' ');
                var mess = new Message()
                {
                    DateMessage = DateTime.Now,
                    NickName = name,
                    TextMessage = message[1],
                    ToName = message[0]
                };

                Console.WriteLine(mess.ToName);

                await Task.Run(() =>
                {

                    try
                    {
                        var data = mess.MessageToJson();
                        byte[] bytes = Encoding.ASCII.GetBytes(data);
                        udpClient.Send(bytes, bytes.Length, localRemouteEndPoint);

                        Console.WriteLine("Сообщение отпавлено!");

                        byte[] buffer = udpClient.Receive(ref localRemouteEndPoint);
                        data = Encoding.ASCII.GetString(buffer);
                        var messageReception = Message.MessageFromJson(data);
                        Console.WriteLine($"Получено сообщение от {messageReception.NickName}," +
                        $" время получения {messageReception.DateMessage}, ");
                        Console.WriteLine(messageReception.TextMessage);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                });

                if (message[1] == "Exit")
                {
                    Console.WriteLine("Приложение закрыто пользователем!");
                    Environment.Exit(0);
                }
            }
        }

        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                await Task.Run(() => Server());
            }
            else
            {
                await Task.Run(() => Client(args[0], args[1]));
            }
        }
    }
}