using Kintino.CipherConf.IO;
using Kintino.CipherConf.Models;

namespace Kintino.CipherConfig;

public class FakeContextRepository : IContextRepository
{
    private Dictionary<string, object> dic = [];

    public ValueTask<IContext> GetContext(string folderPath)
    {
        if (!dic.TryGetValue(folderPath, out var context))
        {
            throw new InvalidOperationException($"No context found for folder path: {folderPath}");
        }
        return new ValueTask<IContext>((IContext)context);
    }

    public ValueTask<bool> HasContext(string folderPath)
    {
        return new ValueTask<bool>(dic.ContainsKey(folderPath));
    }

    public ValueTask SaveContext(IContext context, string folderPath)
    {
        dic[folderPath] = context;
        return new ValueTask();
    }
}
