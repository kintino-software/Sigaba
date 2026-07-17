namespace Kintino.CipherConf.Documents.Services;

internal interface ICipherResolver
{
    public IDocumentCipher Resolve(string filePath);
}
