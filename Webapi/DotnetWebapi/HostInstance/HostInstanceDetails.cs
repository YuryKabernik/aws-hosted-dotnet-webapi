using Amazon.Util;

namespace dotnet_intermediate_mentoring_program.HostInstance;

public record InstanceResidenceDetails(string Region, string AvailabilityZone);

public static class HostInstanceDetails
{
    public static RouteHandlerBuilder MapHostInstanceDetails(this WebApplication app) => app
        .MapGet("/instance/current", GetCurrentInstanceDetails)
        .WithName("GetInstanceLocation")
        .WithOpenApi();

    private static InstanceResidenceDetails GetCurrentInstanceDetails() => new(
        EC2InstanceMetadata.Region?.DisplayName ?? "Not Available",
        EC2InstanceMetadata.AvailabilityZone ?? "Not Available"
    );
}
