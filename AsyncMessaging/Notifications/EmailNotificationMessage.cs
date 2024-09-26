namespace AsyncMessaging.Notifications;

// an explanation that an image has been uploaded
public record EmailNotificationMessage
(
    // the image metadata (size, name, extension)
    ImageUploadNotificationMetadata Metadata,

    // a link to the web application endpoint for downloading the image
    Uri ApplicationEndpoint
);
