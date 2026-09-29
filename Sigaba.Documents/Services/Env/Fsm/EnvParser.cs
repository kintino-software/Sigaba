namespace Sigaba.Documents.Services.Env.Fsm;

public record EnvEntry(string Key, int ValueStartIdx, int ValueLength);

public class EnvParser
{
    public IReadOnlyDictionary<string, EnvEntry> Parse(string envDocument)
    {
        var content = envDocument.Replace("\r\n", "\n").Replace('\r', '\n');

        var fsm = new EnvFsm();

        var lineIdx = 0;
        var colIndex = 0;

        for (int i = 0; i < content.Length; i++)
        {
            var c = content[i];
            if (c == SChar.NewLine)
            {
                lineIdx++;
                colIndex = 0;
            }
            else
            {
                colIndex++;
            }

            try
            {
                fsm.HandleChar(
                    c: c,
                    curIndex: i,
                    isLastChar: (i == content.Length - 1));
            }
            catch (Exception ex) when (ex is FormatException)
            {
                throw new EnvParseException(lineIdx + 1, colIndex + 1, ex.Message);
            }
        }


        return fsm.Entries;
    }
}
