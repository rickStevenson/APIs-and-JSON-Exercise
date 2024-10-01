using Newtonsoft.Json.Linq;
using System;
using System.Data;


namespace APIsAndJSON
{
    public class RonVSKanyeAPI
    {
        public static void RonVsKanyeQuotes()
        {
            int counter = 0;
            while (counter < 5)
            {
                var client = new HttpClient();

                var kanyeURL = "https://api.kanye.rest";

                var kanyeResponse = client.GetStringAsync(kanyeURL).Result;

                var kanyeQuote = JObject.Parse(kanyeResponse).GetValue("quote").ToString();

                Console.WriteLine($"Kanye: {kanyeQuote}");
                Console.WriteLine();

                var client2 = new HttpClient();

                var ronURL = "https://ron-swanson-quotes.herokuapp.com/v2/quotes";

                var ronResponse = client2.GetStringAsync(ronURL).Result;

                var ronQuote = JArray.Parse(ronResponse).ToString().Replace('[', ' ').Replace(']', ' ').Trim();

                Console.WriteLine($"Ron: {ronQuote}");
                Console.WriteLine();
                Console.WriteLine("_______________");
                Console.WriteLine();
                

                counter++;
            }
        }        
    }
}
