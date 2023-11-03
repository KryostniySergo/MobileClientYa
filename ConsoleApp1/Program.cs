using System.Net.Http;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            var dict = new Dictionary<string, string>
            {
                { "filename", "65743582-4602-4942-bc89-6743cce9ec35.ogg" }
            };

            using (var response = client.PostAsync(
                @"http://195.135.252.148:5000/api", 
                new FormUrlEncodedContent(dict))) 
            {
                Console.WriteLine(response.Result.StatusCode.ToString());
            } ;

        }
    }
}