using System.Diagnostics;
using Corsair.Bindings;

namespace Corsair.Tracing;

internal static class InteropTracing
{
    private const string KEY = "Interop";

    [Conditional("DEBUG")]
    internal static void DebugTrace(CorsairError result, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = default)
        => Debug.WriteLine($"From [{memberName}:{lineNumber}] with result [{result}]", KEY);

    [Conditional("DEBUG")]
    internal static void DebugTrace<TParam>(CorsairError result, TParam param, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = default)
        => Debug.WriteLine($"From [{memberName}:{lineNumber}] with parameter {param} with result [{result}]", KEY);

    [Conditional("DEBUG")]
    internal static void DebugTrace<TParam1, TParam2>(CorsairError result, TParam1 param1,
        TParam2 param2, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = default) =>
        Debug.WriteLine($"From [{memberName}:{lineNumber}] with parameters {param1}, {param2} with result [{result}]", KEY);

    [Conditional("DEBUG")]
    internal static void DebugTrace<TParam1, TParam2, TParam3>(CorsairError result, TParam1 param1,
        TParam2 param2, TParam3 param3, [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = default) =>
        Debug.WriteLine($"From [{memberName}:{lineNumber}] with parameters {param1}, {param2}, {param3} with result [{result}]", KEY);

    [Conditional("DEBUG")]
    internal static void DebugTrace<TParam1, TParam2, TParam3, TParam4>(CorsairError result,
        TParam1 param1,
        TParam2 param2, TParam3 param3, TParam4 param4, [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = default) =>
        Debug.WriteLine($"From [{memberName}:{lineNumber}] with parameters {param1}, {param2}, {param3}, {param4} with result [{result}]", KEY);
}
