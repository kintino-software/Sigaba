namespace Kintino.CipherConf.IO.Services;

public class FileCrawlerTest : BaseTest
{
    private IFileCrawler CreateService() => new FileCrawler(this.Fs);

    //[Fact]
    //public void Should_crawl_files_recursively()
    //{
    //    string[] filePaths =
    //    [
    //        this.RootCombine("1.txt"),
    //        this.RootCombine("2.txt"),
    //        this.RootCombine("a", "3.txt"),
    //        this.RootCombine("a", "4.txt"),
    //        this.RootCombine("a", "b", "5.txt"),
    //        this.RootCombine("a", "b", "6.txt"),
    //    ];
    //    foreach (var filePath in filePaths) this.Fs.AddEmptyFile(filePath);
    //    var service = this.CreateService();

    //    var result = new List<string>();
    //    service.Crawl(
    //        rootDir: RootPath,
    //        filePathAction: (f) => result.Add(f),
    //        shouldIncludeFile: _ => true,
    //        shouldExcludeFolder: _ => false);

    //    result.Should().HaveCount(filePaths.Length);
    //    result.Should().BeEquivalentTo(filePaths);
    //}
}

