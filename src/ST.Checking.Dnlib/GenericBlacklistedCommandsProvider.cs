using DI.Services.Scheme.Abstraction;
using DI.Services.Scheme.Attributes;
using ST.Checking.DnLib.Abstraction;
using System.Collections.Generic;
namespace ST.Checking.DnLib;
[DiDescript(Order = -5, Lifetime = EDiServiceLifetime.Singleton, ServiceType = typeof(IBlacklistedCommandsProvider),
Key = "GenericBlacklistedCommandsProvider")]
public class GenericBlacklistedCommandsProvider : IBlacklistedCommandsProvider
{
    private readonly IEnumerable<IBlackListedCommand> m_BlacklistedCommands =
        [new BlackListedCommand("admin", 50), new BlackListedCommand("shutdown", 50), new BlackListedCommand("ban", 20)];
    public IEnumerable<IBlackListedCommand> GetBlacklistedCommands() => m_BlacklistedCommands;
}