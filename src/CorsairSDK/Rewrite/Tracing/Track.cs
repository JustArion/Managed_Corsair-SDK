using System.Diagnostics;
using Dawn.CorsairSDK.Bindings;

namespace Dawn.CorsairSDK.Rewrite.Tracing;

internal static class Track
{

    internal static CorsairError Interop(CorsairError result, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = default)
    {

        Debug.WriteLine($"From [{memberName}:{lineNumber}] with result [{result}]", "Interop Call");

        return result;
    }
    internal static CorsairError Interop<TParam>(CorsairError result, TParam param, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = default)
    {

        Debug.WriteLine($"From [{memberName}:{lineNumber}] with parameter {param} with result [{result}]", "Interop Call");

        return result;
    }

    internal static CorsairError Interop<TParam1, TParam2>(CorsairError result, TParam1 param1,
        TParam2 param2, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = default)
    {

        Debug.WriteLine($"From [{memberName}:{lineNumber}] with parameters {param1}, {param2} with result [{result}]", "Interop Call");

        return result;
    }

    internal static CorsairError Interop<TParam1, TParam2, TParam3>(CorsairError result, TParam1 param1,
        TParam2 param2, TParam3 param3, [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = default)
    {

        Debug.WriteLine($"From [{memberName}:{lineNumber}] with parameters {param1}, {param2}, {param3} with result [{result}]", "Interop Call");

        return result;
    }

    internal static CorsairError Interop<TParam1, TParam2, TParam3, TParam4>(CorsairError result,
        TParam1 param1,
        TParam2 param2, TParam3 param3, TParam4 param4, [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = default)
    {

        Debug.WriteLine($"From [{memberName}:{lineNumber}] with parameters {param1}, {param2}, {param3}, {param4} with result [{result}]", "Interop Call");

        return result;
    }


}
