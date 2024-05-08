namespace CorsairSDK.Tests.Unit.Devices;

using Corsair.Device;
using Corsair;
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
    public void ShouldList_Devices()
    {
        // Act
        var corsairDevices = CorsairSDK.GetDevices().ToArray();
        
        // Assert
        corsairDevices.Should().NotBeEmpty();
    }

    
    [Test(Author = "JustArion", Description = "Corsair devices should contain proper info")]
    public void Devices_ShouldHave_Information()
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
    
    [Test(Author = "JustArion", Description = "Corsair devices should contain proper info")]
    public void Devices_ShouldHave_ValidIds()
    {
        // Act
        var corsairDevices = CorsairSDK.GetDevices().ToArray();
        
        foreach (var device in corsairDevices)
        {
            var id = device.Id;
            // Assert
            id.Should().NotBeNullOrWhiteSpace();

            id.Should().StartWith("{");
            id.Should().EndWith("}");
        }
    }
}