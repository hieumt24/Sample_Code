﻿namespace MatchFinder.Domain.Models
{
    public class GeneralCreateResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; } = null;
    }
}