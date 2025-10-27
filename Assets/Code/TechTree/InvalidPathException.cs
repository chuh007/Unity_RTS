using System;

namespace Code.TechTree
{
    public class InvalidPathException : Exception
    {
        public InvalidPathException(string attributeName) : base($"{attributeName} is invalid")
        {
            
        }
    }
}