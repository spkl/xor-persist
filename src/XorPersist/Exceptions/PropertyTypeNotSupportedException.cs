using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LateNightStupidities.XorPersist.Exceptions
{
    /// <summary>
    /// This exception is thrown if the type of a property is not supported by XorPersist.
    /// </summary>
    public class PropertyTypeNotSupportedException : XorPersistException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyTypeNotSupportedException" /> class.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="ownerType">Type where the property is defined.</param>
        /// <param name="xorPropertyName">Name of the XorProperty.</param>
        /// <param name="classPropertyName">Name of the property at the class.</param>
        public PropertyTypeNotSupportedException(Type propertyType, Type ownerType, string xorPropertyName,
            string classPropertyName)
            : base(
                string.Format(Properties.Exceptions.PropertyTypeNotSupportedException, propertyType, ownerType,
                    xorPropertyName, classPropertyName))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyTypeNotSupportedException" /> class.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="propertyElement">The property element.</param>
        public PropertyTypeNotSupportedException(Type propertyType, XElement propertyElement)
            : base(
                string.Format(Properties.Exceptions.PropertyTypeNotSupportedException2, propertyType, propertyElement))
        {

        }
    }
}
