using MatchFinder.Application.Authorize.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace MatchFinder.Application.Authorize.Filters.InactiveTime
{
    public class InactiveTimeAuthorizeFilter : BaseAuthorizeFilter
    {
        private readonly IInactiveTimeAuthorizer _authorizer;

        public InactiveTimeAuthorizeFilter(IUserAuthenticator userAuthenticator, IRequestIdExtractor requestIdExtractor, IInactiveTimeAuthorizer inactiveTimeAuthorizer)
            : base(userAuthenticator, requestIdExtractor)
        {
            _authorizer = inactiveTimeAuthorizer;
        }

        public override async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            var request = context.HttpContext.Request;

            if (HttpMethods.IsPost(request.Method))
            {
                if (!_userAuthenticator.IsAuthenticated(user, out int userId))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var requestIdString = await ExtractRequestFieldIdAsync(request);
                if (!int.TryParse(requestIdString, out int requestId))
                {
                    context.Result = new BadRequestObjectResult("Invalid requestId format");
                    return;
                }

                if (user.IsInRole("Admin"))
                {
                    return;
                }
                else if (user.IsInRole("Staff"))
                {
                    if (!await _authorizer.IsAuthorizedStaffByFieldAsync(userId, requestId))
                    {
                        context.Result = new ForbidResult();
                    }
                    return;
                }

                if (!await _authorizer.IsAuthorizedByFieldAsync(userId, requestId))
                {
                    context.Result = new ForbidResult();
                }
            }
            else
            {
                var (isValid, userId, requestId) = await ValidateRequestAsync(context);
                if (!isValid) return;

                if (user.IsInRole("Admin"))
                {
                    return;
                }
                else if (user.IsInRole("Staff"))
                {
                    if (!await _authorizer.IsAuthorizedStaffAsync(userId, requestId))
                    {
                        context.Result = new ForbidResult();
                    }
                    return;
                }

                if (!await _authorizer.IsAuthorizedAsync(userId, requestId))
                {
                    context.Result = new ForbidResult();
                }
            }
        }

        public async Task<string> ExtractRequestFieldIdAsync(HttpRequest request)
        {
            if (request.RouteValues.TryGetValue("fieldId", out var routeRequestId))
            {
                return routeRequestId?.ToString();
            }

            if (request.Query.TryGetValue("fieldId", out var queryRequestId))
            {
                return queryRequestId.ToString();
            }

            if (request.HasFormContentType)
            {
                var form = await request.ReadFormAsync();
                if (form.TryGetValue("fieldId", out var formRequestId))
                {
                    return formRequestId.ToString();
                }
            }

            if (request.Method != "GET" && request.ContentType != null && request.ContentType.StartsWith("application/json"))
            {
                request.EnableBuffering();
                using var reader = new StreamReader(request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0;
                try
                {
                    var jsonDocument = JsonDocument.Parse(body);
                    if (jsonDocument.RootElement.TryGetProperty("fieldId", out var bodyRequestId))
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