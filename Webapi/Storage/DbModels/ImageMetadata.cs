namespace Storage.DbModels;

public class ImageMetadata
{
    public required string Name { get; set; }
    public long Size { get; set; }
    public required string Extension { get; set; }
    public DateTime LastUpdate { get; set; }
}
