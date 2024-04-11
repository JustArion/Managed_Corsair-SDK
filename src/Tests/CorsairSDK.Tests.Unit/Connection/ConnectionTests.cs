namespace CorsairSDK.Tests.Unit;

using Dawn.CorsairSDK.Rewrite.Device;
using Dawn.Rewrite;
using FluentAssertions;

[TestFixture(Category = "Connection", Description = "Mixed external requirements. Minimal: iCUE is installed. Maximum: Has a Corsair Keyboard and iCUE is installed.")]
public class ConnectionTests
{
    [TearDown]
    public void Cleanup()
    {
        CorsairSDK._connectionHandler.Disconnect();
    }
    
    [Test(Author = "JustArion", Description = "C# SDK should connect to iCUE")]
    public void Should_ConnectToSDK()
    {
        // Act
        var connected = CorsairSDK._connectionHandler.Connect();
        
        // Assert
        connected.Should().BeTrue();
    }
}