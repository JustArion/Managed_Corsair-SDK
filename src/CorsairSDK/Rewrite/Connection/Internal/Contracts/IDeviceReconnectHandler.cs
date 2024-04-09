namespace Dawn.CorsairSDK.Rewrite.Connection.Internal.Contracts;

internal interface IDeviceReconnectHandler
{
    DeviceReconnectPolicy ReconnectPolicy { get; set; }

    // Task RequestReconnection(Func<bool> reconnectionAttempt);
}
