namespace Sigaba.Documents.Services.Env.Fsm;

internal class Cursor(string content)
{
    public int LineIndex { get; private set; } = 0;
    public int ColumnIndex { get; private set; } = 0;
    public int CurrIndex { get; private set; } = 0;
    public bool IsLastChar => CurrIndex == content.Length - 1;
    public char CurrChar => content[CurrIndex];

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
