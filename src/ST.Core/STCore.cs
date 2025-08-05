using DI.Engine.Template;
using DI.Service.Provide.Extensions;
using DI.Services.Abstraction;
using DI.Services.Factory;
using DI.Services.Provide;
using DI.Services.Scheme.Factory;
using DI.Services.Scheme.Read;
using DI.Services.Scheme.Read.Abstraction;
using DI.Services.Scheme.Read.Validation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
internal sealed class StCore
{
    public static void Main(string[] args)
    {
        IDiSchemeReader reader = InternalCreateReader();
        foreach(string file in Directory.GetFiles(Directory.GetCurrentDirectory(), "ST.*.dll", SearchOption.TopDirectoryOnly))
            reader.Read(Assembly.LoadFrom(file));
        reader.Read(Assembly.GetExecutingAssembly());
        Initialize(InternalCreateProvider(reader));
    }
    private static void Initialize(IDiServiceProvider provider)
    {
        IEnumerable<IStInitializable> services = provider.ResolveServices<IStInitializable>(null, EDiServiceEquals.SubClassOrAssignableOrEquals, true)
            .Cast<IStInitializable>();
        foreach(IStInitializable intializable in services) intializable.Initialize();
    }
    private static IDiSchemeReader InternalCreateReader()
    {
        DiSchemeFactory               schemeFactory = new();
        DiServiceReadContextValidator validator     = new();
        return new DiSchemeReader(schemeFactory, validator);
    }
    private static IDiServiceProvider InternalCreateProvider(IDiSchemeReader reader)
    {
        DiEngineTemplate engine         = new(false);
        DiServiceFactory serviceFactory = new(engine);
        return new DiServiceProvider(serviceFactory, reader.EndRead());
    }
}