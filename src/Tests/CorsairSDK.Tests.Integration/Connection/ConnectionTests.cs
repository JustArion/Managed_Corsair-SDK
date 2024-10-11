namespace CorsairSDK.Tests.Integration;

using Corsair;
using FluentAssertions;

[TestFixture(Author = "JustArion", Category = "Connection", Description = "Mixed external requirements. Minimal: iCUE is installed. Maximum: Has a Corsair Keyboard and iCUE is installed.")]
public class ConnectionTests
{
    [TearDown]
    public void Cleanup()
    {
        CorsairSDK._connectionHandler.Disconnect();
    }
    
    [Test(Description = "C# SDK should connect to iCUE")]
    public void ShouldConnect_ToSDK()
    {
        // Act
        var connected = CorsairSDK._connectionHandler.Connect();
        
        // Assert
        connected.Should().BeTrue();
    }
}