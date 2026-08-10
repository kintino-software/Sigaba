using Sigaba.App;
using Sigaba.Primitives;
using System.Text;

namespace Sigaba.Cli.Services;

internal class WindowsEditTextEditor : ITextEditor
{
    public async Task EditFile(FilePath filePath)
    {
        var sb = new StringBuilder();
        var result = await CliWrap.Cli.Wrap("edit")
            .WithArguments(filePath.Path)
            .WithValidation(CliWrap.CommandResultValidation.None)
            .WithStandardOutputPipe(CliWrap.PipeTarget.ToStringBuilder(sb))
            .WithStandardErrorPipe(CliWrap.PipeTarget.ToStringBuilder(sb))
            .ExecuteAsync();

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException(sb.ToString());
        }
    }
}
