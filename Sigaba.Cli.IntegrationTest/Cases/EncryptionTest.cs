namespace Sigaba.Cli.Cases;

public class EncryptionTest : BaseTest
{
  private readonly string password = "password";

  private async Task InitializeAppAsync()
  {
    await App.RunAsync(["init", "-n", "-p", password]);
  }

  [Fact]
  public async Task Should_encrypt_all_files_in_directory_tree()
  {
    await InitializeAppAsync();
    var file1Path = Fs.AddFilePath2("""
            {
                "field1": "value 1",
                "field2_secret": "secret value 2",
            }
            """, "fileA.secrets.json");
    var file2Path = Fs.AddFilePath2("""
            {
                "field3": "value 3",
                "field4_secret": "secret value 4",
            }
            """, "subdir1", "subdir2", "fileB.secrets.json");

    //

    await App.RunAsync(["encrypt"]);

    //

    var jsonTester1 = JsonTester.FromFile(file1Path);
    jsonTester1.GetJsonValue<string>("$.field1").Should().Be("value 1");
    jsonTester1.GetJsonValue<string>("$.field2_secret").Should().NotBe("secret value 2");

    var jsonTester2 = JsonTester.FromFile(file2Path);
    jsonTester2.GetJsonValue<string>("$.field3").Should().Be("value 3");
    jsonTester2.GetJsonValue<string>("$.field4_secret").Should().NotBe("secret value 4");
  }

  [Fact]
  public async Task Should_not_encrypt_without_valid_public_key()
  {
    await InitializeAppAsync();
    var file1Path = Fs.AddFilePath2(null, "file_secrets.json");

    JsonTester.EditJsonFileInPlace<string>(Fs, "sigaba.json", "$.publicKey", value => (value + "x")); // messing with the key so that it is invalid
    var action = () => App.RunAsync(["encrypt"]);

    await action.Should().ThrowAsync<Exception>();
  }
}
