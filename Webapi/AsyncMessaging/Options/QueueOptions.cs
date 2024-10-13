namespace AsyncMessaging.Options;

public class QueueOptions
{
    public const string SectionKey = "SqsQueue";

    public string Name { get; set; } = string.Empty;
}
