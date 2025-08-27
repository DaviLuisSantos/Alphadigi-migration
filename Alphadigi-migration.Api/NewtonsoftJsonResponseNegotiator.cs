namespace Carter.ResponseNegotiators.SystemTextJson;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

/// <summary>
/// A System.Text.Json implementation of <see cref="IResponseNegotiator"/>
/// </summary>
public class SystemTextJsonResponseNegotiator : IResponseNegotiator
{
    private readonly JsonSerializerOptions jsonOptions;

    public SystemTextJsonResponseNegotiator()
    {
        this.jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public bool CanHandle(MediaTypeHeaderValue accept)
    {
        return accept.MediaType.ToString().IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public Task Handle<T>(HttpRequest req, HttpResponse res, T model, CancellationToken cancellationToken)
    {
        res.ContentType = "application/json; charset=utf-8";
        return res.WriteAsync(JsonSerializer.Serialize(model, this.jsonOptions), cancellationToken);
    }
}