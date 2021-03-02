using System;
using System.Collections.Generic;
using System.Text;

namespace Hades.Core.Abbstraction.ServiceDiscovery
{
    public interface IServiceDiscovery
    {
    }

    public interface IServiceDefinition
    {
        string Name { get; }
        IEnumerable<IMethodDefinition> Methods { get; }
    }
    public interface IMethodDefinition
    {
        string Name { get; }
    }
}
