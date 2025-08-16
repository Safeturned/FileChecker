using dnlib.DotNet;

namespace Safeturned.FileChecker;

public static class Checker
{
    public static void Process(Stream stream)
    {
        var module = ModuleDefMD.Load(stream);
    }
}