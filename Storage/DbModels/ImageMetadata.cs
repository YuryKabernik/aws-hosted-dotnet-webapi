namespace Storage.DbModels;

public class ImageMetadata
{
    public string Name { get; set; }
    public long Size { get; set; }
    public string Extension { get; set; }
    public DateTime LastUpdate { get; set; }
}
