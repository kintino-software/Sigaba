//using System.Text;

//namespace Sigaba.Documents.Services.Env.Interpreter;

//public class LexerTests
//{
//    private Cursor CreateCursor(string content)
//    {
//        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
//        return new Cursor(stream);
//    }

//    [Fact]
//    public void Lex_should_parse_tokens()
//    {
//        var content = "KEY=VALUE\n#This is a comment\nANOTHER_KEY=ANOTHER_VALUE";
//        using var cursor = CreateCursor(content);
//        var lexer = new Lexer(cursor);

//        var tokens = lexer.Lex().ToArray();

//        tokens[0].Should().BeEquivalentTo(new Token(TokenType.Key, 0) { Content = "KEY", EndIndex = 2 });
//        tokens[2].Should().BeEquivalentTo(new Token(TokenType.Value, 4) { Content = "VALUE", EndIndex = 8 });
//        tokens[4].Should().BeEquivalentTo(new Token(TokenType.Comment, 10) { Content = "This is a comment", EndIndex = 30 });
//        tokens[6].Should().BeEquivalentTo(new Token(TokenType.Key, 32) { Content = "ANOTHER_KEY", EndIndex = 42 });
//        tokens[8].Should().BeEquivalentTo(new Token(TokenType.Value, 44) { Content = "ANOTHER_VALUE", EndIndex = 56 });

//    }
//}

