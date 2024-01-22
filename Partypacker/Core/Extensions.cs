using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Partypacker.Core
{
    internal static class Extensions
    {
        // credit to PsychoPast's LawinServer launcher for the following code:
        // https://github.com/PsychoPast/LawinServer/blob/master/LawinServer/Core/Extensions.cs
        // without it, fiddler-less proxying would have never been achieved

        public static void SetValues(this RegistryKey registryKey, List<RegistryInfo> registryInfos) => registryInfos
                .ForEach(x => registryKey
                .SetValue(x.Name, x.Value, x.RegistryValueKind));
    }
}