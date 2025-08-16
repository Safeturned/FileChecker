using dnlib.DotNet.Emit;
using Safeturned.FileChecker.Modules;

namespace Safeturned.FileChecker.Analyzers;

internal class BlacklistedCommandAnalyzer : IModuleAnalyzer
{
    private static readonly string LoweredRocketCommandManagerCommands = "Rocket.Core.Commands.RocketCommandManager Rocket.Core.R::Commands".ToLower();
    private static readonly string LoweredConsolePlayer = "System.Void Rocket.API.ConsolePlayer::.ctor()".ToLower();
    private static readonly IReadOnlyList<(string Command, float Score)> BlacklistedCommands =
    [
        ("admin", 50),
        ("shutdown", 50),
        ("ban", 20),
    ];

    public void Analyze(ModuleProcessingContext context)
    {
        foreach (var typeDef in context.Module.GetTypes())
        foreach (var typeDefMethod in typeDef.Methods)
        {
            if (!typeDefMethod.HasBody)
                continue;
            for (var i = 0; i < typeDefMethod.Body.Instructions.Count; i++)
            {
                var instruction = typeDefMethod.Body.Instructions[i];
                if (instruction.OpCode != OpCodes.Ldsfld || instruction.Operand.ToString()?.ToLower() != LoweredRocketCommandManagerCommands)
                    continue;
                instruction = typeDefMethod.Body.Instructions[i + 1];
                if (instruction.OpCode != OpCodes.Newobj || instruction.Operand.ToString()?.ToLower() != LoweredConsolePlayer)
                    continue;
                instruction = typeDefMethod.Body.Instructions[i + 2];
                if (instruction.OpCode != OpCodes.Ldstr)
                    continue;
                foreach (var (command, score) in BlacklistedCommands)
                {
                    if (instruction.Operand.ToString()!.Contains(command, StringComparison.CurrentCultureIgnoreCase))
                    {
                        context.Score += score;
                        context.Message += $"Blacklisted command {command} found in {typeDefMethod.FullName}";
                    }
                }
            }
        }
    }
}