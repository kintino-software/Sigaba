namespace Sigaba.App.Services.Settings;

internal static class InterfacesInspector
{
    public static IEnumerable<Type> GetAllImplementationsOf<T>()
    {
        var interfaceType = typeof(T);
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => interfaceType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract);
    }
}