namespace Dawn.Apps.RemoteControlHost.Services;

using CorsairSDK.Rewrite;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mapping;

public class CorsairService(PlatformService platformService, ILogger<CorsairService> logger) : Corsair.CorsairBase
{
    public override async Task GetAllKeyboardKeys(Empty request, IServerStreamWriter<KeyboardState> responseStream, ServerCallContext context)
    {
        logger.LogInformation("GetAllKeyboardKeys");

        var keys = platformService.Colors.KeyboardKeys;

        foreach (var key in keys) 
            await responseStream.WriteAsync(key.MapToKeyboardKeyMessage());
    }

    public override Task<Empty> SetGlobal(Color request, ServerCallContext context)
    {
        logger.LogInformation("SetGlobal - {Color}", request);

        platformService.Colors.SetGlobal(request.MapToColor());

        return Task.FromResult(new Empty());
    }

    public override Task<Empty> ClearAll(Empty request, ServerCallContext context)
    {
        logger.LogInformation("ClearAll");

        platformService.Colors.ClearAll();

        return Task.FromResult(new Empty());
    }

    public override async Task<Empty> SetKeys(IAsyncStreamReader<SetKeyOptions> requestStream, ServerCallContext context)
    {
        logger.LogInformation("SetKeys");
        
        await foreach (var keyMessage in requestStream.ReadAllAsync())
        {
            var keys = keyMessage.Key.Select(x => (KeyboardKeys)x.Id).ToArray();
            logger.LogInformation("SetKeys - {Color} - {Key}", keyMessage.Color, string.Join(", ", keys));
            platformService.Colors.SetKeys(keyMessage.Color.MapToColor(), keys);
        }

        return new Empty();
    }

    public override async Task<Empty> ClearKeys(IAsyncStreamReader<Dawn.Apps.RemoteControlHost.KeyboardKeys> requestStream, ServerCallContext context)
    {
        logger.LogInformation("ClearKeys");

        await foreach (var key in requestStream.ReadAllAsync())
        {
            logger.LogInformation("ClearKeys - {Key}", key);
            platformService.Colors.ClearKeys((KeyboardKeys)key.Id);
        }

        return new Empty();
    }

    public override async Task<Empty> SetZones(IAsyncStreamReader<SetZoneOptions> requestStream, ServerCallContext context)
    {
        logger.LogInformation("SetZones");
        
        await foreach (var zoneOptions in requestStream.ReadAllAsync())
        {
            var zone = (KeyboardZones)zoneOptions.Zone.Id;
            logger.LogInformation("SetZones - {Color} - {Zone}", zoneOptions.Color, zone);
            
            platformService.Colors.SetZones(zoneOptions.Color.MapToColor(), zone);
        }

        return new Empty();
    }

    public override Task<Empty> ClearZones(Dawn.Apps.RemoteControlHost.KeyboardZones request, ServerCallContext context)
    {
        var zones = (KeyboardZones)request.Id;
        
        logger.LogInformation("ClearZones - {Zones}", zones);

        platformService.Colors.ClearZones(zones);

        return Task.FromResult(new Empty());
    }
}