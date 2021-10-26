using System;

namespace Pokedex.Exceptions
{
    public class ApplicationException : Exception, IApplicationException
    {
        public ApplicationException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
        private int ErrorCode { get; }

        public int GetCode()
        {
            return ErrorCode;
        }
        public string GetMessage()
        {
            return Message;
        }
    }
}
