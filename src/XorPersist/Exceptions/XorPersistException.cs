using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LateNightStupidities.XorPersist.Exceptions
{
    /// <summary>
    /// Base class for Exceptions thrown be the XorPersist library.
    /// </summary>
    public class XorPersistException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XorPersistException" /> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public XorPersistException(string message) 
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XorPersistException" /> class
        /// with a specified error message and a reference to the inner exception that
        /// is the cause of this exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public XorPersistException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }
    }
}
