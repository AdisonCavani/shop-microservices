using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using OpenTelemetry;

namespace ServiceDefaults;

public sealed class OtelTracingProcessor : BaseProcessor<Activity>
{
    public override void OnEnd(Activity activity)
    {
        if (activity.DisplayName != "grpc.health.v1.Health/Check")
            return;
        
        var statusCode = activity.Tags.FirstOrDefault(t => t.Key == "rpc.grpc.status_code").Value;

        if (statusCode != "0")
            return;
        
        activity.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;
    }
}