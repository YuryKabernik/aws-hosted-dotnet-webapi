using System.Runtime.CompilerServices;
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using AsyncMessaging.Notifications;
using AsyncMessaging.Options;
using Microsoft.Extensions.Options;

namespace AsyncMessaging;

public class QueueService(IAmazonSQS queue, IOptions<QueueOptions> options)
{
    public async IAsyncEnumerable<EmailNotificationMessage> Read(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string url = await this.GetQueueUrl(cancellationToken);
        var response = await queue.ReceiveMessageAsync(url, cancellationToken);

        foreach (var message in response.Messages)
        {
            EmailNotificationMessage data = JsonSerializer.Deserialize<EmailNotificationMessage>(
                message.Body,
                JsonSerializerOptions.Default)!;

            yield return data;
        }

        List<DeleteMessageBatchRequestEntry> deleteEntries = response.Messages
            .Select(m => new DeleteMessageBatchRequestEntry(m.MessageId, m.ReceiptHandle))
            .ToList();
        
        await queue.DeleteMessageBatchAsync(url, deleteEntries, cancellationToken);
    }

    public async Task Send(EmailNotificationMessage message, CancellationToken cancellationToken)
    {
        string url = await this.GetQueueUrl(cancellationToken);
        string json = JsonSerializer.Serialize(message, JsonSerializerOptions.Default);

        await queue.SendMessageAsync(url, json, cancellationToken);
    }

    private async Task<string> GetQueueUrl(CancellationToken cancellationToken)
    {
        var response = await queue.GetQueueUrlAsync(options.Value.Name, cancellationToken);

        return response.QueueUrl;
    }
}