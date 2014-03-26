using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LateNightStupidities.XorPersist.Exceptions
{
    /// <summary>
    /// This exception is thrown when encountering a class without 
    /// the XorClass attribute while saving the model.
    /// </summary>
    public class ClassAttributeMissingException : XorPersistException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassAttributeMissingException" /> class.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        public ClassAttributeMissingException(Type objectType)
            : base(string.Format(Properties.Exceptions.ClassAttributeMissingException, objectType))
        {

        }
    }
}
