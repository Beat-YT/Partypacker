using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Partypacker.Core
{
    internal class AppRegistry : IDisposable
    {
        // credit to PsychoPast's LawinServer launcher for the following code:
        // https://github.com/PsychoPast/LawinServer/blob/master/LawinServer/Core/AppRegistry.cs
        // without it, fiddler-less proxying would have never been achieved

        private const string AppKey = @"SOFTWARE\Partypacker";

        private readonly RegistryKey _registryKey;

        private readonly RegistryKey currentUser = Registry.CurrentUser;

        private RegistryKey OpenKey => currentUser.OpenSubKey(AppKey, RegistryKeyPermissionCheck.ReadWriteSubTree);

        public AppRegistry() => _registryKey = OpenKey switch
        {
            null => currentUser.CreateSubKey(AppKey, RegistryKeyPermissionCheck.ReadWriteSubTree),
            _ => currentUser.OpenSubKey(AppKey, RegistryKeyPermissionCheck.ReadWriteSubTree)
        };

        public void UpdateRegistry(List<RegistryInfo> registryInfos) => _registryKey.SetValues(registryInfos);

        public void Dispose()
        {
            _registryKey.Close();
            _registryKey.Dispose();
            GC.SuppressFinalize(this);
        }

        public T GetRegistryValue<T>(string name) => (T)_registryKey.GetValue(name, null);
    }

    internal class RegistryInfo
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public RegistryValueKind RegistryValueKind { get; set; }
    }
}