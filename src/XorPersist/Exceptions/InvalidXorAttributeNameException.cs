using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LateNightStupidities.XorPersist.Attributes;

namespace LateNightStupidities.XorPersist.Exceptions
{
    /// <summary>
    /// This exception is thrown when an invalid name is passed to an <see cref="XorAttribute"/> constructor.
    /// </summary>
    public class InvalidXorAttributeNameException : XorPersistException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidXorAttributeNameException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidXorAttributeNameException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidXorAttributeNameException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidXorAttributeNameException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
