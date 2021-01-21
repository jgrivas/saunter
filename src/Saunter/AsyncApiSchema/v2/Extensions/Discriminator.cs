using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Saunter.AsyncApiSchema.v2.Extensions
{
    public class Discriminator
    {
        [JsonPropertyName("propertyName")]
        public string PropertyName { get; set; }

        [JsonPropertyName("mapping")]
        public IDictionary<string, string> Mapping { get; set; }
    }
}
