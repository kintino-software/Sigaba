namespace Kintino.CipherConf.IO.Services;

internal interface IFileCrawler
{
    IEnumerable<string> Crawl(string rootDirFullPath, bool scanDeep);
}
