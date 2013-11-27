using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using LateNightStupidities.XorPersist.Attributes;
using LateNightStupidities.XorPersist.Schema;

namespace LateNightStupidities.XorPersist
{
    /// <summary>
    /// Controller class for saving and loading <see cref="XorObject"/> data structures.
    /// </summary>
    public class XorController : IXorController
    {
        private readonly Dictionary<Guid, XorObject> objects;
        private Dictionary<string, Type> typeMapping;

        /// <summary>
        /// Gets a new <see cref="IXorController"/> instance.
        /// </summary>
        public static IXorController Get()
        {
            return new XorController();
        }

        private XorController()
        {
            objects = new Dictionary<Guid, XorObject>();
        }

        internal void RegisterObject(XorObject obj)
        {
            objects.Add(obj.XorId, obj);
        }

        internal Type GetTypeForName(string name)
        {
            return typeMapping[name];
        }

        private void BuildTypeDictionary()
        {
            typeMapping = new Dictionary<string, Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attribute = (XorClassAttribute)Attribute.GetCustomAttribute(type, typeof(XorClassAttribute));
                    if (attribute != null)
                    {
                        typeMapping.Add(attribute.Name, type);
                    }
                }
            }
        }

        private void ResolveReferences()
        {
            foreach (var xorObject in objects.Values)
            {
                xorObject.ResolveReferences(objects);
            }
        }

        private void InitializeObjects()
        {
            foreach (var xorObject in objects.Values)
            {
                xorObject.Initialize();
            }
        }

        #region Load

        /// <summary>
        /// Loads an <see cref="XorObject"/> data structure from the specified file path.
        /// </summary>
        /// <typeparam name="T">The type of the root object you are loading.</typeparam>
        /// <param name="path">The path.</param>
        /// <returns>The root object of the loaded data structure.</returns>
        public T Load<T>(string path) where T : XorObject
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Load<T>(fileStream);
            }
        }

        /// <summary>
        /// Loads an <see cref="XorObject"/> data structure from the specified XML stream.
        /// </summary>
        /// <typeparam name="T">The type of the root object you are loading.</typeparam>
        /// <param name="xmlStream">The XML stream.</param>
        /// <returns>The root object of the loaded data structure.</returns>
        public T Load<T>(Stream xmlStream) where T : XorObject
        {
            return Load<T>(XDocument.Load(xmlStream));
        }

        /// <summary>
        /// Loads an <see cref="XorObject"/> data structure from the specified <see cref="XDocument"/>.
        /// </summary>
        /// <typeparam name="T">The type of the root object your are loading.</typeparam>
        /// <param name="xDocument">The <see cref="XDocument"/>.</param>
        /// <returns>The root object of the loaded data structure.</returns>
        public T Load<T>(XDocument xDocument) where T : XorObject
        {
            // Validate the document
            Validate(xDocument);

            BuildTypeDictionary();

            var rootElement = xDocument.Root;
            var staticInfoElement = rootElement.Element(XorXsd.StaticInfo);
            var contentElement = rootElement.Element(XorXsd.Content);

            var rootObjectElement = contentElement.Element(XorXsd.Object);
            XorObject rootObject = XorObject.LoadFromElement(rootObjectElement, this);

            ResolveReferences();
            InitializeObjects();

            objects.Clear();

            return (T)rootObject;
        }

        

        #endregion

        #region Save

        /// <summary>
        /// Saves the data structure contained in the root <see cref="XorObject"/> to the specified file path.
        /// </summary>
        /// <param name="rootObject">The root object.</param>
        /// <param name="path">The file path.</param>
        public void Save(XorObject rootObject, string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(rootObject, fileStream);
            }
        }

        /// <summary>
        /// Saves the data structure contained in the root <see cref="XorObject"/> to the specified XML stream.
        /// </summary>
        /// <param name="rootObject">The root object.</param>
        /// <param name="xmlStream">The XML stream.</param>
        public void Save(XorObject rootObject, Stream xmlStream)
        {
            var xDocument = new XDocument();

            var rootElement = new XElement(XorXsd.Root);
            xDocument.Add(rootElement);

            var staticInfoElement = new XElement(XorXsd.StaticInfo);
            rootElement.Add(staticInfoElement);

            var contentElement = new XElement(XorXsd.Content);
            rootElement.Add(contentElement);

            rootObject.SaveTo(contentElement);

            xDocument.Save(xmlStream);
            xmlStream.Flush();
        }

        #endregion

        #region Schema validation

        /// <summary>
        /// Validates the specified XML stream with the XorPersist XML schema.
        /// </summary>
        /// <param name="xmlStream">The XML stream.</param>
        public void Validate(Stream xmlStream)
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string schemaResourceName = "LateNightStupidities.XorPersist.Schema.XorPersist.xsd";

            using (Stream schemaStream = assembly.GetManifestResourceStream(schemaResourceName))
            {
                XmlSchema schema = XmlSchema.Read(schemaStream, null);
                var settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags = XmlSchemaValidationFlags.None;
                settings.Schemas.Add(schema);
                settings.ValidationEventHandler += SchemaValidationHandler;
                
                using (var reader = XmlReader.Create(xmlStream, settings))
                {
                    while (reader.Read());
                }
            }
        }

        /// <summary>
        /// Validates the specified XDocument with the XorPersist XML schema.
        /// </summary>
        /// <param name="xDocument">The XDocument.</param>
        public void Validate(XDocument xDocument)
        {
            using (var stream = new MemoryStream())
            {
                xDocument.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                Validate(stream);
            }
        }

        /// <summary>
        /// Validates the file with the specified file path with the XorPersist XML schema.
        /// </summary>
        /// <param name="path">The path.</param>
        public void Validate(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Validate(fileStream);
            }
        }

        private void SchemaValidationHandler(object sender, ValidationEventArgs validationEventArgs)
        {
            // TODO Custom exception
            throw new Exception(
                string.Format("The data does not conform to the XorPersist XML schema. Line {0}, Position {1}: {2}",
                    validationEventArgs.Exception.LineNumber, validationEventArgs.Exception.LinePosition,
                    validationEventArgs.Message), validationEventArgs.Exception);
        }

        #endregion
    }
}