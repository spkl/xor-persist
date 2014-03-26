using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LateNightStupidities.XorPersist.Exceptions
{
    /// <summary>
    /// This exception is thrown if class derived from XorObject 
    /// does not declare a public parameterless constructor.
    /// </summary>
    public class CtorMissingException : XorPersistException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CtorMissingException" /> class.
        /// </summary>
        /// <param name="objectType">Type of the object where the constructor is missing.</param>
        /// <param name="innerException">The inner exception.</param>
        public CtorMissingException(Type objectType, Exception innerException)
            : base(string.Format(Properties.Exceptions.CtorMissingException, objectType), innerException)
        {
            
        }
    }
}
