using BioLabManager.Models;
using System.Net.Http;
using System.Xml.Linq;

namespace BioLabManager.Services
{
	public static class NewsService
	{
        public static async Task<NewsResult> FetchScienceNewsAsync()
        {
            var result = new NewsResult();
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; BioLabManager/1.0)");
                var response = await client.GetStringAsync("https://www.sciencedaily.com/rss/top/science.xml");
                var doc = XDocument.Parse(response);
                foreach (var item in doc.Descendants("item"))
                {
                    result.Articles.Add(new NewsItem
                    {
                        Title = item.Element("title")?.Value,
                        Summary = item.Element("description")?.Value,
                        Url = item.Element("link")?.Value,
                    });
                }
            }
            catch
            {
                result.IsError = true;
            }
            return result;
        }

    }
}
