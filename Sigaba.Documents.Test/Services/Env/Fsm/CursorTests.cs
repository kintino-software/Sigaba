namespace Sigaba.Documents.Services.Env.Fsm;

public class CursorTests
{
    private static Cursor CreateCursor(string content)
    {
        return new Cursor(content);
    }

    [Fact]
    public void Next_should_move_forward_and_return_next_character()
    {
        var content = "abc";
        var cursor = CreateCursor(content);

        cursor.Next().Should().Be('b');
        cursor.Next().Should().Be('c');
        cursor.Next().Should().BeNull();
    }

    [Fact]
    public void CurrChar_should_return_current_char()
    {
        var content = "abc";
        var cursor = CreateCursor(content);

        cursor.CurrChar.Should().Be('a');
        cursor.Next();
        cursor.CurrChar.Should().Be('b');
        cursor.Next();
        cursor.CurrChar.Should().Be('c');
        cursor.Next();
        cursor.CurrChar.Should().Be('c'); // as it could not move next, remains in the last char
    }

    [Fact]
    public void Should_track_current_char_index()
    {
        var content = "ab\nc";
        var cursor = CreateCursor(content);

        cursor.CurrIndex.Should().Be(0);
        cursor.Next();
        cursor.CurrIndex.Should().Be(1);
        cursor.Next();
        cursor.CurrIndex.Should().Be(2);
        cursor.Next();
        cursor.CurrIndex.Should().Be(3);
        cursor.Next();
        cursor.CurrIndex.Should().Be(3); // as it could not move next, remains in the last index

    }

    [Fact]
    public void Should_track_line_and_columns()
    {
        var content = "ab\ncd\ne";
        var cursor = CreateCursor(content);

        //a
        cursor.LineIndex.Should().Be(0);
        cursor.ColumnIndex.Should().Be(0);
        cursor.Next();
        //b
        cursor.LineIndex.Should().Be(0);
        cursor.ColumnIndex.Should().Be(1);
        cursor.Next();
        // \n
        cursor.LineIndex.Should().Be(0);
        cursor.ColumnIndex.Should().Be(2);
        cursor.Next();
        //c
        cursor.LineIndex.Should().Be(1);
        cursor.ColumnIndex.Should().Be(0);
        cursor.Next();
        //d
        cursor.LineIndex.Should().Be(1);
        cursor.ColumnIndex.Should().Be(1);
        cursor.Next();
        // \n
        cursor.LineIndex.Should().Be(1);
        cursor.ColumnIndex.Should().Be(2);
        cursor.Next();
        //e
        cursor.LineIndex.Should().Be(2);
        cursor.ColumnIndex.Should().Be(0);
        cursor.Next();
        // as it cant move forward, it should remain in the last indexes
        cursor.LineIndex.Should().Be(2);
        cursor.ColumnIndex.Should().Be(0);
    }


}

