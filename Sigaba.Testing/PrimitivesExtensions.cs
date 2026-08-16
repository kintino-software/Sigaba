using Sigaba.Primitives.Crypto.Base;

namespace Sigaba;

public static class PrimitivesExtensions
{
  extension<T>(T) where T : IByteLike
  {
    public static T Any()
    {
      var random = new Random();
      var bytes = new byte[16];
      random.NextBytes(bytes);
      return (T)Activator.CreateInstance(typeof(T), bytes)!;
    }
  }
}
