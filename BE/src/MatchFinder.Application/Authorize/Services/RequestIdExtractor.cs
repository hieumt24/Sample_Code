using MatchFinder.Application.Authorize.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace MatchFinder.Application.Services
{
    public class RequestIdExtractor : IRequestIdExtractor
    {
        public async Task<string> ExtractRequestIdAsync(HttpRequest request)
        {
            // Check route values
            if (request.RouteValues.TryGetValue("id", out var routeRequestId))
            {
                return routeRequestId?.ToString();
            }

            // Check query string
            if (request.Query.TryGetValue("id", out var queryRequestId))
            {
                return queryRequestId.ToString();
            }

            // Check form data
            if (request.HasFormContentType)
            {
                var form = await request.ReadFormAsync();
                if (form.TryGetValue("id", out var formRequestId))
                {
                    return formRequestId.ToString();
                }
            }

            // Check JSON body
            if (request.Method != "GET" && request.ContentType != null && request.ContentType.StartsWith("application/json"))
            {
                request.EnableBuffering();
                using var reader = new StreamReader(request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0;
                try
                {
                    var jsonDocument = JsonDocument.Parse(body);
                    if (jsonDocument.RootElement.TryGetProperty("id", out var bodyRequestId))
                    {
                        switch (bodyRequestId.ValueKind)
                        {
                            case JsonValueKind.String:
                                return bodyRequestId.GetString();

                            case JsonValueKind.Number:
                                return bodyRequestId.GetInt32().ToString();

                            default:
                                return null;
                        }
                    }
                }
                catch (JsonException)
                {
                }
            }

            return null;
        }
    }
}