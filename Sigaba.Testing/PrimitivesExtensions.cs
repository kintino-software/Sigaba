using Sigaba.Primitives.Crypto.Base;
using System.Diagnostics.CodeAnalysis;

namespace Sigaba;

[ExcludeFromCodeCoverage]
public static class PrimitivesExtensions
{
    extension<T>(T) where T : IByteLike
    {
        /// <summary>
        /// Generates a random instance of the specified type T that implements <see cref="IByteLike"/>.
        /// </summary>
        /// <returns>A random instance of type T.</returns>
        public static T Any()
        {
            var random = new Random();
            var bytes = new byte[16];
            random.NextBytes(bytes);
            return (T)Activator.CreateInstance(typeof(T), bytes)!;
        }
    }
}
