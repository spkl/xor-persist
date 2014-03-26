using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LateNightStupidities.XorPersist.Exceptions
{
    /// <summary>
    /// This exception is thrown if a file is not valid 
    /// according to the XorPersist XSD.
    /// </summary>
    public class SchemaValidationException : XorPersistException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaValidationException" /> class.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="position">The line position.</param>
        /// <param name="message">The XML validation message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SchemaValidationException(int line, int position, string message, Exception innerException)
            : base(string.Format(Properties.Exceptions.SchemaValidationException, line, position, message), innerException)
        {

        }
    }
}
