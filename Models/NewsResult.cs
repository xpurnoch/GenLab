namespace BioLabManager.Models
{
    public class NewsResult
    {
        public List<NewsItem> Articles { get; set; } = new();
        public bool IsError { get; set; }
    }

}
