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
        using (CorsairMarshal.StringToAnsiPointer("{TestString1234}", out var ptr))
        {
            var str = new string(ptr);
            
            // Assert
            str.Should().Be("{TestString1234}");
        }
    }

    [Test]
    public void NativeStringArrays_ShouldConvert_ToManagedStringArrays()
    {
        // Arrange
        using (CorsairMarshal.StringToAnsiPointer("{TestElement1}", out var element1Ptr))
        using (CorsairMarshal.StringToAnsiPointer("{TestElement2}", out var element2Ptr))
        using (CorsairMarshal.StringToAnsiPointer("{TestElement3}", out var element3Ptr))
        using (CorsairMarshal.StringToAnsiPointer("{TestElement4}", out var element4Ptr))
        using (CorsairMarshal.StringToAnsiPointer("{TestElement5}", out var element5Ptr))
        {           
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
}