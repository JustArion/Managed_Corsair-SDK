namespace Dawn.CorsairSDK;

internal static class CommonPaths
{
    private static readonly string MainDriveLetter = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System))!;

    internal static readonly string ProgramFiles_Corsair_ICUE = Path.Join(MainDriveLetter, "Program Files", "Corsair", "Corsair iCUE5 Software", "iCUE.exe");
    
    internal static readonly string ProgramFiles86_Corsair_CUE = Path.Join(MainDriveLetter, "Program File x86", "Corsair", "Corsair iCUE5 Software", "CUE.exe");
}