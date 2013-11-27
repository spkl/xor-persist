using System;
using System.Linq;
using System.Collections.Generic;
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
                        if (member.Attr.ListItemType.IsSupportedXorType()
                            || member.Attr.ListItemType.IsInterface)
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

                    for (int i = 0; i < ids.Length; i++)
                    {
                        XorObject referencedObject;
                        if (objects.TryGetValue(ids[i], out referencedObject))
                        {
                            referencedObjects[i] = referencedObject;
                        }
                        // TODO Exception?
                    }

                    member.Info.SetMemberValue(this, referencedObjects);
                }
            }
        }

        #region Load methods

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

        private void LoadObjectContents(XElement objectElement, XorController controller)
        {
            foreach (var propertyElement in objectElement.Elements(XorXsd.Property))
            {
                XorMultiplicity multiplicity;
                string memberName;
                propertyElement.GetNameAndMultiplicity(out memberName, out multiplicity);

                var member = GetType().GetXorPropertyMembers().Single(tuple => tuple.Attr.Name == memberName);

                LoadPropertyContents(propertyElement, member, multiplicity, controller);
            }

            foreach (var referenceElement in objectElement.Elements(XorXsd.Reference))
            {
                XorMultiplicity multiplicity;
                string memberName;
                referenceElement.GetNameAndMultiplicity(out memberName, out multiplicity);

                var member = GetType().GetXorReferenceMembers().Single(tuple => tuple.Attr.Name == memberName);

                LoadReferenceContents(referenceElement, member, multiplicity);
                this.referenceInformation.Add(member);
            }
        }

        private void LoadPropertyContents(XElement propertyElement, XorPropertyTuple member, XorMultiplicity multiplicity, XorController controller)
        {
            if (propertyElement.Name.LocalName != XorXsd.Property)
                throw new ArgumentException();

            if (multiplicity == XorMultiplicity.Single)
            {
                var type = member.Info.GetMemberInfoType();

                if (type.IsSupportedSimpleType())
                {
                    var value = propertyElement.ConvertToType(type);
                    member.Info.SetMemberValue(this, value);
                }
                else if (type.IsSupportedXorType() || type.IsInterface) // TODO Produce warning
                {
                    var objectElement = propertyElement.Element(XorXsd.Object);
                    XorObject childObject = LoadFromElement(objectElement, controller);
                    member.Info.SetMemberValue(this, childObject);
                }
                else
                {
                    // TODO Custom exception
                    throw new Exception(string.Format("Property type ({0}) is not supported. Class: {1}. Property: {2}.", type, GetType(), member.Attr.Name));
                }
            }
            else if (multiplicity == XorMultiplicity.List)
            {
                var type = member.Attr.ListItemType;
                int itemCount = propertyElement.Elements().Count();

                if (type.IsSupportedSimpleType())
                {
                    var list = new List<object>(itemCount);

                    foreach (var itemElement in propertyElement.Elements())
                    {
                        if (itemElement.Name.LocalName == XorXsd.ListItemNull)
                        {
                            list.Add(null);
                        }
                        else if (itemElement.Name.LocalName == XorXsd.ListItemNonNull)
                        {
                            var value = itemElement.ConvertToType(type);
                            list.Add(value);
                        }
                    }

                    member.Info.SetMemberValue(this, list);
                }
                else if (type.IsSupportedXorType() || type.IsInterface) // TODO Produce warning
                {
                    var list = new List<XorObject>(itemCount);

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

                    member.Info.SetMemberValue(this, list);
                }
                else
                {
                    // TODO Custom exception
                    throw new Exception(string.Format("Property type ({0}) is not supported. Class: {1}. Property: {2}.", type, GetType(), member.Attr.Name));
                }
            }
        }

        private static void LoadReferenceContents(XElement referenceElement, XorReferenceTuple member, XorMultiplicity multiplicity)
        {
            if (referenceElement.Name.LocalName != XorXsd.Reference)
                throw new ArgumentException();

            if (multiplicity == XorMultiplicity.Single)
            {
                var xorId = Guid.Parse(referenceElement.Value);
                member.Attr.AddId(xorId);
            }
            else if (multiplicity == XorMultiplicity.List)
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
        }

        #endregion

        #region Save methods

        /// <summary>
        /// Saves this XorObject and all children to the supplied element.
        /// </summary>
        /// <param name="contentElement">The content element.</param>
        internal void SaveTo(XElement contentElement)
        {
            var classAttribute = (XorClassAttribute)Attribute.GetCustomAttribute(GetType(), typeof(XorClassAttribute));

            if (classAttribute == null)
            {
                // TODO Custom exception
                throw new Exception("XorClass attribute is missing on type " + GetType() + ".");
            }

            var mainElement = new XElement(XorXsd.Object);
            mainElement.SetAttributeValue(XorXsd.ClassName, classAttribute.Name);
            contentElement.Add(mainElement);

            SaveXorProperties(mainElement);
            SaveXorReferences(mainElement);
        }

        /// <summary>
        /// Saves the XorReferences.
        /// </summary>
        /// <param name="mainElement">The main element.</param>
        private void SaveXorReferences(XElement mainElement)
        {
            foreach (var member in GetType().GetXorReferenceMembers().OrderBy(m => m.Attr.Name))
            {
                if (member.Attr.Multiplicity == XorMultiplicity.Single)
                {
                    var referencedObject = (XorObject) member.Info.GetMemberValue(this);
                    if (referencedObject != null)
                    {
                        var refElement = CreateReferenceElement(referencedObject, member);
                        mainElement.Add(refElement);
                    }
                }
                else if (member.Attr.Multiplicity == XorMultiplicity.List)
                {
                    var refList = (IEnumerable) member.Info.GetMemberValue(this);
                    if (refList != null)
                    {
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
        }

        /// <summary>
        /// Saves the XorProperties.
        /// </summary>
        /// <param name="mainElement">The main element.</param>
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

                    var listElement = CreateListElement(member);
                    var listItemType = member.Attr.ListItemType;
                    
                    bool supportedSimpleType = listItemType.IsSupportedSimpleType();
                    bool supportedXorType = !supportedSimpleType && (listItemType.IsSupportedXorType() || listItemType.IsInterface); // TODO Produce warning for interface

                    foreach (object listItem in list)
                    {
                        XElement listItemElement;

                        if (supportedSimpleType)
                        {
                            listItemElement = CreateSimpleListItemElement(listItem);
                        }
                        else if (supportedXorType)
                        {
                            listItemElement = CreateXorTypeListItemElement((XorObject)listItem);
                        }
                        else
                        {
                            // TODO Custom exception
                            throw new Exception(
                                string.Format("Property type ({0}) is not supported. Class: {1}. Property: {2}.",
                                    member.Info.GetMemberInfoType(), GetType(), member.Attr.Name));
                        }

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
            if (member.Attr.Multiplicity != XorMultiplicity.Single)
            {
                refElement.SetAttributeValue(XorXsd.Multiplicity, member.Attr.Multiplicity);
            }

            return refElement;
        }

        /// <summary>
        /// Creates an XElement that can store an object reference list.
        /// </summary>
        /// <param name="member">The member.</param>
        private static XElement CreateReferenceListElement(XorReferenceTuple member)
        {
            var refListElement = new XElement(XorXsd.Reference);
            refListElement.SetAttributeValue(XorXsd.MemberName, member.Attr.Name);
            if (member.Attr.Multiplicity != XorMultiplicity.Single)
            {
                refListElement.SetAttributeValue(XorXsd.Multiplicity, member.Attr.Multiplicity);
            }

            return refListElement;
        }

        /// <summary>
        /// Creates an XElement that stores a simple-type property.
        /// </summary>
        /// <param name="propertyContent">Content of the property.</param>
        /// <param name="member">The member.</param>
        private static XElement CreateSimplePropertyElement(object propertyContent, XorPropertyTuple member)
        {
            // TODO unify this
            var propertyElement = new XElement(XorXsd.Property, propertyContent);
            propertyElement.SetAttributeValue(XorXsd.MemberName, member.Attr.Name);
            if (member.Attr.Multiplicity != XorMultiplicity.Single)
            {
                propertyElement.SetAttributeValue(XorXsd.Multiplicity, member.Attr.Multiplicity);
            }

            return propertyElement;
        }

        /// <summary>
        /// Creates an XElement that stores an XorObject.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="member">The member.</param>
        private static XElement CreateXorTypePropertyElement(XorObject obj, XorPropertyTuple member)
        {
            // TODO unify this
            var propertyElement = new XElement(XorXsd.Property);
            propertyElement.SetAttributeValue(XorXsd.MemberName, member.Attr.Name);
            if (member.Attr.Multiplicity != XorMultiplicity.Single)
            {
                propertyElement.SetAttributeValue(XorXsd.Multiplicity, member.Attr.Multiplicity);
            }
            obj.SaveTo(propertyElement);

            return propertyElement;
        }

        /// <summary>
        /// Creates an XElement that can store a simple-type or XorObject list.
        /// </summary>
        /// <param name="member">The member.</param>
        private static XElement CreateListElement(XorPropertyTuple member)
        {
            // TODO unify this
            var listElement = new XElement(XorXsd.Property);
            listElement.SetAttributeValue(XorXsd.MemberName, member.Attr.Name);
            if (member.Attr.Multiplicity != XorMultiplicity.Single)
            {
                listElement.SetAttributeValue(XorXsd.Multiplicity, member.Attr.Multiplicity);
            }

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