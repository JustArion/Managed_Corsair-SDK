namespace CorsairSDK.Tests.Unit;

using System.Runtime.InteropServices;
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
        GC.AddMemoryPressure(4096);
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
        var element1Ptr = CorsairMarshal.ToPointer("{TestElement1}");
        var element2Ptr = CorsairMarshal.ToPointer("{TestElement2}");
        var element3Ptr = CorsairMarshal.ToPointer("{TestElement3}");
        var element4Ptr = CorsairMarshal.ToPointer("{TestElement4}");
        var element5Ptr = CorsairMarshal.ToPointer("{TestElement5}");
        
        // Act
        var memory = (sbyte**)NativeMemory.AllocZeroed(5, (nuint)IntPtr.Size);
        memory[0] = element1Ptr;
        memory[1] = element2Ptr;
        memory[2] = element3Ptr;
        memory[3] = element4Ptr;
        memory[4] = element5Ptr;
        
        var strings = CorsairMarshal.ToArray(memory, 5);
        
        // Assert
        strings.Length.Should().Be(5);
        strings[0].Should().Be("{TestElement1}");
        strings[1].Should().Be("{TestElement2}");
        strings[2].Should().Be("{TestElement3}");
        strings[3].Should().Be("{TestElement4}");
        strings[4].Should().Be("{TestElement5}");
        
        NativeMemory.Free(memory);
    }
}