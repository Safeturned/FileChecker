using System.Collections.Generic;
namespace ST.Checking.DnLib.Abstraction;
public interface IBlacklistedCommandsProvider
{
    IEnumerable<IBlackListedCommand> GetBlacklistedCommands();
}
public interface IBlackListedCommand
{
    string Value { get; }
    float  Score { get; }
}