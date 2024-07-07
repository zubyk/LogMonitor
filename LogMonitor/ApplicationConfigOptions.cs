//using Json.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace LogMonitor
{
    public class ApplicationConfigOptions
    {
        public class LowerCaseNamingPolicy : JsonNamingPolicy
        {
            public override string ConvertName(string name) =>
                name.ToLower();
        }

        public const string SectionName = "AppConfig";

        public string? MainTitle { get; set; }
        public string? CompanyName { get; set; }

        public string? SmtpFrom { get; set; }
        public string? SmtpHost { get; set; }
        public ushort? SmtpPort { get; set; }

        public List<string>? SmtpTo { get; set; }


        public JsonSerializerOptions JsonSerializerOptions { get; } =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = new LowerCaseNamingPolicy(),
                WriteIndented = true,
                AllowTrailingCommas = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            };

        //public ValidationOptions JsonSchemaValidationOptions { get; } =
        //    new ValidationOptions()
        //    {
        //        OutputFormat = OutputFormat.Basic
        //    };
    }
}
