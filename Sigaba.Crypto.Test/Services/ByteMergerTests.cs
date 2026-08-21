namespace Sigaba.Crypto.Services;

public class ByteMergerTests
{
    // constructor

    [Fact]
    public void Should_throw_when_creating_from_splited_data_with_less_than_two_arrays()
    {
        var action = () => ByteMerger.FromSplitedData([1, 2, 3]);

        action.Should().Throw<ArgumentException>().WithMessage("At least two byte arrays must be provided*");
    }

    [Fact]
    public void Should_throw_when_creating_from_merged_data_with_less_than_two_bytes()
    {
        var action = () => ByteMerger.FromMergedData([1]);

        action.Should().Throw<ArgumentException>().WithMessage("Merged data must have at least two bytes*");
    }

    // Merge

    [Fact]
    public void Should_merge_arrays_of_byte_arrays()
    {
        byte[] part1 = [1, 2, 3];
        byte[] part2 = [4, 5, 6];
        var meger = ByteMerger.FromSplitedData(part1, part2);

        var resut = meger.Merge();

        resut.Should().HaveCount(6);
    }

    // Split

    [Fact]
    public void Should_split_in_2_parts()
    {
        byte[] data1 = [1, 2];
        byte[] data2 = [3, 4, 5];
        var splitter = ByteMerger.FromMergedData([.. data1, .. data2]);

        splitter.Split(data1.Length, out var part1, out var part2);

        part1.Should().BeEquivalentTo(data1);
        part2.Should().BeEquivalentTo(data2);
    }

    [Fact]
    public void Should_split_in_3_parts()
    {
        byte[] data1 = [1, 2];
        byte[] data2 = [3, 4, 5];
        byte[] data3 = [6, 7, 8, 9];
        var splitter = ByteMerger.FromMergedData([.. data1, .. data2, .. data3]);

        splitter.Split(data1.Length, data2.Length, out var part1, out var part2, out var part3);

        part1.Should().BeEquivalentTo(data1);
        part2.Should().BeEquivalentTo(data2);
        part3.Should().BeEquivalentTo(data3);
    }
}

