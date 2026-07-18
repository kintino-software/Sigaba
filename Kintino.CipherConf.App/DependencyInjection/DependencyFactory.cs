using Kintino.CipherConf.App.Dependencies;
using System.IO.Abstractions;

namespace Kintino.CipherConf.App.DependencyInjection;

public class DependencyFactory
{
    public required Func<IServiceProvider, ITextEditor> TextEditorFactory { get; init; }
    public required Func<IServiceProvider, IFileSystem> FileSystemFactory { get; init; }
}
