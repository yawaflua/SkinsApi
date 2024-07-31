using System.Text.Json.Serialization;

namespace SkinsApi.Models
{
    public class Metadata
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }
    }

    public class DecodedSkinProperty
    {
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("profileId")]
        public string ProfileId { get; set; }

        [JsonPropertyName("profileName")]
        public string ProfileName { get; set; }

        [JsonPropertyName("textures")]
        public Textures Textures { get; set; }
    }

    public class SKIN
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("metadata")]
        public Metadata Metadata { get; set; }
    }

    public class Textures
    {
        [JsonPropertyName("SKIN")]
        public SKIN SKIN { get; set; }
    }


}
