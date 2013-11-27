using System.IO;
using System.Xml.Linq;

namespace LateNightStupidities.XorPersist
{
    /// <summary>
    /// Controller interface for saving and loading <see cref="XorObject"/> data structures.
    /// </summary>
    public interface IXorController
    {
        #region Load

        /// <summary>
        /// Loads an <see cref="XorObject"/> data structure from the specified file path.
        /// </summary>
        /// <typeparam name="T">The type of the root object you are loading.</typeparam>
        /// <param name="path">The path.</param>
        /// <returns>The root object of the loaded data structure.</returns>
        T Load<T>(string path) where T : XorObject;

        /// <summary>
        /// Loads an <see cref="XorObject"/> data structure from the specified XML stream.
        /// </summary>
        /// <typeparam name="T">The type of the root object you are loading.</typeparam>
        /// <param name="xmlStream">The XML stream.</param>
        /// <returns>The root object of the loaded data structure.</returns>
        T Load<T>(Stream xmlStream) where T : XorObject;

        /// <summary>
        /// Loads an <see cref="XorObject"/> data structure from the specified <see cref="XDocument"/>.
        /// </summary>
        /// <typeparam name="T">The type of the root object your are loading.</typeparam>
        /// <param name="xDocument">The <see cref="XDocument"/>.</param>
        /// <returns>The root object of the loaded data structure.</returns>
        T Load<T>(XDocument xDocument) where T : XorObject;

        #endregion

        #region Save

        /// <summary>
        /// Saves the data structure contained in the root <see cref="XorObject"/> to the specified file path.
        /// </summary>
        /// <param name="rootObject">The root object.</param>
        /// <param name="path">The file path.</param>
        void Save(XorObject rootObject, string path);

        /// <summary>
        /// Saves the data structure contained in the root <see cref="XorObject"/> to the specified XML stream.
        /// </summary>
        /// <param name="rootObject">The root object.</param>
        /// <param name="xmlStream">The XML stream.</param>
        void Save(XorObject rootObject, Stream xmlStream);

        #endregion

        #region Validate

        /// <summary>
        /// Validates the specified XML stream with the XorPersist XML schema.
        /// </summary>
        /// <param name="xmlStream">The XML stream.</param>
        void Validate(Stream xmlStream);

        /// <summary>
        /// Validates the specified XDocument with the XorPersist XML schema.
        /// </summary>
        /// <param name="xDocument">The XDocument.</param>
        void Validate(XDocument xDocument);

        /// <summary>
        /// Validates the file with the specified file path with the XorPersist XML schema.
        /// </summary>
        /// <param name="path">The path.</param>
        void Validate(string path);

        #endregion
    }
}