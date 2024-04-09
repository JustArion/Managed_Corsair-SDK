namespace CorsairSDK.Tests.Unit;

using Dawn.CorsairSDK.Rewrite.Device;
using Dawn.Rewrite;
using FluentAssertions;

[TestFixture]
public class ConnectionTests
{
    [Test(Author = "JustArion", Description = "The Test environment should have iCUE installed")]
    public void Should_ConnectToSDK()
    {
        CorsairSDK._connectionHandler.Connect()
            .Should().BeTrue();
        
        CorsairSDK._connectionHandler.Disconnect();
    }

    [Test(Author = "JustArion", Description = "The Test environment should have iCUE installed & has a Corsair Keyboard")]
    public async Task Should_InitializeLighting()
    {
        CorsairSDK.KeyboardLighting.TryInitialize()
            .Should().BeTrue();

        await Task.Delay(TimeSpan.FromSeconds(1));
        
        CorsairSDK.KeyboardLighting.Shutdown();
    }

    [Test(Author = "JustArion", Description = "The Test environment should have iCUE installed & has at least 1 Corsair Device")]
    public void Should_ListDevices()
    {
        // Act
        var corsairDevices = CorsairSDK.GetDevices().ToArray();
        
        // Assert
        corsairDevices.Should().NotBeEmpty();
    }

    
    [Test(Author = "JustArion", Description = "The Test environment should have iCUE installed & has at least 1 Corsair Device")]
    public void Devices_Should_HaveInformation()
    {
        // Act
        var corsairDevices = CorsairSDK.GetDevices().ToArray();
        
        // Assert
        foreach (var device in corsairDevices)
        {
            device.Id
                .Should().NotBeNullOrWhiteSpace();
            
            device.Model
                .Should().NotBeNullOrWhiteSpace();
            
            device.Serial
                .Should().NotBeNull();

            device.Type
                .Should().NotBe(DeviceType.All);
            
            device.Type
                .Should().NotBe(DeviceType.None);

            device.LedCount.Should().BeGreaterThan(0);
        }
    }
}