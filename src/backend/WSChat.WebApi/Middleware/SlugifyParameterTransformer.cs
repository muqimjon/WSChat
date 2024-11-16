using System.Text.RegularExpressions;

namespace WSChat.WebSocketApi.Middlewares;

#nullable disable
public partial class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string TransformOutbound(object value)
        => value is null ? null : SlugyPattern().Replace(value.ToString(), "$1-$2").ToLowerInvariant();

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex SlugyPattern();
}
