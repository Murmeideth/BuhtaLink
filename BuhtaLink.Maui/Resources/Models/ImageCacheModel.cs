namespace BuhtaLink.Models;

public class ImageCacheModel
{
    public string Url { get; set; }
    public string LocalPath { get; set; }
    public DateTime CachedDate { get; set; }
    public long FileSize { get; set; }
}