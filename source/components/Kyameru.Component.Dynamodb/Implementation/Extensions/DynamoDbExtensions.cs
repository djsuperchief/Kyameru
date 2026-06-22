using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Amazon.DynamoDBStreams.Model;

namespace Kyameru.Component.Dynamodb.Extensions
{
    /// <summary>
    /// DynamoDb extensions
    /// </summary>
    /// <remarks>
    /// This code was copied from the <see cref="https://github.com/aws/aws-lambda-dotnet/blob/master/Libraries/src/Amazon.Lambda.DynamoDBEvents/ExtensionMethods.cs"/> on 28/05/2026.
    /// At the time this code was copied, it was under Apache 2.0 license.
    /// Converting a stream to Json is an already achieved milestone and the package could not be used in dotnet standard.
    /// Alterations made were to always pretty print the Json.
    /// </remarks>
    public static class DynamoDbExtensions
    {
        public static string ToJson(this Dictionary<string, AttributeValue>? item)
        {
            if (item == null || item.Count == 0)
            {
                return "{}";
            }

            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true});

            WriteJson(writer, item);

            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }
        
        private static void WriteJson(Utf8JsonWriter writer, Dictionary<string, AttributeValue> item)
        {
            writer.WriteStartObject();

            foreach (var attribute in item)
            {
                writer.WritePropertyName(attribute.Key);
                WriteJsonValue(writer, attribute.Value);
            }

            writer.WriteEndObject();
        }
        
        private static void WriteJsonValue(Utf8JsonWriter writer, AttributeValue attribute)
        {
            if (attribute.S != null)
            {
                writer.WriteStringValue(attribute.S);
            }
            else if (attribute.N != null)
            {
                writer.WriteRawValue(attribute.N);
            }
            else if (attribute.B != null)
            {
                writer.WriteBase64StringValue(attribute.B.ToArray());
            }
            else if (attribute.BOOL != null)
            {
                writer.WriteBooleanValue(attribute.BOOL.Value);
            }
            else if (attribute.NULL != null)
            {
                writer.WriteNullValue();
            }
            else if (attribute.M != null)
            {
                WriteJson(writer, attribute.M);
            }
            else if (attribute.L != null)
            {
                writer.WriteStartArray();
                foreach (var item in attribute.L)
                {
                    WriteJsonValue(writer, item);
                }
                writer.WriteEndArray();
            }
            else if (attribute.SS != null)
            {
                writer.WriteStartArray();
                foreach (var item in attribute.SS)
                {
                    writer.WriteStringValue(item);
                }
                writer.WriteEndArray();
            }
            else if (attribute.NS != null)
            {
                writer.WriteStartArray();
                foreach (var item in attribute.NS)
                {
                    writer.WriteRawValue(item);
                }
                writer.WriteEndArray();
            }
            else if (attribute.BS != null)
            {
                writer.WriteStartArray();
                foreach (var item in attribute.BS)
                {
                    writer.WriteBase64StringValue(item.ToArray());
                }
                writer.WriteEndArray();
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}