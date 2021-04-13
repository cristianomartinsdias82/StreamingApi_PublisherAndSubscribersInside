using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace EventsListenerConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const int timeoutInDays = 1;
            Stream dataStream;
            StreamReader reader;

            while (true)
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromDays(timeoutInDays);

                    dataStream = await httpClient.GetStreamAsync("https://localhost:5001/api/v1/customers/streaming");

                    reader = new StreamReader(dataStream);
                    Console.WriteLine(await reader.ReadLineAsync());
                    reader.Close();
                }
            }
        }
    }
}
