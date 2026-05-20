using System;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;
using Kyameru.Component.DynamoDB.Contracts;

namespace Kyameru.Component.DynamoDB.Entities
{
    public abstract class DynamoRecord<TKey> : IDynamoRecord
    {
        [JsonIgnore]
        public abstract TKey HashKey { get; set; }
        
        [JsonIgnore]
        object IDynamoRecord.HashKey
        {
            get => HashKey!;
            set => HashKey = (TKey)value;
        }
    }
}