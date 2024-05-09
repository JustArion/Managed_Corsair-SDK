namespace CorsairSDK.Tests.Integration.Devices;

using Corsair.Device;
using Corsair.Device.Devices;
using Corsair;
using FluentAssertions;

[TestFixture(Category = "Device", Description = "Strict external requirements. Minimal: iCUE is installed and has at least 1 Corsair Headset connected.", 
    IgnoreReason = "Device Specific Test (Headset)")]
public class HeadsetTests
{
    [Test(Author = "JustArion", Description = "Device of type 'Headset' should be up-castable to the <Headset> class")]
    public void Headsets_ShouldHave_DeviceSpecific_Information()
    {
        // Act
        var device = CorsairSDK.GetDevices(DeviceType.Headset).FirstOrDefault();
        
        var headset = device!.AsDevice<Headset>();
        
        // Assert
        device.Should().NotBeNull();
        headset.BatteryLevel.Should().BeInRange(0, 100);
        headset.EqualizerPreset.Should().BeInRange(0, 5);
    }
}