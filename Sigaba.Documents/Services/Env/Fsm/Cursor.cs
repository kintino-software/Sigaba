using System.Text;

namespace Sigaba.Documents.Services.Env.Fsm;

internal interface ICursor : IDisposable
{
    int ColumnIndex { get; }
    int CurrIndex { get; }
    int LineIndex { get; }

    char? Next();
    char? Peek();
}

internal class StreamCursor(Stream stream) : ICursor
{
    private readonly StreamReader reader = new(stream, Encoding.UTF8);
    public int ColumnIndex { get; }
    public int CurrIndex { get; }
    public int LineIndex { get; }

    public char? Next()
    {
        return reader.Read() switch
        {
            -1 => null,
            var c => (char)c
        };
    }

    public char? Peek()
    {
        return reader.Peek() switch
        {
            -1 => null,
            var c => (char)c
        };
    }

    public void Dispose()
    {
        reader.Dispose();
    }
}

internal sealed class Cursor : ICursor
{
    private readonly string content;
    private bool allConsumed = false;
    private bool pendingLineBreak = false;

    public int LineIndex { get; private set; } = 0;
    public int ColumnIndex { get; private set; } = -1;
    public int CurrIndex { get; private set; } = -1;

    public Cursor(string content)
    {
        if (string.IsNullOrEmpty(content))
            throw new ArgumentException("Content cannot be null or empty.", nameof(content));
        this.content = content;
    }

    public char? Peek()
    {
        if (CurrIndex < 0 || allConsumed)
            return null;
        return content[CurrIndex];
    }

    public char? Next()
    {
        CurrIndex++;
        allConsumed = CurrIndex >= content.Length; // next char is beyond the content length, so all chars have been consumed
        if (allConsumed)
        {
            CurrIndex = content.Length - 1; // cursor cannot go beyond the content length, so set it to the last valid index
            return null;
        }

        var c = content[CurrIndex];

        if (pendingLineBreak)
        {
            LineIndex++;
            ColumnIndex = 0;
            pendingLineBreak = false;
        }
        else
        {
            ColumnIndex++;
        }

        pendingLineBreak = c == SChar.NewLine;

        return c;
    }

    public void Dispose()
    {
        // No unmanaged resources to release, but implement IDisposable for future-proofing
    }
}
