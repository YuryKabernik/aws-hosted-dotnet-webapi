using AsyncMessaging.Notifications;
using Riok.Mapperly.Abstractions;
using Storage.DbModels;

namespace UseCases.AutoMappers;

[Mapper]
public static partial class ImageMetadataMapper
{
    public static partial ImageUploadNotificationMetadata ToNotify(this ImageMetadata image);
}
