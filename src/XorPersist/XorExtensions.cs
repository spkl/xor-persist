using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using LateNightStupidities.XorPersist.Attributes;
using LateNightStupidities.XorPersist.Exceptions;
using LateNightStupidities.XorPersist.Schema;

namespace LateNightStupidities.XorPersist
{
    internal static class XorExtensions
    {
        private const BindingFlags XorBindingFlags =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Gets the XorReference members (fields and properties).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IEnumerable<XorReferenceTuple> GetXorReferenceMembers(this Type type)
        {
            var fields = type.GetFields(XorBindingFlags);
            var properties = type.GetProperties(XorBindingFlags);
            var members = fields.Concat<MemberInfo>(properties).Where(member => member.DeclaringType == type);

            foreach (var memberInfo in members)
            {
                var attribute = (XorReferenceAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(XorReferenceAttribute), true);
                if (attribute != null)
                {
                    yield return new XorReferenceTuple(memberInfo, attribute);
                }
            }

            if (type.BaseType != null)
            {
                foreach (var xorReferenceMember in type.BaseType.GetXorReferenceMembers())
                {
                    yield return xorReferenceMember;
                }
            }
        }

        /// <summary>
        /// Gets the XorProperty members.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IEnumerable<XorPropertyTuple> GetXorPropertyMembers(this Type type)
        {
            var properties = type.GetProperties(XorBindingFlags);
            var fields = type.GetFields(XorBindingFlags);
            var members = properties.Concat<MemberInfo>(fields).Where(member => member.DeclaringType == type);

            foreach (var memberInfo in members)
            {
                var attribute = (XorPropertyAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(XorPropertyAttribute), true);
                if (attribute != null)
                {
                    yield return new XorPropertyTuple(memberInfo, attribute);
                }
            }

            if (type.BaseType != null)
            {
                foreach (var xorPropertyMember in type.BaseType.GetXorPropertyMembers())
                {
                    yield return xorPropertyMember;
                }
            }
        }

        /// <summary>
        /// Determines whether this type is a supported simple type (primitives, string, Guid, DateTime, TimeSpan, decimal).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if this is a supported simple type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSupportedSimpleType(this Type type)
        {
            return type.IsPrimitive || type == typeof (string) || type == typeof (Guid) || type == typeof (DateTime) ||
                   type == typeof (TimeSpan) || type == typeof (decimal);
        }

        /// <summary>
        /// Determines whether this is derived from XorObject.
        /// Attention: The type can be an interface that is not supported, 
        /// but a class implementing the interface may be supported!
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if this is derived from XorObject; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSupportedXorType(this Type type)
        {
            return typeof(XorObject).IsAssignableFrom(type);
        }

