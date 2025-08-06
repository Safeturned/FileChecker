using DI.Engine.Attributes;
using DI.Services.Abstraction;
using DI.Services.Scheme.Abstraction;
using DI.Services.Scheme.Attributes;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ST.Checking.Abstraction;
using ST.Checking.DnLib.Abstraction;
using ST.CheckingProcessor.DnLib;
using System.Collections.Generic;
using System.Linq;
namespace ST.Checking.DnLib;
[DiDescript(Order = 0, Lifetime = EDiServiceLifetime.Singleton, ServiceType = typeof(IModuleChecker), Key = "DnLibRocketCommandsModuleChecker")]
public class DnLibRocketCommandsModuleChecker(
    [DiParameter(IgnoreKeys = true, ServiceEquals = EDiServiceEquals.SubClassOrAssignableOrEquals)]
    IEnumerable<IBlacklistedCommandsProvider> blacklistedCommands) : BaseModuleChecker<IDnLibModuleProcessingContext>
{
    protected virtual string LoweredRocketCommandManagerCommands { get; } = "Rocket.Core.Commands.RocketCommandManager Rocket.Core.R::Commands".ToLower();
    protected virtual string LoweredConsolePlayer                { get; } = "System.Void Rocket.API.ConsolePlayer::.ctor()".ToLower();
    protected override void InternalCheck(IDnLibModuleProcessingContext context)
    {
        foreach(TypeDef typeDef in context.Module.GetTypes())
        foreach(MethodDef typeDefMethod in typeDef.Methods)
        {
            if(!typeDefMethod.HasBody) continue;

            for(int i = 0; i < typeDefMethod.Body.Instructions.Count; i++)
            {
                Instruction? instruction = typeDefMethod.Body.Instructions[i];
                if(instruction.OpCode != OpCodes.Ldsfld || instruction.Operand.ToString()?.ToLower() != LoweredRocketCommandManagerCommands) continue;

                instruction = typeDefMethod.Body.Instructions[i + 1];
                if(instruction.OpCode != OpCodes.Newobj || instruction.Operand.ToString()?.ToLower() != LoweredConsolePlayer) continue;

                instruction = typeDefMethod.Body.Instructions[i + 2];
                if(instruction.OpCode != OpCodes.Ldstr) continue;

                foreach(IBlackListedCommand? blacklistedCommand in blacklistedCommands.SelectMany(x => x.GetBlacklistedCommands()))
                    if(instruction.Operand.ToString()!.ToLower().Contains(blacklistedCommand.Value))
                    {
                        context.Score   += blacklistedCommand.Score;
                        context.Message += $"Blacklisted command {blacklistedCommand.Value} found in {typeDefMethod.FullName}";
                    }
            }
        }
    }
}