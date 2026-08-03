namespace Kintino.CipherConf.App.TestHelpers;

internal static class InterfacesInspector
{
    public static IEnumerable<T> GetAllImplementationsOf<T>()
    {
        var interfaceType = typeof(T);
        if (!interfaceType.IsInterface)
        {
            throw new ArgumentException($"{interfaceType.FullName} is not an interface type.");
        }
        var implementations = new List<T>();
        foreach (var type in typeof(T).Assembly.GetTypes())
        {
            if (interfaceType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            {
                var instance = (T)Activator.CreateInstance(type)!;
                implementations.Add(instance);
            }
        }
        return implementations;
    }
}
