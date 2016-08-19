using System;
using System.Collections.Generic;
using System.Linq;

namespace TWNewEgg.ECWeb.PrivilegeFilters.Api
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class AllowNonSecuresAttribute : Attribute
    {
    }
}
