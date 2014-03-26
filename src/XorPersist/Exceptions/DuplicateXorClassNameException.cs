using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LateNightStupidities.XorPersist.Exceptions
{
    /// <summary>
    /// This exception is thrown if two XorObject classes 
    /// define the same name in the XorClass attribute.
    /// </summary>
    public class DuplicateXorClassNameException : XorPersistException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateXorClassNameException" /> class.
        /// </summary>
        /// <param name="xorClassName">Name of the XorClass attribute.</param>
        /// <param name="existingType">The type that declared the XorClass name first.</param>
        /// <param name="newType">Another type that uses the same XorClass name.</param>
        public DuplicateXorClassNameException(string xorClassName, Type existingType, Type newType)
            : base(
                string.Format(Properties.Exceptions.DuplicateXorClassNameException, xorClassName, existingType, newType,
                    existingType.Assembly, newType.Assembly))
        {

        }
    }
}
