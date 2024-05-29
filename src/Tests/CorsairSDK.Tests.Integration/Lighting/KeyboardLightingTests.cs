namespace CorsairSDK.Tests.Integration.Lighting;

using System.Drawing;
using Corsair;
using Corsair.Lighting;
using Corsair.Lighting.Contracts;
using FluentAssertions;

[TestFixture(Category = "Lighting / Keyboard", Description = "Mixed external requirements. Minimal: iCUE is installed & has a Corsair Keyboard")]
public class KeyboardLightingTests
{
    
    private static IKeyboardLighting _lighting => CorsairSDK.KeyboardLighting;
    private static IKeyboardColorController _sut => _lighting.Colors;

    private static IEnumerable<KeyboardKey> _keys => _sut.KeyboardKeys;
    [TearDown]
    public void Cleanup()
    {
        _lighting.Shutdown();
    }
    
    [Test(Author = "JustArion", Description = "Keyboard lighting should be initialized for 1 sec")]
    public async Task ShouldInitialize_Lighting()
    {
        // Act
        var initialized = _lighting.TryInitialize();
        
        await Task.Delay(TimeSpan.FromSeconds(1));

        // Assert
        initialized.Should().BeTrue();
    }
    
    
    [Test(Author = "JustArion", Description = "All keys should light up Red for at least 1 second")]
    public async Task ShouldSet_GlobalKeyboardColor_Red()
    {
        // Arrange
        _lighting.TryInitialize().Should().BeTrue();

        // Act
        using (_sut.SetGlobal(Color.Red))
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            
            // Assert
            _sut.KeyboardKeys.Should().NotBeEmpty()
                .And.AllSatisfy(x => x.Color.Should().Be(Color.Red));
        }
    }
    
    [Test(Author = "JustArion", Description = "The enter key should turn Green for 1 second")]
    public async Task ShouldSet_SingleKey_Green()
    {
        // Arrange
        _lighting.TryInitialize().Should().BeTrue();

        // Act
        using (_sut.SetKeys(Color.Green, KeyboardKeys.ENTER))
        {
            await Task.Delay(TimeSpan.FromSeconds(1)); // The delay is to visually inspect the changed keys 

            // Assert
            _keys.Should().NotBeEmpty()
                .And.ContainSingle(x => x.Key == KeyboardKeys.ENTER)
                .And.AllSatisfy(x => x.Color.Should().Be(x.Key == KeyboardKeys.ENTER 
                            ? Color.Green 
                            : Color.Black));
        }
    }
    
    [Test(Author = "JustArion", Description = "The Q, W, E, R, T, and Y keys should turn Blue for 1 second")]
    public async Task ShouldSet_MultipleKeys_Blue()
    {
        // Arrange
        _lighting.TryInitialize().Should().BeTrue();
        KeyboardKeys[] expectedKeys = [KeyboardKeys.Q, KeyboardKeys.W, KeyboardKeys.E, KeyboardKeys.R, KeyboardKeys.T, KeyboardKeys.Y];

        // Act
        using (_sut.SetKeys(Color.Blue, expectedKeys))
        {
            await Task.Delay(TimeSpan.FromSeconds(1)); // The delay is to visually inspect the changed keys 

            
            // Assert
            _keys.Should().NotBeEmpty()
                .And.Subject.Select(x => x.Key)
                .Should().Contain(expectedKeys);
                
            _keys.Should().AllSatisfy(x => x.Color.Should().Be(expectedKeys.Contains(x.Key)
                ? Color.Blue
                : Color.Black));
            
        }
    }
    
    [Test(Author = "JustArion", Description = "The Arrow-Keys should turn Cyan for 1 second")]
    public async Task ShouldSet_Zone_Cyan()
    {
        // Arrange
        _lighting.TryInitialize().Should().BeTrue();
        var expectedKeys = ZoneUtility.GetKeysFromZones(KeyboardZones.ArrowKeys);

        // Act
        using (_sut.SetZones(Color.Cyan, KeyboardZones.ArrowKeys))
        {
            await Task.Delay(TimeSpan.FromSeconds(1)); // The delay is to visually inspect the changed keys                  
            
            // Assert
            _keys.Should().NotBeEmpty()
                .And.Subject.Select(x => x.Key)
                .Should().Contain(expectedKeys);
            
            _keys.Should().AllSatisfy(x => x.Color.Should().Be(expectedKeys.Contains(x.Key) 
                ? Color.Cyan 
                : Color.Black));
        }
    }
    
    [Test(Author = "JustArion", Description = "Multiple keyboard zones should be different colors")]
    public async Task ShouldSet_MultipleZones_VariousColors()
    {
        // Arrange
        _lighting.TryInitialize().Should().BeTrue();
        var expectedRedKeys = ZoneUtility.GetKeysFromZones(KeyboardZones.NumKeys | KeyboardZones.MediaKeys);
        var expectedOrangeKeys = ZoneUtility.GetKeysFromZones(KeyboardZones.PageKeys);
        var expectedGreenKeys = ZoneUtility.GetKeysFromZones(KeyboardZones.WASDKeys);
        var expectedIndigoKeys = ZoneUtility.GetKeysFromZones(KeyboardZones.MainZone | KeyboardZones.Logo)
            .Except(expectedGreenKeys) // The WASD keys are within the main-zone, it's a sub-zone / zone-within-a-zone
            .ToHashSet();

        // Act
        using (_sut.SetZones(Color.Red, KeyboardZones.NumKeys | KeyboardZones.MediaKeys))
        using (_sut.SetZones(Color.Orange, KeyboardZones.PageKeys)) 
        using (_sut.SetZones(Color.Indigo, KeyboardZones.MainZone | KeyboardZones.Logo)) 
        using (_sut.SetZones(Color.Green, KeyboardZones.WASDKeys))
        {
            await Task.Delay(TimeSpan.FromSeconds(1)); // The delay is to visually inspect the changed keys

            // Assert
            _keys.Should().NotBeEmpty()
                .And.Subject.Select(x => x.Key)
                .Should().Contain(expectedRedKeys)
                .And.Contain(expectedOrangeKeys)
                .And.Contain(expectedIndigoKeys)
                .And.Contain(expectedGreenKeys);
            
            _keys.Should().AllSatisfy(x =>
                {
                    if (expectedRedKeys.Contains(x.Key)) 
                        x.Color.Should().Be(Color.Red);
                    
                    else if (expectedOrangeKeys.Contains(x.Key))
                        x.Color.Should().Be(Color.Orange);
                    
                    else if (expectedIndigoKeys.Contains(x.Key))
                        x.Color.Should().Be(Color.Indigo);
                    
                    else if (expectedGreenKeys.Contains(x.Key))
                        x.Color.Should().Be(Color.Green);
                });
        }
    }
    
    [Test(Author = "JustArion", Description = "A color is globally set, some keys, some keys are cleared")]
    public async Task ShouldClear_MultipleKeys_FromGlobal()
    {
        // Arrange
        _lighting.TryInitialize().Should().BeTrue();
        var expectedKeys = ZoneUtility.GetKeysFromZones(KeyboardZones.MainZone);

        using (_sut.SetGlobal(Color.Red))
        {
            
            // Act
            _sut.ClearKeys(expectedKeys);            
            await Task.Delay(TimeSpan.FromSeconds(1)); // The delay is to visually inspect the changed keys
            
            // Assert
            _keys.Should().NotBeEmpty().And.Subject.Select(x => x.Key).Should().Contain(expectedKeys);
            _keys.Should().AllSatisfy(x => x.Color.Should().Be(expectedKeys.Contains(x.Key) ? Color.Black : Color.Red));
        }
    }
}