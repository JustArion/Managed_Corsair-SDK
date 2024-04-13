namespace CorsairSDK.Tests.Unit.Devices;

using Dawn.CorsairSDK.Rewrite.Device;
using Dawn.CorsairSDK.Rewrite.Device.Devices;
using Dawn.Rewrite;
using FluentAssertions;

[TestFixture(Category = "Device", Description = "Mixed external requirements. Minimal: iCUE is installed. Maximum: Has 1 Corsair device connected.")]
public class DeviceTests
{
    
    [TearDown]
    public void Cleanup()
    {
        CorsairSDK.KeyboardLighting.Shutdown();
    }
    
    [Test(Author = "JustArion", Description = "Corsair devices should be listed")]
    public void Should_ListDevices()
    {
        // Act
        var corsairDevices = CorsairSDK.GetDevices().ToArray();
        
        // Assert
        corsairDevices.Should().NotBeEmpty();
    }

    
    [Test(Author = "JustArion", Description = "Corsair devices should contain proper info")]
    public void Devices_Should_HaveInformation()
    {
        // Act
        var corsairDevices = CorsairSDK.GetDevices().ToArray();
        
        foreach (var device in corsairDevices)
        {
            // Assert
            device.Id.Should().NotBeNullOrWhiteSpace();
            device.Model.Should().NotBeNullOrWhiteSpace();
            device.Serial.Should().NotBeNullOrWhiteSpace();
            device.Type.Should().NotBe(DeviceType.All)
                .And.NotBe(DeviceType.None);
            device.LedCount.Should().BeGreaterThan(0);
        }
    }
    
    [Test(Author = "JustArion", Description = "Devices of type 'Headset' should be up-castable to the <Headset> class")]
    public void Headsets_ShouldHave_HeadsetSpecific_Information()
    {
        // Act
        var device = CorsairSDK.GetDevices(DeviceType.Headset).FirstOrDefault();
        
        var headset = device!.AsDevice<Headset>();
        
        // Assert
        device.Should().NotBeNull();
        headset.BatteryLevel.Should().BeInRange(0, 100);
    }
}