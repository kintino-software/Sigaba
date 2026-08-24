using Sigaba.Cli.Models;
using Spectre.Console.Cli;

namespace Sigaba.Cli.Adaptors;

public class BaseCommandTests
{
    private class DummyCommand(IGlobalOptions globalOptions) : BaseCommand(globalOptions)
    {
        public bool ExecuteCoreAsyncCalled { get; private set; } = false;
        public int ReturnValue { get; } = 66;

        protected override Task<int> ExecuteCoreAsync(CommandContext context, BaseCommandSettings settings, CancellationToken cancellationToken)
        {
            ExecuteCoreAsyncCalled = true;
            return Task.FromResult(ReturnValue);
        }
    }

    private readonly IGlobalOptions globalOptions = Substitute.For<IGlobalOptions>();

    private async static Task<int> ExecuteCommand(DummyCommand command, BaseCommandSettings settings)
    {
        return await (command as ICommand<BaseCommandSettings>).ExecuteAsync(
               new CommandContext([], Substitute.For<IRemainingArguments>(), "name", null),
               settings,
               CancellationToken.None);
    }

    // ExecuteAsync

    [Fact]
    public async Task Should_set_verbosity_of_logger()
    {
        var command = new DummyCommand(globalOptions);

        foreach (var verbosity in Enum.GetValues<VerbosityLevel>())
        {
            globalOptions.ClearReceivedCalls();

            await ExecuteCommand(command, new BaseCommandSettings() { Verbosity = verbosity });

            globalOptions.Received().SetVerbosity(verbosity);
        }
    }

    [Fact]
    public async Task Should_set_quiet_mode()
    {
        var command = new DummyCommand(globalOptions);

        await ExecuteCommand(command, new BaseCommandSettings() { IsQuiet = true });

        globalOptions.Received().SetVerbosity(VerbosityLevel.Quiet);
    }


    [Fact]
    public async Task Should_call_derived_classes_execute()
    {
        var command = new DummyCommand(globalOptions);
        command.ExecuteCoreAsyncCalled.Should().BeFalse();

        var result = await ExecuteCommand(command, new BaseCommandSettings());

        command.ExecuteCoreAsyncCalled.Should().BeTrue();
        result.Should().Be(command.ReturnValue);
    }
}

