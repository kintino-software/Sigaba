//using Kintino.CipherConf.Documents.Models;
//using System.Text.Json;

//namespace Kintino.CipherConf.Documents.Services.Json;

//internal class JsonDocumentModel : IDocumentModel
//{
//    private record Replacement(int Start, int OriginalLength, byte[] NewValueBytes);

//    public static DocumentType DocumentType { get; } = DocumentType.Json;

//    public string Transform<TNewValue>(
//        string documentContent,
//        Func<IDocumentNode<TNewValue>, TNewValue>? transform = null,
//        Func<IDocumentNode<TNewValue>, string>? transformRaw = null)
//    {
//        var jsonByteScanner = JsonByteScanner.Create(documentContent);
//        return jsonByteScanner.Transform((propertyName) =>
//        {
//            if(transform != null)
//            {
//                var newValue = transform(node);
//                return SanitizeValue(JsonSerializer.Serialize(newValue));

//        }
//        else if (transformRaw != null)
//        {
//            var node = new DocumentNode<TNewValue>(propertyName, jsonByteScanner.GetPropertyValue(propertyName));
//            return transformRaw(node);
//        }
//        else
//        {
//            return jsonByteScanner.GetPropertyValue(propertyName);
//        });

//    }


//    private static string SanitizeValue(string value)
//    {
//        // If the value is a valid JSON, return it as-is; otherwise, treat it as a string and quote it
//        try
//        {
//            using var doc = JsonDocument.Parse(value);
//            return value; // It's valid JSON
//        }
//        catch (JsonException)
//        {
//            return JsonSerializer.Serialize(value); // Quote it as a string
//        }
//    }

//}
