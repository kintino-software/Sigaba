//using Sigaba.Documents.Services.Env.Interpreter;
//using System.Text;

//namespace Sigaba.Documents.Services.Env.Interpreter;

//public sealed class CursorTests : IDisposable
//{
//    private Cursor cursor;
//    private Stream stream;

//    private Cursor CreateCursor(string content)
//    {
//        stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
//        cursor = new Cursor(stream);

//        return cursor;
//    }

//    public void Dispose()
//    {
//        cursor?.Dispose();
//        stream?.Dispose();
//    }

//    //

//    [Fact]
//    public void Should_throw_when_instantiating_with_null_or_empty_string()
//    {
//        Action act = () => { using var cursor = new Cursor(null); };
//        act.Should().Throw<ArgumentException>().WithMessage("Stream cannot be null.*");
//    }

//    [Fact]
//    public void Should_read_next_char()
//    {
//        var content = "abc";
//        var cursor = CreateCursor(content);

//        cursor.Read().Should().Be(new CharRef('a', 0, true));
//        cursor.Read().Should().Be(new CharRef('b', 1, true));
//        cursor.Read().Should().Be(new CharRef('c', 2, true));
//        cursor.Read().Should().Be(CharRef.NullChar);
//    }

//    [Fact]
//    public void Should_peek_next_char()
//    {
//        var content = "abc";
//        var cursor = CreateCursor(content);

//        cursor.Peek().Should().Be(new CharRef('a', 0, true));
//        cursor.Read();
//        cursor.Peek().Should().Be(new CharRef('b', 1, true));
//        cursor.Read();
//        cursor.Peek().Should().Be(new CharRef('c', 2, true));
//        cursor.Read();
//        cursor.Peek().Should().Be(CharRef.NullChar);
//    }

//    [Fact]
//    public void Should_track_current_char_index()
//    {
//        var content = "ab\nc";
//        var cursor = CreateCursor(content);

//        cursor.CurrIndex.Should().Be(-1); // starts at -1 as it has not moved yet
//        cursor.Read();
//        cursor.CurrIndex.Should().Be(0);
//        cursor.Read();
//        cursor.CurrIndex.Should().Be(1);
//        cursor.Read();
//        cursor.CurrIndex.Should().Be(2);
//        cursor.Read();
//        cursor.CurrIndex.Should().Be(3);
//        cursor.CurrIndex.Should().Be(3); // as it could not move next, remains in the last index

//    }

//    [Theory]
//    [InlineData("abc", 2)]
//    [InlineData("ab\nc", 3)]
//    [InlineData("a", 0)]
//    public void Cursor_index_should_not_go_beyond_last_char(string content, int expectedLastIndex)
//    {
//        var cursor = CreateCursor(content);

//        for (int i = 0; i < 10; i++)
//        {
//            cursor.Read();
//        }

//        cursor.CurrIndex.Should().Be(expectedLastIndex);
//    }

//}

