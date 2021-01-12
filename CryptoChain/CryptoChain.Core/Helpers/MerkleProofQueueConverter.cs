using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CryptoChain.Core.Helpers
{
    /// <summary>
    /// A custom converter to serialize and deserialize a MerkleProof Queue
    /// <code>
    /// var options = new JsonSerializerOptions {Converters = {new MerkleProofQueueConverter()}};
    /// JsonSerializer.Serialize(..., options);
    /// </code>
    /// </summary>
    public class MerkleProofQueueConverter : JsonConverter<Queue<(byte[], bool)>>
    {
        public override Queue<(byte[], bool)> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var queue = new Queue<(byte[], bool)>();
            
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    reader.Read();
                    if(reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "hash")
                        throw new JsonException("JSON is invalid, expected property \"hash\"");
                    
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.String)
                        throw new JsonException("JSON is invalid, expected base64 string");
                   
                    byte[] bytes = reader.GetBytesFromBase64();
                    
                    reader.Read();
                    if(reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "side")
                        throw new JsonException("JSON is invalid, expected property \"side\"");
                    
                    reader.Read();
                    if(reader.TokenType != JsonTokenType.True && reader.TokenType != JsonTokenType.False)
                        throw new JsonException("JSON is invalid, expected boolean");
                    
                    bool side = reader.GetBoolean();
                    queue.Enqueue((bytes, side));
                }
            }

            return queue;
        }

        public override void Write(Utf8JsonWriter writer, Queue<(byte[], bool)> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value.ToList())
            {
                writer.WriteStartObject();
                writer.WriteBase64String("hash", item.Item1);
                writer.WriteBoolean("side",item.Item2);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
    }
}