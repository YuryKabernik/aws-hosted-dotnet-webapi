namespace AsyncMessaging.Options;

public class SnsTopicOptions
{
    public const string SectionKey = "SnsTopic";
    public const string EnvironmentArnKey = "SNS_TOPIC_ARN";

    public string? Name { get; set; }

    public string? ArnName { get; set; }
}
