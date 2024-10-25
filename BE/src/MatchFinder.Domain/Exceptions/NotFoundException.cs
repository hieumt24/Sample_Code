using MatchFinder.Domain.Resource;
using System.Net;

namespace MatchFinder.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public int ErrorCode { get; set; } = (int)HttpStatusCode.NotFound;

        public NotFoundException() : base(ResourceENG.Error_NotFound)
        {
        }

        public NotFoundException(int errorCode)
        {
            ErrorCode = errorCode;
        }

        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}