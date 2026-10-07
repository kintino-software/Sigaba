namespace Sigaba.Documents.Services.Env.Fsm;

public class CursorTests
{
    private static Cursor CreateCursor(string content)
    {
        return new Cursor(content);
    }

    //

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_throw_when_instantiating_with_null_or_empty_string(string content)
    {
        Action act = () => CreateCursor(content);
        act.Should().Throw<ArgumentException>().WithMessage("Content cannot be null or empty.*");
    }

    [Fact]
    public void Next_should_move_forward_and_return_next_character()
    {
        var content = "abc";
        var cursor = CreateCursor(content);

        cursor.Next().Should().Be('a');
        cursor.Next().Should().Be('b');
        cursor.Next().Should().Be('c');
        cursor.Next().Should().BeNull();
    }

    [Fact]
    public void Peek_should_return_current_char()
    {
        var content = "abc";
        var cursor = CreateCursor(content);

        cursor.Peek().Should().BeNull();
        cursor.Next();
        cursor.Peek().Should().Be('a');
        cursor.Next();
        cursor.Peek().Should().Be('b');
        cursor.Next();
        cursor.Peek().Should().Be('c');
        cursor.Next();
        cursor.Peek().Should().BeNull();
    }

    [Fact]
    public void Should_track_current_char_index()
    {
        var content = "ab\nc";
        var cursor = CreateCursor(content);

        cursor.CurrIndex.Should().Be(-1); // starts at -1 as it has not moved yet
        cursor.Next();
        cursor.CurrIndex.Should().Be(0);
        cursor.Next();
        cursor.CurrIndex.Should().Be(1);
        cursor.Next();
        cursor.CurrIndex.Should().Be(2);
        cursor.Next();
        cursor.CurrIndex.Should().Be(3);
        cursor.CurrIndex.Should().Be(3); // as it could not move next, remains in the last index

    }

    [Fact]
    public void Should_track_line_and_columns()
    {
        var content = "ab\ncd\ne";
        var cursor = CreateCursor(content);

        // before moving, the cursor should be at the initial position
        cursor.LineIndex.Should().Be(0);
        cursor.ColumnIndex.Should().Be(-1);

        cursor.Next(); //a
        cursor.LineIndex.Should().Be(0);
        cursor.ColumnIndex.Should().Be(0);

        cursor.Next(); //b
        cursor.LineIndex.Should().Be(0);
        cursor.ColumnIndex.Should().Be(1);

        cursor.Next(); // \n
        cursor.LineIndex.Should().Be(0);
        cursor.ColumnIndex.Should().Be(2);

        cursor.Next(); //c
        cursor.LineIndex.Should().Be(1);
        cursor.ColumnIndex.Should().Be(0);

        cursor.Next(); //d
        cursor.LineIndex.Should().Be(1);
        cursor.ColumnIndex.Should().Be(1);

        cursor.Next(); // \n
        cursor.LineIndex.Should().Be(1);
        cursor.ColumnIndex.Should().Be(2);

        cursor.Next(); //e
        cursor.LineIndex.Should().Be(2);
        cursor.ColumnIndex.Should().Be(0);
        // as it cant move forward, it should remain in the last indexes
        cursor.Next();
        cursor.LineIndex.Should().Be(2);
        cursor.ColumnIndex.Should().Be(0);
    }

    [Theory]
    [InlineData("abc", 2)]
    [InlineData("ab\nc", 3)]
    [InlineData("a", 0)]
    public void Cursor_index_should_not_go_beyond_last_char(string content, int expectedLastIndex)
    {
        var cursor = CreateCursor(content);

        for (int i = 0; i < 10; i++)
        {
            cursor.Next();
        }

        cursor.CurrIndex.Should().Be(expectedLastIndex);
    }

}

