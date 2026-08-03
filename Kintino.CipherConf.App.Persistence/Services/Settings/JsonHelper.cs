using System.Text;
using System.Text.Json;

namespace Kintino.CipherConf.App.Services.Settings;

internal static class JsonHelper
{
    public static int ReadVersionFromJson(string jsonDocument)
    {
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonDocument), isFinalBlock: true, state: default);
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case (JsonTokenType.PropertyName):
                    if (reader.GetString()?.ToLower() == "version")
                    {
                        reader.Read();
                        return reader.GetInt32();
                    }
                    break;
            }
        }
        return -1;
    }

    public static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions();
}
