using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using System.Collections;
using LateNightStupidities.XorPersist.Attributes;
using LateNightStupidities.XorPersist.Schema;

namespace LateNightStupidities.XorPersist
{
    /// <summary>
    /// Abstract base class for all XorObject implementations.
    /// </summary>
    public abstract class XorObject
    {
        [XorProperty("_XorId")]
        internal Guid XorId { get; set; }

        private List<XorReferenceTuple> referenceInformation;

        #region Informative public properties

        /// <summary>
        /// Gets all <see cref="XorObject"/>s that are owned by this instance.
        /// (All <see cref="XorObject"/>s contained in properties with the <see cref="XorPropertyAttribute"/>.)
        /// </summary>
        /// <value>
        /// The owned <see cref="XorObject"/>s.
        /// </value>
        public IEnumerable<XorObject> OwnedXorObjects
        {
            get
            {
                foreach (var member in GetType().GetXorPropertyMembers())
                {
                    if (member.Attr.Multiplicity == XorMultiplicity.Single)
                    {
                        if (member.Info.GetMemberInfoType().IsSupportedXorType()
                            || member.Info.GetMemberInfoType().IsInterface)
                        {
                            var obj = (XorObject) member.Info.GetMemberValue(this);
                            if (obj != null)
                            {
                                yield return obj;
                            }
                        }
                    }
                    else if (member.Attr.Multiplicity == XorMultiplicity.List)
                    {
                        if (member.GetListItemType().IsSupportedXorType()
                            || member.GetListItemType().IsInterface)
                        {
                            var list = (IEnumerable) member.Info.GetMemberValue(this);
                            if (list != null)
                            {
                                foreach (XorObject obj in list)
                                {
                                    if (obj != null)
                                    {
                                        yield return obj;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets all <see cref="XorObject"/>s that are referenced by this instance.
        /// (All <see cref="XorObject"/>s contained in properties with the <see cref="XorReferenceAttribute"/>.)
        /// </summary>
        /// <value>
        /// The referenced <see cref="XorObject"/>s.
        /// </value>
        public IEnumerable<XorObject> ReferencedXorObjects
        {
            get
            {
                foreach (var member in GetType().GetXorReferenceMembers())
                {
                    if (member.Attr.Multiplicity == XorMultiplicity.Single)
                    {
                        var obj = (XorObject) member.Info.GetMemberValue(this);
                        if (obj != null)
                        {
                            yield return obj;
                        }
                    }
                    else if (member.Attr.Multiplicity == XorMultiplicity.List)
                    {
                        var list = (IEnumerable) member.Info.GetMemberValue(this);
                        if (list != null)
                        {
                            foreach (XorObject obj in list)
                            {
                                if (obj != null)
                                {
                                    yield return obj;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="XorObject" /> class
        /// and assigns it a random <see cref="Guid"/>.
        /// </summary>
        protected XorObject() : this(Guid.NewGuid())
        {
            
        }

        private XorObject(Guid id)
        {
            XorId = id;
            referenceInformation = new List<XorReferenceTuple>();
        }

        internal void Initialize()
        {
            // Clear reference resolving information
            referenceInformation = null;

            // Call custom initialization
            XorInitialize();
        }

        /// <summary>
        /// Initializes the <see cref="XorObject"/> after it was loaded
        /// from a file and after the references were resolved.
        /// Override in a child class to implement custom initialization behavior.
        /// </summary>
        protected virtual void XorInitialize()
        {
            
        }

        internal void Finish()
        {
            XorFinish();
        }

        /// <summary>
        /// Prepares the <see cref="XorObject"/> for saving.
        /// Called before the object is saved.
        /// Override in a child class to implement custom pre-saving behavior.
        /// </summary>
        protected virtual void XorFinish()
        {
            
        }

        /// <summary>
        /// Resolves the references of this XorObject.
        /// </summary>
        /// <param name="objects">The known objects.</param>
        internal void ResolveReferences(Dictionary<Guid, XorObject> objects)
        {
            foreach (var member in this.referenceInformation)
            {
                if (member.Attr.Multiplicity == XorMultiplicity.Single)
                {
                    XorObject referencedObject;
                    if (objects.TryGetValue(member.Attr.GetId(), out referencedObject))
                    {
                        member.Info.SetMemberValue(this, referencedObject);
                    }
                    // TODO Exception?
                }
                else if (member.Attr.Multiplicity == XorMultiplicity.List)
                {
                    var ids = member.Attr.GetIds().ToArray();
                    var referencedObjects = new XorObject[ids.Length];
                    var listItemType = member.GetListItemType();

                    for (int i = 0; i < ids.Length; i++)
                    {
                        XorObject referencedObject;
                        if (objects.TryGetValue(ids[i], out referencedObject))
                        {
                            referencedObjects[i] = referencedObject;
                        }
                        // TODO Exception?
                    }

                    SetCollectionMemberValue(member.Info, referencedObjects, listItemType);
                }
            }
        }

        #region Load methods

        /// <summary>
        /// Deserializes an <see cref="XorObject"/> from an <see cref="XElement"/>.
        /// </summary>
        /// <param name="objectElement">The object element.</param>
        /// <param name="controller">The controller.</param>
        /// <returns>The deserialized <see cref="XorObject"/>.</returns>
        /// <exception cref="System.ArgumentException">Supplied <see cref="XElement"/> is not an <see cref="XorXsd.Object"/> element.</exception>
        /// <exception cref="System.Exception">Parameterless constructor is missing on type.</exception>
        internal static XorObject LoadFromElement(XElement objectElement, XorController controller)
        {
            if (objectElement.Name.LocalName != XorXsd.Object)
                throw new ArgumentException();

            var objectType = controller.GetTypeForName(objectElement.Attribute(XorXsd.ClassName).Value);

            try
            {
                var obj = (XorObject)Activator.CreateInstance(objectType);

                obj.LoadObjectContents(objectElement, controller);
                controller.RegisterObject(obj);

                return obj;
            }
            catch (MissingMethodException mme)
            {
                // TODO Custom exception
                throw new Exception("Parameterless constructor is missing on type " + objectType + ".", mme);
            }
        }

        /// <summary>
        /// Loads the properties and references of this <see cref="XorObject"/> 
        /// from an <see cref="XElement"/> and sets them on this instance.
        /// </summary>
        /// <param name="objectElement">The object element.</param>
        /// <param name="controller">The controller.</param>
        private void LoadObjectContents(XElement objectElement, XorController controller)
        {
            foreach (var propertyElement in objectElement.Elements().Where(XorExtensions.IsPropertyElement))
            {
                string memberName = propertyElement.GetMemberName();

                var member = GetType().GetXorPropertyMembers().Single(tuple => tuple.Attr.Name == memberName);

                switch (propertyElement.Name.LocalName)
                {
                    case XorXsd.Property:
                        LoadSimpleProperty(propertyElement, member);
                        break;
                    case XorXsd.XProperty:
                        LoadXorTypeProperty(propertyElement, member, controller);
                        break;
                    case XorXsd.PropertyList:
                        LoadSimplePropertyList(propertyElement, member);
                        break;
                    case XorXsd.XPropertyList:
                        LoadXorTypePropertyList(propertyElement, member, controller);
                        break;
                }
            }

            foreach (var referenceElement in objectElement.Elements().Where(XorExtensions.IsReferenceElement))
            {
                string memberName = referenceElement.GetMemberName();

                var member = GetType().GetXorReferenceMembers().Single(tuple => tuple.Attr.Name == memberName);

                switch (referenceElement.Name.LocalName)
                {
                    case XorXsd.Reference:
                        LoadReference(referenceElement, member);
                        break;
                    case XorXsd.ReferenceList:
                        LoadReferenceList(referenceElement, member);
                        break;
                }

                this.referenceInformation.Add(member);
            }
        }

        /// <summary>
        /// Loads a property list of <see cref="XorObject"/>s from an <see cref="XElement"/>.
        /// </summary>
        /// <param name="propertyElement">The property element.</param>
        /// <param name="member">The member.</param>
        /// <param name="controller">The controller.</param>
        private void LoadXorTypePropertyList(XElement propertyElement, XorPropertyTuple member, XorController controller)
        {
            var list = new List<XorObject>(propertyElement.Elements().Count());
            var listItemType = member.GetListItemType();

            foreach (var itemElement in propertyElement.Elements())
            {
                if (itemElement.Name.LocalName == XorXsd.ListItemNull)
                {
                    list.Add(null);
                }
                else if (itemElement.Name.LocalName == XorXsd.ListItemNonNull)
                {
                    var objectElement = itemElement.Element(XorXsd.Object);
                    XorObject childObject = LoadFromElement(objectElement, controller);
                    list.Add(childObject);
                }
            }

            SetCollectionMemberValue(member.Info, list, listItemType);
        }

        /// <summary>
        /// Loads a property list of simple values from an <see cref="XElement"/>.
        /// </summary>
        /// <param name="propertyElement">The property element.</param>
        /// <param name="member">The member.</param>
        private void LoadSimplePropertyList(XElement propertyElement, XorPropertyTuple member)
        {
            var list = new List<object>(propertyElement.Elements().Count());
            var listItemType = member.GetListItemType();

            foreach (var itemElement in propertyElement.Elements())
            {
                if (itemElement.Name.LocalName == XorXsd.ListItemNull)
                {
                    list.Add(null);
                }
                else if (itemElement.Name.LocalName == XorXsd.ListItemNonNull)
                {
                    var value = itemElement.ConvertToType(listItemType);
                    list.Add(value);
                }
            }

            SetCollectionMemberValue(member.Info, list, listItemType);
        }

        /// <summary>
        /// Converts the supplied list/collection to the target type 
        /// and sets is at the specified member.
        /// Casts the IEnumerable to an IEnumerable of <paramref name="listItemType"/>,
        /// if the type information is available.
        /// </summary>
        /// <param name="memberInfo">The member to set.</param>
        /// <param name="list">The list/collection.</param>
        /// <param name="listItemType">Type of the list item, may be null if unknown.</param>
        private void SetCollectionMemberValue(MemberInfo memberInfo, IEnumerable list, Type listItemType)
        {
            if (listItemType != null)
            {
                var castedEnumerable = list.Cast(listItemType);
                SetCollectionMemberValue(memberInfo, castedEnumerable);
            }
            else
            {
                SetCollectionMemberValue(memberInfo, list);
            }
        }

        /// <summary>
        /// Converts the supplied list/collection to the target type
        /// and sets is at the specified member.
        /// Expects (if possible) a generic IEnumerable of the correct
        /// list item type as <paramref name="castedEnumerable"/>.
        /// </summary>
        /// <param name="memberInfo">The member to set.</param>
        /// <param name="castedEnumerable">The casted enumerable.</param>
        private void SetCollectionMemberValue(MemberInfo memberInfo, IEnumerable castedEnumerable)
        {
            var type = memberInfo.GetMemberInfoType();

            if (type == typeof(ArrayList) || type == typeof(ICollection) || type == typeof(IList))
            {
                // Use ArrayList for the interfaces as well
                memberInfo.SetMemberValue(this, new ArrayList(castedEnumerable.ToObjectList()));
                return;
            }

            if (type == typeof(Queue))
            {
                memberInfo.SetMemberValue(this, new Queue(castedEnumerable.ToObjectList()));
                return;
            }

            if (type == typeof(Stack))
            {
                // Stack constructor reverses the order
                var reversed = castedEnumerable.ToObjectList();
                reversed.Reverse();
                memberInfo.SetMemberValue(this, new Stack(reversed));
                return;
            }

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var list = castedEnumerable.ToObjectList();

                // Create an array of the element type and
                // transfer the values from the list
                var array = Array.CreateInstance(elementType, list.Count);
                for (int i = 0; i < array.Length; i++)
                {
                    array.SetValue(list[i], i);
                }

                memberInfo.SetMemberValue(this, array);
                return;
            }

            if (type.IsGenericType)
            {
                // Extract generic base type from supplied type
                var genericTypeDefinition = type.GetGenericTypeDefinition();

                if (genericTypeDefinition == typeof(List<>) 
                    || genericTypeDefinition == typeof(HashSet<>) 
                    || genericTypeDefinition == typeof(SortedSet<>) 
                    || genericTypeDefinition == typeof(Queue<>) 
                    || genericTypeDefinition == typeof(Stack<>)
                    || genericTypeDefinition == typeof(LinkedList<>))
                {
                    if (genericTypeDefinition == typeof(Stack<>))
                    {
                        // Stack constructor reverses the order
                        var reversed = castedEnumerable.ToObjectList();
                        reversed.Reverse();
                        castedEnumerable = reversed.Cast(type.GetGenericArguments().Single());
                    }

                    // Create an instance of the type.
                    // All these classes have a constructor that takes
                    // a typed (generic) IEnumerable as argument.
                    var instance = Activator.CreateInstance(type, castedEnumerable);
                    memberInfo.SetMemberValue(this, instance);
                    return;
                }

                if (genericTypeDefinition == typeof(ICollection<>)
                    || genericTypeDefinition == typeof(IList<>))
                {
                    // Create the List<T> type from the ICollection<T>/IList<T>,
                    // then instantiate a list with the IEnumerable<T> constructor.
                    var instance = Activator.CreateInstance(
                        typeof(List<>).MakeGenericType(type.GetGenericArguments()), castedEnumerable);
                    memberInfo.SetMemberValue(this, instance);
                    return;
                }

                if (genericTypeDefinition == typeof(ISet<>))
                {
                    // Create the HashSet<T> type from the ISet<T>,
                    // then instantiate a set with the IEnumerable<T> constructor.
                    var instance = Activator.CreateInstance(
                        typeof(HashSet<>).MakeGenericType(type.GetGenericArguments()), castedEnumerable);
                    memberInfo.SetMemberValue(this, instance);
                    return;
                }
            }

            // This is used for IEnumerable and IEnumerable<T>.
            memberInfo.SetMemberValue(this, castedEnumerable);
        }

        /// <summary>
        /// Loads a property that contains an <see cref="XorObject"/> from an <see cref="XElement"/>.
        /// </summary>
        /// <param name="propertyElement">The property element.</param>
        /// <param name="member">The member.</param>
        /// <param name="controller">The controller.</param>
        private void LoadXorTypeProperty(XElement propertyElement, XorPropertyTuple member, XorController controller)
        {
            var objectElement = propertyElement.Element(XorXsd.Object);
            XorObject childObject = LoadFromElement(objectElement, controller);
            member.Info.SetMemberValue(this, childObject);
        }

        /// <summary>
        /// Loads a property that contains a simple value from an <see cref="XElement"/>.
        /// </summary>
        /// <param name="propertyElement">The property element.</param>
        /// <param name="member">The member.</param>
        private void LoadSimpleProperty(XElement propertyElement, XorPropertyTuple member)
        {
            var type = member.Info.GetMemberInfoType();
            var value = propertyElement.ConvertToType(type);
            member.Info.SetMemberValue(this, value);
        }

        /// <summary>
        /// Loads a reference list of <see cref="XorObject"/>s from an <see cref="XElement"/>.
        /// </summary>
        /// <param name="referenceElement">The reference element.</param>
        /// <param name="member">The member.</param>
        private static void LoadReferenceList(XElement referenceElement, XorReferenceTuple member)
        {
            foreach (var itemElement in referenceElement.Elements())
            {
                if (itemElement.Name.LocalName == XorXsd.ListItemNull)
                {
                    member.Attr.AddId(default(Guid));
                }
                else if (itemElement.Name.LocalName == XorXsd.ListItemNonNull)
                {
                    var xorId = Guid.Parse(itemElement.Value);
                    member.Attr.AddId(xorId);
                }
            }
        }

        /// <summary>
        /// Loads a reference to an <see cref="XorObject"/> from an <see cref="XElement"/>.
        /// </summary>
        /// <param name="referenceElement">The reference element.</param>
        /// <param name="member">The member.</param>
        private static void LoadReference(XElement referenceElement, XorReferenceTuple member)
        {
            var xorId = Guid.Parse(referenceElement.Value);
            member.Attr.AddId(xorId);
        }

        #endregion

        #region Save methods

        /// <summary>
        /// Saves this XorObject and all children to the supplied element.
        /// </summary>
        /// <param name="contentElement">The content element.</param>
        /// <exception cref="System.Exception">XorClass attribute is missing on type.</exception>
        internal void SaveTo(XElement contentElement)
        {
            var classAttribute = (XorClassAttribute)Attribute.GetCustomAttribute(GetType(), typeof(XorClassAttribute));

            if (classAttribute == null)
            {
                // TODO Custom exception
                throw new Exception("XorClass attribute is missing on type " + GetType() + ".");
            }

            Finish();

            var mainElement = new XElement(XorXsd.Object);
            mainElement.SetAttributeValue(XorXsd.ClassName, classAttribute.Name);
            contentElement.Add(mainElement);

            SaveXorProperties(mainElement);
            SaveXorReferences(mainElement);
        }

        /// <summary>
        /// Saves the XorReferences to the supplied <see cref="XorXsd.Object"/> element.
        /// </summary>
        /// <param name="mainElement">The main element.</param>
        private void SaveXorReferences(XElement mainElement)
        {
            foreach (var member in GetType().GetXorReferenceMembers().OrderBy(m => m.Attr.Name))
            {
                if (member.Attr.Multiplicity == XorMultiplicity.Single)
                {
                    var referencedObject = (XorObject) member.Info.GetMemberValue(this);
                    if (referencedObject == null) continue; // Skip null references

                    var refElement = CreateReferenceElement(referencedObject, member);
                    mainElement.Add(refElement);
                }
                else if (member.Attr.Multiplicity == XorMultiplicity.List)
                {
                    var refList = (IEnumerable) member.Info.GetMemberValue(this);
                    if (refList == null) continue; // Skip null reference lists

                    var refListElement = CreateReferenceListElement(member);

                    foreach (XorObject xorObject in refList)
                    {
                        Guid? id = null;
                        if (xorObject != null)
                        {
                            id = xorObject.XorId;
                        }

                        var refListItemElement = CreateSimpleListItemElement(id);
                        refListElement.Add(refListItemElement);
                    }
                        
                    mainElement.Add(refListElement);
                }
            }
        }

        /// <summary>
        /// Saves the XorProperties to the supplied <see cref="XorXsd.Object" /> element.
        /// </summary>
        /// <param name="mainElement">The main element.</param>
        /// <exception cref="System.Exception">Property type is not supported.</exception>
        private void SaveXorProperties(XElement mainElement)
        {
            foreach (var member in GetType().GetXorPropertyMembers().OrderBy(m => m.Attr.Name))
            {
                if (member.Attr.Multiplicity == XorMultiplicity.Single)
                {
                    object memberValue = member.Info.GetMemberValue(this);
                    if (memberValue == null) continue; // Skip null properties

                    var propertyType = member.Info.GetMemberInfoType();
                    XElement propertyElement;

                    if (propertyType.IsSupportedSimpleType())
                    {
                        propertyElement = CreateSimplePropertyElement(memberValue, member);
                    }
                    else if (propertyType.IsSupportedXorType() || (memberValue.GetType().IsSupportedXorType()))
                    {
                        propertyElement = CreateXorTypePropertyElement((XorObject)memberValue, member);
                    }
                    else
                    {
                        // TODO Custom exception
                        throw new Exception(
                            string.Format("Property type ({0}) is not supported. Class: {1}. Property: {2}.",
                                member.Info.GetMemberInfoType(), GetType(), member.Attr.Name));
                    }

                    mainElement.Add(propertyElement);
                }
                else if (member.Attr.Multiplicity == XorMultiplicity.List)
                {
                    var list = (IEnumerable)member.Info.GetMemberValue(this);
                    if (list == null) continue; // Skip null lists

                    var listItemType = member.GetListItemType();

                    bool supportedSimpleType = listItemType.IsSupportedSimpleType();
                    bool supportedXorType = !supportedSimpleType && (listItemType.IsSupportedXorType() || listItemType.IsInterface);

                    // TODO Produce warning for interface

                    XElement listElement;
                    if (supportedSimpleType)
                    {
                        listElement = CreateSimpleListElement(member);
                    }
                    else if (supportedXorType)
                    {
                        listElement = CreateXorTypeListElement(member);
                    }
                    else
                    {
                        // TODO Custom exception
                        throw new Exception(
                            string.Format("Property type ({0}) is not supported. Class: {1}. Property: {2}.",
                                member.Info.GetMemberInfoType(), GetType(), member.Attr.Name));
                    }

                    foreach (object listItem in list)
                    {
                        XElement listItemElement = supportedSimpleType
                            ? CreateSimpleListItemElement(listItem)
                            : CreateXorTypeListItemElement((XorObject) listItem);
                        
                        listElement.Add(listItemElement);
                    }

                    mainElement.Add(listElement);
                }
            }
        }

        #endregion

        #region Create element methods

        /// <summary>
        /// Creates an XElement that stores an object reference.
        /// </summary>
        /// <param name="referencedObject">The referenced object.</param>
        /// <param name="member">The member.</param>
        private static XElement CreateReferenceElement(XorObject referencedObject, XorReferenceTuple member)
        {
            var refElement = new XElement(XorXsd.Reference, referencedObject.XorId.ToString());
            refElement.SetAttributeValue(XorXsd.MemberName, member.Attr.Name);

            return refElement;
        }

        /// <summary>
        /// Creates an XElement that can store an object reference list.
        /// </summary>
        /// <param name="member">The member.</param>
        private static XElement CreateReferenceListElement(XorReferenceTuple member)
        {
            var refListElement = new XElement(XorXsd.ReferenceList);
            refListElement.SetAttributeValue(XorXsd.MemberName, member.Attr.Name);

            return refListElement;
        }

        /// <summary>
        /// Creates an XElement that stores a simple-type property.
        /// </summary>
        /// <param name="propertyContent">Content of the property.</param>
        /// <param name="member">The member.</param>
        private static XElement CreateSimplePropertyElement(object propertyContent, XorPropertyTuple member)
        {
            var propertyElement = new XElement(XorXsd.Property, propertyContent);
            propertyElement.SetAttributeValue(XorXsd.MemberName, member.Attr.Name);

            return propertyElement;
        }

        /// <summary>
        /// Creates an XElement that stores an XorObject.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="member">The member.</param>
        private static XElement CreateXorTypePropertyElement(XorObject obj, XorPropertyTuple member)
        {
            var propertyElement = new XElement(XorXsd.XProperty);
            propertyElement.SetAttributeValue(XorXsd.MemberName, member.Attr.Name);
            obj.SaveTo(propertyElement);

            return propertyElement;
        }

        /// <summary>
        /// Creates an XElement that can store a simple-type list.
        /// </summary>
        /// <param name="member">The member.</param>
        private static XElement CreateSimpleListElement(XorPropertyTuple member)
        {
            var listElement = new XElement(XorXsd.PropertyList);
            listElement.SetAttributeValue(XorXsd.MemberName, member.Attr.Name);

            return listElement;
        }

        /// <summary>
        /// Creates an XElement that can store a XorObject list.
        /// </summary>
        /// <param name="member">The member.</param>
        private static XElement CreateXorTypeListElement(XorPropertyTuple member)
        {
            var listElement = new XElement(XorXsd.XPropertyList);
            listElement.SetAttributeValue(XorXsd.MemberName, member.Attr.Name);

            return listElement;
        }

        /// <summary>
        /// Creates an XElement that stores a simple-type list item.
        /// </summary>
        /// <param name="simpleTypeValue">The simple type value.</param>
        private static XElement CreateSimpleListItemElement(object simpleTypeValue)
        {
            XElement listItemElement = simpleTypeValue != null 
                ? new XElement(XorXsd.ListItemNonNull, simpleTypeValue) 
                : new XElement(XorXsd.ListItemNull);

            return listItemElement;
        }

        /// <summary>
        /// Creates an XElement that stores an XorObject.
        /// </summary>
        /// <param name="xorObject">The xor object.</param>
        private static XElement CreateXorTypeListItemElement(XorObject xorObject)
        {
            XElement listItemElement;
            if (xorObject != null)
            {
                listItemElement = new XElement(XorXsd.ListItemNonNull);
                xorObject.SaveTo(listItemElement);
            }
            else
            {
                listItemElement = new XElement(XorXsd.ListItemNull);
            }

            return listItemElement;
        }

        #endregion
    }
}