using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.Cli.Cases;

public class DecryptTest : BaseTest
{
  private readonly string password = "password";

  private async Task InitializeAppAsync()
  {
    await App.RunAsync(["init", "-n", "-p", password]);
  }

  [Fact]
  public async Task Should_decrypt_all_files_in_directory_tree()
  {
    await InitializeAppAsync();
    var originalContent1 = """
            {
                "field1": "value 1",
                "field2_secret": "secret value 2",
            }
            """;
    var originalContent2 = """
            {
                "field3": "value 3",
                "field4_secret": "secret value 4",
            }
            """;
    Fs.AddFile("file1.secrets.json", new MockFileData(originalContent1));
    Fs.AddFile("file2.secrets.json", new MockFileData(originalContent2));
    await App.RunAsync(["encrypt"]);

    //

    await App.RunAsync(["decrypt", "-p", password]);

    //

    Fs.GetFile("file1.secrets.json").TextContents.Should().Be(originalContent1);
    Fs.GetFile("file2.secrets.json").TextContents.Should().Be(originalContent2);
  }

  [Fact]
  public async Task Should_not_decript_without_private_key()
  {
    await InitializeAppAsync();

    //

    Fs.RemoveFile(Fs.AllFiles.First(f => f.EndsWith("private.key"))); // remove private key to simulate missing key
    var action = () => App.RunAsync(["decrypt", "-p", password]);

    //

    await action.Should().ThrowAsync<Exception>();
  }
}
