using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LateNightStupidities.XorPersist.Attributes;

namespace LateNightStupidities.XorPersist.Exceptions
{
    /// <summary>
    /// This exception is thrown if a reference cannot be resolved when loading the model.
    /// </summary>
    public class CouldNotResolveReferenceException : XorPersistException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouldNotResolveReferenceException" /> class.
        /// </summary>
        /// <param name="memberInfo">The member where the reference is located.</param>
        /// <param name="xorId">The xor id that could not be found.</param>
        public CouldNotResolveReferenceException(MemberInfo memberInfo, Guid xorId)
            : base(string.Format(Properties.Exceptions.CouldNotResolveReferenceException, memberInfo, xorId))
        {

        }
    }
}
