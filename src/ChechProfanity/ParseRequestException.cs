using System;

namespace CheckProfanityAwsLambda
{
    public class ParseRequestException : Exception
    {
        public ParseRequestException(string message) : base(message)
        {
        }
    }
}