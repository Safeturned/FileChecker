using ST.Checking.DnLib.Abstraction;
namespace ST.Checking.DnLib;
public class BlackListedCommand(string command, float score) : IBlackListedCommand
{
    public string Value { get; } = command;
    public float  Score { get; } = score;
}