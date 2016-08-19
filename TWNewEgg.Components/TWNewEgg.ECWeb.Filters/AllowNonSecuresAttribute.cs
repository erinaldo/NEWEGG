using System;
using System.Collections.Generic;
using System.Linq;

namespace TWNewEgg.ECWeb.PrivilegeFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AllowNonSecuresAttribute : Attribute
    {
    }
}
