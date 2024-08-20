using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Practice_20._08._24
{
    internal class Program
    {
        static async Task Main()
        {
            var recipes = new Dictionary<string, List<string>>
            {
                { "eggs,milk", new List<string> { "Омлет", "Френч-тост" } },
                { "flour,sugar", new List<string> { "Блинчики", "Пирог" } },
                { "chicken,garlic", new List<string> { "Запеченная курица", "Курица с чесноком" } },
            };

            var tcpListener = new TcpListener(IPAddress.Any, 8888);
            tcpListener.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                _ = Task.Run(() => ProcessClient(tcpClient, recipes));
            }
        }

        static async Task ProcessClient(TcpClient tcpClient, Dictionary<string, List<string>> recipes)
        {
            using (tcpClient)
            {
                var stream = tcpClient.GetStream();
                var buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                Console.WriteLine($"Requast recived: {receivedMessage}");

                string responseMessage = ProcessRequest(receivedMessage, recipes);
                byte[] responseBytes = Encoding.UTF8.GetBytes(responseMessage + "\n");

                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
        }

        static string ProcessRequest(string request, Dictionary<string, List<string>> recipes)
        {
            if (recipes.TryGetValue(request.ToLower(), out var recipeList))
            {
                return string.Join(", ", recipeList);
            }
            else
            {
                return "Recipe not found.";
            }
        }
    }
}
