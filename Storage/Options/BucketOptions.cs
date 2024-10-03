namespace Storage.Options;

public class BucketOptions
{
    public const string SectionKey = "Bucket";
    public const string EnvironmentKey = "BUCKET_NAME";

    public string Name { get; set; } = string.Empty;
}