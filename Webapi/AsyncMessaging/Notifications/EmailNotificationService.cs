using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AsyncMessaging.Options;
using Microsoft.Extensions.Options;

namespace AsyncMessaging.Notifications;

public class EmailNotificationService(
    IAmazonSimpleNotificationService notificationService,
    IOptions<SnsTopicOptions> options)
{
    private readonly SnsTopicOptions _snsTopic = options.Value;

    /// <summary>
    /// After a user visits the subscription endpoint,
    /// the specified email should receive a confirmation message.
    /// </summary>
    /// <returns></returns>
    public async Task SubscribeAsync(string email, CancellationToken cancellationToken)
    {
        string topicArn = await this.GetTopicArnAsync(cancellationToken);
        string protocol = "email";

        var request = new SubscribeRequest(topicArn, protocol, email);
        var response = await notificationService.SubscribeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Whenever a user visits the unsubscription endpoint,
    /// AWS should stop sending notifications to the specified email.
    /// </summary>
    /// <returns></returns>
    public async Task UnsubscribeAsync(string email, CancellationToken cancellationToken)
    {
        string topicArn = await this.GetTopicArnAsync(cancellationToken);
        var topicResponse = await notificationService.ListSubscriptionsByTopicAsync(topicArn, cancellationToken);

        Subscription subscription = topicResponse.Subscriptions
            .Single(s => s.Endpoint.Equals(email, StringComparison.InvariantCultureIgnoreCase));

        UnsubscribeRequest request = new UnsubscribeRequest(subscription.SubscriptionArn);
        UnsubscribeResponse unsubscribeResponse =
            await notificationService.UnsubscribeAsync(request, cancellationToken);
    }

    public async Task PushNotificationAsync(EmailNotificationMessage message, CancellationToken cancellationToken)
    {
        string topicArn = await this.GetTopicArnAsync(cancellationToken);
        string emailMessage = this.BuildEmailMessage(message);

        PublishResponse response = await notificationService.PublishAsync(topicArn, emailMessage, cancellationToken);
    }

    private string BuildEmailMessage(EmailNotificationMessage message)
    {
        Uri link = message.ApplicationEndpoint;
        var metadata = message.Metadata;

        return $"""
                    Please be informed that a new image was uploaded to the bucket.
                     - Name: {metadata.Name}
                     - Extension: {metadata.Extension}
                     - Size: {metadata.Size}
                     - Last Modified: {metadata.LastUpdate}
                    
                    Image could be downloaded by the link: {link}.
                """;
    }

    private async Task<string> GetTopicArnAsync(CancellationToken cancellationToken)
    {
        if (this._snsTopic.ArnName is not null)
        {
            return this._snsTopic.ArnName;
        }

        return await this.GetOrCreateTopic(cancellationToken);
    }

    private async Task<string> GetOrCreateTopic(CancellationToken cancellationToken)
    {
        CreateTopicRequest request = new CreateTopicRequest(this._snsTopic.Name);
        CreateTopicResponse response = await notificationService.CreateTopicAsync(request, cancellationToken);

        return response.TopicArn;
    }
}