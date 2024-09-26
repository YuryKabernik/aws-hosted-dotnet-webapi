using System.Net;
using AsyncMessaging.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_intermediate_mentoring_program.Endpoints.Subscriptions
{
    [Route("subscriptions")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly EmailNotificationService notificationService;
        private readonly ILogger<SubscriptionsController> logger;

        public SubscriptionsController(
            EmailNotificationService notificationService,
            ILogger<SubscriptionsController> logger)
        {
            this.notificationService = notificationService;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> Subscribe(string email, CancellationToken cancellationToken)
        {
            try
            {
                await this.notificationService.SubscribeAsync(email, cancellationToken);

                return Ok();
            }
            catch (System.Exception ex)
            {
                logger.LogCritical(ex, "Unable to subscribe a user by '{email}' email.", email);

                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Unsubscribe(string email, CancellationToken cancellationToken)
        {
            try
            {
                await this.notificationService.UnsubscribeAsync(email, cancellationToken);

                return this.NoContent();
            }
            catch (System.Exception ex)
            {
                logger.LogCritical(ex, "Unable to unsubscribe a user by '{email}' email.", email);

                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
