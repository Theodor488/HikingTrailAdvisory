using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace HikingTrailAdvisory
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var url = "https://www.wta.org/go-outside/hikes";
            var client = new HttpClient();
            var html = await client.GetStringAsync(url);

            ScrapeHikeData(html);
        }

        static void ScrapeHikeData(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var hikeNodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='listing-image backpack-wrapper backpack-hikebuttons']");

            if (hikeNodes != null)
            {
                foreach ( var hikeNode in hikeNodes )
                {
                    var hikeName = hikeNode.GetAttributeValue("data-hikename", string.Empty);
                    Console.WriteLine(hikeName);
                }
            }
            else
            {
                Console.WriteLine("No hikes found");
            }
        
        }
    }
}
