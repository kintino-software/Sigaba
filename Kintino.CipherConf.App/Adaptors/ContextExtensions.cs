using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.App.Models;
using Kintino.CipherConf.App.Primitives;

namespace Kintino.CipherConf.App.Adaptors;

internal static class ContextExtensions
{
    extension(Context context)
    {
        public PlainKey DecryptKey(IAsymmetricCipher asymmetricCipher)
        {
            var cryptoBytes = new CryptoBytes(context.Key.Bytes);
            var plainBytes = asymmetricCipher.Decrypt(cryptoBytes, context.PrivateKey);
            return new PlainKey(plainBytes.Bytes);
        }
    }
}
