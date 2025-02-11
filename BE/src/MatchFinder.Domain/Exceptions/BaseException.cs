﻿using Newtonsoft.Json;

namespace MatchFinder.Domain.Exceptions
{
    public class BaseException
    {
        public int ErrorCode { get; set; }

        public string? DevMessage { get; set; }

        public string? UserMessage { get; set; }

        public string? TraceId { get; set; }

        public string? MoreInfo { get; set; }

        public object? Errors { get; set; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}