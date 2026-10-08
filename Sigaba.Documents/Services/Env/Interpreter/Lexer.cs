//using System.Text;

//namespace Sigaba.Documents.Services.Env.Interpreter;

//internal class Lexer(Cursor cursor)
//{
//    // pattern: peek first, move later

//    private readonly StringBuilder textBuffer = new(256);

//    public IEnumerable<Token> Lex()
//    {
//        if (cursor.Current == CharRef.NullChar)
//            yield break; // If the cursor is at the end of the file, return an empty sequence

//        CharRef current;
//        while ((current = cursor.Current) != CharRef.NullChar)
//        {
//            if (current == SChar.Space || current == SChar.Tab)
//            {
//                cursor.Move(); // consume the whitespace character
//                continue;
//            }

//            if (current == SChar.Comment)
//            {
//                cursor.Move(); // consume the comment character
//                yield return this.HandleComment();
//                continue;
//            }

//            if (char.IsLetter(current) || current == SChar.Unserscore)
//            {
//                yield return this.HandleKey();
//                continue;
//            }

//            if (current == SChar.Eq)
//            {
//                cursor.Move(); // consume the '=' character
//                yield return this.HandleValue();
//                continue;
//            }

//            cursor.Move(); // move forward to avoid infinite loop
//        }
//    }

//    private Token HandleComment()
//    {
//        int startIndex = cursor.Current.Index;
//        while (true)
//        {
//            if (cursor.Current == CharRef.NullChar)
//            {
//                return new Token(TokenType.Comment, FlushTextBuffer(), startIndex, cursor.Current.Index);
//            }

//            textBuffer.Append(cursor.Current);

//            if (cursor.Next == SChar.LineBreak)
//            {
//                var token = new Token(TokenType.Comment, FlushTextBuffer(), startIndex, cursor.Current.Index - 1);
//                return token;
//            }

//            cursor.Move();
//        }
//    }

//    private Token HandleKey()
//    {
//        int startIndex = cursor.Current.Index;
//        while (true)
//        {
//            if (cursor.Current == CharRef.NullChar)
//            {
//                throw new FormatException($"Unexpected end of file while parsing key at index {cursor.Current.Index}. Expected '=' after key.");
//            }

//            textBuffer.Append(cursor.Current);

//            if (cursor.Next == SChar.Eq)
//            {
//                return new Token(TokenType.Key, FlushTextBuffer(), startIndex, cursor.Current.Index);
//            }

//            cursor.Move();
//        }
//    }

//    private Token HandleValue()
//    {
//        if (cursor.Current == CharRef.NullChar)
//            return HandleEmptyValue();
//        if (cursor.Current == SChar.SingleQuote)
//        {
//            cursor.Move(); // consume the opening single quote
//            return HandleEscapedLiteralValue();
//        }
//        if (cursor.Current == SChar.DoubleQuote)
//        {
//            cursor.Move(); // consume the opening double quote
//            return HandleLiteralValue();
//        }

//        int startIndex = cursor.Current.Index;
//        bool hasValueContinuation = false;
//        while (true)
//        {
//            if (cursor.Current == CharRef.NullChar)
//            {
//                return new Token(TokenType.Value, FlushTextBuffer(), startIndex, cursor.Current.Index);
//            }

//            textBuffer.Append(cursor.Current.Value);

//            if (cursor.Current == SChar.ValueContinuation)
//            {
//                hasValueContinuation = true;
//                continue;
//            }

//            if (cursor.Next == SChar.LineBreak)
//            {

//            }

//            textBuffer.Append(cursor.Current.Value);
//        }
//        var token = new Token(TokenType.Value, firstCharRef.Index)
//        {
//            Content = FlushTextBuffer(),
//            EndIndex = cursor.CurrIndex,
//        };
//        return token;
//    }

//    private Token HandleEmptyValue()
//    {
//        var token = new Token(TokenType.Value, cursor.CurrIndex)
//        {
//            Content = string.Empty,
//            EndIndex = cursor.CurrIndex,
//        };
//        return token;
//    }

//    private Token HandleEscapedLiteralValue()
//    {
//        var startTextIndex = cursor.CurrIndex;
//        CharRef c;
//        while ((c = cursor.Read()) != CharRef.NullChar)
//        {
//            if (c == SChar.LineBreak)
//                break;
//            if (c == SChar.SingleQuote)
//            {
//                cursor.Read(); // consume the closing single quote
//                break;
//            }
//            textBuffer.Append(c.Value);
//        }

//        var token = new Token(TokenType.EscapedLiteralValue, startTextIndex)
//        {
//            Content = FlushTextBuffer(),
//            EndIndex = cursor.CurrIndex,
//        };
//        return token;
//    }

//    private Token HandleLiteralValue()
//    {
//        var startTextIndex = cursor.CurrIndex;
//        CharRef c;
//        while ((c = cursor.Read()) != CharRef.NullChar)
//        {
//            if (c == SChar.LineBreak)
//                break;
//            if (c == SChar.DoubleQuote)
//            {
//                cursor.Read(); // consume the closing double quote
//                break;
//            }
//            textBuffer.Append(c.Value);
//        }

//        var token = new Token(TokenType.LiteralValue, startTextIndex)
//        {
//            Content = FlushTextBuffer(),
//            EndIndex = cursor.CurrIndex,
//        };
//        return token;
//    }


//    private string FlushTextBuffer()
//    {
//        if (textBuffer.Length == 0)
//            return string.Empty;
//        var content = textBuffer.ToString();
//        textBuffer.Clear();
//        return content;
//    }

//}