namespace WSChat.Domain.Enums;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChatType
{
    Private,
    Group,
    SuperGroup
}
