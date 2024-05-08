namespace CorsairSDK.Tests.Unit.Devices;

using Dawn.CorsairSDK.Rewrite.Device;
using Dawn.CorsairSDK.Rewrite.Device.Devices;
using Dawn.Rewrite;
using FluentAssertions;

[TestFixture(Category = "Device", Description = "Strict external requirements. Minimal: iCUE is installed and has at least 1 Corsair HeadsetStand connected.", 
    IgnoreReason = "Device Specific Test (HeadsetStand)")]
public class HeadsetStandTests
{
    [Test(Author = "JustArion", Description = "Devices of type 'Headset' should be up-castable to the <Headset> class")]
    public void Headsets_ShouldHave_HeadsetSpecific_Information()
    {
        // Act
        var device = CorsairSDK.GetDevices(DeviceType.HeadsetStand).FirstOrDefault();
        
        var headsetStand = device!.AsDevice<HeadsetStand>();
        
        // Assert
        device.Should().NotBeNull();
        headsetStand.EqualizerPreset.Should().BeInRange(0, 5);
    }
}