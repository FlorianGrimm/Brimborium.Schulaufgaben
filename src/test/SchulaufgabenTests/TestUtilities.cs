// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben;

public static class TestUtilities {
    public static string GetProjectFolder() {
        return System.IO.Path.GetDirectoryName(GetCallerFilePath())!;
        static string GetCallerFilePath([System.Runtime.CompilerServices.CallerFilePath] string CallerFilePath = "") {
            return CallerFilePath;
        }
    }
}