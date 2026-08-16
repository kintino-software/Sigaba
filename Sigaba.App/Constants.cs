namespace Sigaba.App;

internal class Constants
{
  public const string PrivateKeyFileName = "private.key";
  public const string SigabaFileName = "sigaba.json";
  public static readonly string SigabaSystemDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ".sigaba");
  public const string PrivateKeyDirEnvVarKey = "SIGABA_PRIVATE_KEY_DIR";
}
