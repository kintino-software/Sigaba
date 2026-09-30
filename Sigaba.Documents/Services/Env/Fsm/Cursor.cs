namespace Sigaba.Documents.Services.Env.Fsm;

internal class Cursor
{
    private readonly string content;

    public int LineIndex { get; private set; } = 0;
    public int ColumnIndex { get; private set; } = 0;
    public int CurrIndex { get; private set; } = 0;
    public bool IsLastChar => CurrIndex == content.Length - 1;
    public char CurrChar => CurrIndex < content.Length ? content[CurrIndex] : '\0';

    public Cursor(string content)
    {
        if (string.IsNullOrEmpty(content))
            throw new ArgumentException("Content cannot be null or empty.", nameof(content));
        this.content = content;
    }

    public char? Next()
    {
        if (CurrChar == SChar.NewLine)
        {
            LineIndex++;
            ColumnIndex = 0;
        }
        else if (!IsLastChar)
        {
            ColumnIndex++;
        }

        if (IsLastChar) // alredy at the last char, cant move forward
            return null;

        CurrIndex++;
        char c = content[CurrIndex];


        return c;
    }

}
