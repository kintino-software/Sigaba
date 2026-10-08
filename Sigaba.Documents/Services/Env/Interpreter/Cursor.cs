namespace Sigaba.Documents.Services.Env.Interpreter;

internal readonly record struct CharRef(char Value, int Index)
{
    public static readonly CharRef NullChar = new('\0', -1);
    public static implicit operator char(CharRef c) => c.Value;
    public static bool operator ==(CharRef c, char ch) => c.Value == ch;
    public static bool operator !=(CharRef c, char ch) => c.Value != ch;
}

internal sealed class Cursor : IDisposable
{
    private readonly StreamReader reader;

    public CharRef Current { get; private set; } = CharRef.NullChar;
    public CharRef Next { get; private set; } = CharRef.NullChar;

    public Cursor(Stream stream)
    {
        if (stream == null)
            throw new ArgumentException("Stream cannot be null.", nameof(stream));

        if (stream.Position > 0)
        {
            if (!stream.CanSeek)
            {
                throw new ArgumentException("Stream must be seekable if the position is greater than 0.", nameof(stream));
            }
            stream.Seek(0, SeekOrigin.Begin);
        }

        reader = new StreamReader(stream);

        var a = reader.Read();
        var b = reader.Read();

        Current = a == -1 ? CharRef.NullChar : new CharRef((char)a, 0);
        Next = b == -1 ? CharRef.NullChar : new CharRef((char)b, 1);

    }

    public void Move()
    {
        var i = reader.Read();
        Current = Next;
        Next = i == -1 ? CharRef.NullChar : new CharRef((char)i, Next.Index + 1);
    }

    public void Dispose()
    {
        reader?.Dispose();
    }
}
