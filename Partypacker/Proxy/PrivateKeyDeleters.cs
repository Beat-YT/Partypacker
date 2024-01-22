using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Partypacker.Net
{
    internal class PrivateKeyDeleters
    {
        // credit to PsychoPast's LawinServer launcher for the following code:
        // https://github.com/PsychoPast/LawinServer/blob/master/LawinServer/Proxy/PrivateKeyDeleters.cs
        // without it, fiddler-less proxying would have never been achieved

        private readonly IDictionary<Type, Action<AsymmetricAlgorithm>> privateKeyDeleters = new Dictionary<Type, Action<AsymmetricAlgorithm>>();

        public PrivateKeyDeleters()
        {
            AddPrivateKeyDeleter<RSACng>(DefaultRSACngPrivateKeyDeleter);
            AddPrivateKeyDeleter<RSACryptoServiceProvider>(DefaultRSACryptoServiceProviderPrivateKeyDeleter);
        }

        private void AddPrivateKeyDeleter<T>(Action<T> keyDeleter) where T : AsymmetricAlgorithm => privateKeyDeleters[typeof(T)] = (a) => keyDeleter((T)a);

        public void DeletePrivateKey(AsymmetricAlgorithm asymmetricAlgorithm)
        {
            for (Type type = asymmetricAlgorithm.GetType(); type != null; type = type.BaseType)
            {
                if (privateKeyDeleters.TryGetValue(type, out Action<AsymmetricAlgorithm> deleter))
                {
                    deleter(asymmetricAlgorithm);
                    return;
                }
            }
        }

        private void DefaultRSACryptoServiceProviderPrivateKeyDeleter(RSACryptoServiceProvider rsaCryptoServiceProvider)
        {
            rsaCryptoServiceProvider.PersistKeyInCsp = false;
            rsaCryptoServiceProvider.Clear();
        }

        private void DefaultRSACngPrivateKeyDeleter(RSACng rsaCng)
        {
            rsaCng.Key.Delete();
            rsaCng.Clear();
        }
    }
}