        /// <summary>
        /// Gets the value of a field or property info from an object.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <param name="obj">The object.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Only fields and properties are supported.</exception>
        public static object GetMemberValue(this MemberInfo memberInfo, object obj, object[] index = null)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) memberInfo).GetValue(obj);
                case MemberTypes.Property:
                    return ((PropertyInfo) memberInfo).GetValue(obj, index);
                default:
                    throw new ArgumentOutOfRangeException(nameof(memberInfo), "Only fields and properties are supported.");
            }
        }

        /// <summary>
        /// Sets the value of a field or property.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Only fields and properties are supported.</exception>
        public static void SetMemberValue(this MemberInfo memberInfo, object obj, object value, object[] index = null)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)memberInfo).SetValue(obj, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo)memberInfo).SetValue(obj, value, index);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(memberInfo), "Only fields and properties are supported.");
            }
        }

        /// <summary>
        /// Casts the specified enumerable to an <see cref="IEnumerable{T}"/> of <paramref name="type"/>.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="type">The type.</param>
        public static IEnumerable Cast(this IEnumerable enumerable, Type type)
        {
            var castMethod = typeof(Enumerable).GetMethod("Cast");
            castMethod = castMethod.MakeGenericMethod(type);
            return (IEnumerable)castMethod.Invoke(null, new object[] { enumerable });
        }

        /// <summary>
        /// Creates an object list from an enumerable.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        public static List<object> ToObjectList(this IEnumerable enumerable)
        {
            return enumerable.Cast<object>().ToList();
        }

        /// <summary>
        /// Determines whether the specified type is an <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is an <see cref="IEnumerable"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEnumerable(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether the specified type is an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is an <see cref="IEnumerable{T}"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTypedEnumerable(this Type type)
        {
            if (type.IsEnumerable() && type.IsGenericType)
            {
                return type.GetGenericArguments().Count() == 1;
            }

            return false;
        }

        /// <summary>
        /// Gets the type of the member info.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Only fields and properties are supported.</exception>
        public static Type GetMemberInfoType(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).PropertyType;
                default:
                    throw new ArgumentOutOfRangeException(nameof(memberInfo), "Only fields and properties are supported.");
            }
        }

        /// <summary>
        /// Gets the type of the list item.
        /// Tries to get the type from the <see cref="XorAttribute.ListItemType"/>.
        /// If that is not successful, the generic argument of the IEnumerable is extracted.
        /// </summary>
        /// <param name="member">The member.</param>
        public static Type GetListItemType(this XorPropertyTuple member)
        {
            return GetListItemType(member.Attr, member.Info);
        }

        /// <summary>
        /// Gets the type of the list item.
        /// Tries to get the type from the <see cref="XorAttribute.ListItemType"/>.
        /// If that is not successful, the generic argument of the IEnumerable is extracted.
        /// </summary>
        /// <param name="member">The member.</param>
        public static Type GetListItemType(this XorReferenceTuple member)
        {
            return GetListItemType(member.Attr, member.Info);
        }

        /// <summary>
        /// Gets the type of the list item.
        /// Tries to get the type from the <see cref="XorAttribute.ListItemType" />.
        /// If that is not successful, the generic argument of the IEnumerable is extracted.
        /// </summary>
        /// <param name="memberAttr">The XorAttribute.</param>
        /// <param name="memberInfo">The member info.</param>
        private static Type GetListItemType(XorAttribute memberAttr, MemberInfo memberInfo)
        {
            var listItemType = memberAttr.ListItemType;
            var memberInfoType = memberInfo.GetMemberInfoType();

            if (listItemType == null && memberInfoType.IsTypedEnumerable())
            {
                listItemType = memberInfoType.GetGenericArguments().Single();
            }

            if (listItemType == null && memberInfoType.IsArray)
            {
                listItemType = memberInfoType.GetElementType();
            }

            return listItemType;
        }

        /// <summary>
        /// Gets the name and multiplicity of a property XElement.
        /// </summary>
        /// <param name="propertyElement">The property element.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="multiplicity">The multiplicity.</param>
        public static void GetNameAndMultiplicity(this XElement propertyElement, out string memberName, out XorMultiplicity multiplicity)
        {
            memberName = propertyElement.Attribute(XorXsd.MemberName).Value;

            switch (propertyElement.Name.LocalName)
            {
                case XorXsd.Property:
                case XorXsd.XProperty:
                case XorXsd.Reference:
                    multiplicity = XorMultiplicity.Single;
                    break;
                case XorXsd.PropertyList:
                case XorXsd.XPropertyList:
                case XorXsd.ReferenceList:
                    multiplicity = XorMultiplicity.List;
                    break;
                default:
                    throw new Exception("GetNameAndMultiplicity: Unknown element name'" + propertyElement.Name + "'.");
            }
        }

        /// <summary>
        /// Gets the member name of a property XElement.
        /// </summary>
        /// <param name="propertyElement">The property element.</param>
        public static string GetMemberName(this XElement propertyElement)
        {
            XorMultiplicity multiplicity;
            string memberName;
            propertyElement.GetNameAndMultiplicity(out memberName, out multiplicity);
            return memberName;
        }

        /// <summary>
        /// Converts an XElement to the supplied type.
        /// </summary>
        /// <param name="propertyElement">The property element.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="System.Exception">Unsupported property type.</exception>
        public static object ConvertToType(this XElement propertyElement, Type type)
        {
            if (type == typeof(Boolean))
            {
                return (bool)propertyElement;
            }
            if (type == typeof(Byte))
            {
                return Byte.Parse(propertyElement.Value);
            }
            if (type == typeof(SByte))
            {
                return SByte.Parse(propertyElement.Value);
            }
            if (type == typeof(Int16)) // (short)
            {
                return Int16.Parse(propertyElement.Value);
            }
            if (type == typeof(Int32))
            {
                return (int)propertyElement;
            }
            if (type == typeof(Int64))
            {
                return (long)propertyElement;
            }
            if (type == typeof(UInt16)) // (ushort)
            {
                return UInt16.Parse(propertyElement.Value);
            }
            if (type == typeof(UInt32))
            {
                return (uint)propertyElement;
            }
            if (type == typeof(UInt64))
            {
                return (ulong)propertyElement;
            }
            if (type == typeof(Single))
            {
                return (float)propertyElement;
            }
            if (type == typeof(Double))
            {
                return (double)propertyElement;
            }
            if (type == typeof(string))
            {
                return (string)propertyElement;
            }
            if (type == typeof(Guid))
            {
                return (Guid)propertyElement;
            }
            if (type == typeof(DateTime))
            {
                return (DateTime)propertyElement;
            }
            if (type == typeof(TimeSpan))
            {
                return (TimeSpan)propertyElement;
            }
            if (type == typeof(decimal))
            {
                return (decimal)propertyElement;
            }

            throw new PropertyTypeNotSupportedException(type, propertyElement);
        }

        /// <summary>
        /// Determines whether the specified element is a reference element.
        /// Element names:
        /// * <see cref="XorXsd.Reference"/>
        /// * <see cref="XorXsd.ReferenceList"/>
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///   <c>true</c> if the specified element is a reference element; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsReferenceElement(this XElement element)
        {
            switch (element.Name.LocalName)
            {
                case XorXsd.Reference:
                case XorXsd.ReferenceList:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether the specified element is a property element.
        /// Element names:
        /// * <see cref="XorXsd.Property"/>
        /// * <see cref="XorXsd.PropertyList"/>
        /// * <see cref="XorXsd.XProperty"/>
        /// * <see cref="XorXsd.XPropertyList"/>
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///   <c>true</c> if the specified element is a property element; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPropertyElement(this XElement element)
        {
            switch (element.Name.LocalName)
            {
                case XorXsd.Property:
                case XorXsd.PropertyList:
                case XorXsd.XProperty:
                case XorXsd.XPropertyList:
                    return true;
                default:
                    return false;
            }
        }
    }
}
