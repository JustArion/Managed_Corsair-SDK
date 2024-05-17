namespace CorsairSDK.Tests.Unit;

using Corsair.Device.Internal;
using FluentAssertions;

[TestFixture(Author = "JustArion", Category = "Interop Helpers")]
public unsafe class InteropHelperTests
{
    [Test]
    public void Strings_ShouldBeMarshalled_ToPointers()
    {
        // Act
        var ptr = CorsairMarshal.ToPointer("{TestString1234}");
        var str = new string(ptr);
        
        // Assert
        str.Should().Be("{TestString1234}");
    }
    
    [Test]
    public void References_ShouldWithstand_GC()
    {
        // Act
        var ptr = CorsairMarshal.ToPointer("{TestString1234}");
        GC.AddMemoryPressure((long)Math.Pow(1024, 4) * 4); // 4mb
        GC.Collect();
        GC.WaitForPendingFinalizers();
        var str = new string(ptr);
        
        // Assert
        str.Should().Be("{TestString1234}");
    }

    [Test]
    public void NativeStringArrays_ShouldConvert_ToManagedStringArrays()
    {
        // Arrange
        sbyte*[] array = 
        [ 
            CorsairMarshal.ToPointer("{TestElement1}"), 
            CorsairMarshal.ToPointer("{TestElement2}"), 
            CorsairMarshal.ToPointer("{TestElement3}"), 
            CorsairMarshal.ToPointer("{TestElement4}"), 
            CorsairMarshal.ToPointer("{TestElement5}")
        ];
        
        // Act
        string[] strings;
        fixed (sbyte** ptr = array)
            strings = CorsairMarshal.ToArray(ptr, 5);

        // Assert
        strings.Length.Should().Be(5);
        for (var i = 0; i < 5; i++) 
            strings[i].Should().Be($"{{TestElement{i + 1}}}");
    }
}