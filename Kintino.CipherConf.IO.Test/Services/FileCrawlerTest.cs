namespace Kintino.CipherConf.IO.Services;

public class FileCrawlerTest : BaseTest
{
    private IFileCrawler CreateService()
    {
        return new FileCrawler(this.Fs);
    }

    [Fact]
    public void Should_crawl_files_recursively()
    {
        string[] filePaths =
        [
            this.Fs.Path.Combine(RootPath, "file1.txt"),
            this.Fs.Path.Combine(RootPath, "file2.txt"),
            this.Fs.Path.Combine(RootPath, "a", "file3.txt"),
            this.Fs.Path.Combine(RootPath, "a", "file4.txt"),
            this.Fs.Path.Combine(RootPath, "a", "b", "file5.txt"),
            this.Fs.Path.Combine(RootPath, "a", "b", "file6.txt"),
        ];
        foreach (var filePath in filePaths) this.Fs.AddEmptyFile(filePath);
        var service = this.CreateService();

        var files = service.Crawl(RootPath, scanDeep: true).ToList();

        files.Should().HaveCount(filePaths.Length);
        files.Should().BeEquivalentTo(filePaths);
    }

    [Fact]
    public void Should_crawl_files_shallow()
    {
        string[] filePaths =
        [
            this.Fs.Path.Combine(RootPath, "file1.txt"),
            this.Fs.Path.Combine(RootPath, "file2.txt"),
            this.Fs.Path.Combine(RootPath, "a", "file3.txt"),
            this.Fs.Path.Combine(RootPath, "a", "file4.txt"),
            this.Fs.Path.Combine(RootPath, "a", "b", "file5.txt"),
            this.Fs.Path.Combine(RootPath, "a", "b", "file6.txt"),
        ];
        foreach (var filePath in filePaths) this.Fs.AddEmptyFile(filePath);
        var service = this.CreateService();

        var files = service.Crawl(RootPath, scanDeep: false).ToList();

        files.Should().HaveCount(2);
        files.Should().BeEquivalentTo(filePaths.Take(2));
    }
}

