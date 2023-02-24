using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Wpf.Core
{
    /// <summary>
    /// An object that can add property change notifications to any object it wraps.
    /// <para>
    /// How it works...
    /// </para>
    /// <para>
    /// This type reads the public properties of the wrapped object that have setters (need 
    /// to be able to set the value). Which will then be available as properties on this 
    /// dynamic object. These properties will have built in change notification that push the 
    /// value back to the wrapped object.
    /// </para>
    /// <para>
    /// That being said, kind of important that the developer know the properties they want 
    /// to access because there is no automatic completion for dynamic objects.
    /// </para>
    /// <para>
    /// New properties, methods or events cannot be added to this type. The collection of 
    /// properties is enforced and this object is immutable. The values are mutable, they need 
    /// to be in order to automatically update the UI.
    /// </para>
    /// </summary>
    public class PropertyChangeNotifier : DynamicObject, INotifyPropertyChanged
    {
        #region Fields

        private List<PropertyInfo> properties;
        private object wrapped;

        #endregion

        #region Events

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="PropertyChangeNotifier"/> class.</summary>
        /// <param name="objectToWrap">The object to wrap with property changes.</param>
        /// <exception cref="ArgumentNullException">objectToWrap is null.</exception>
        public PropertyChangeNotifier(object objectToWrap)
        {
            if (objectToWrap == null) throw new ArgumentNullException(nameof(objectToWrap));

            wrapped = objectToWrap;

            // get properties, remove properties that have no setter
            properties = objectToWrap.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite).ToList();
        }

        /// <summary>Initializes a new instance of the <see cref="PropertyChangeNotifier"/> class.</summary>
        /// <param name="objectToWrap">The object to wrap with property changes.</param>
        /// <exception cref="ArgumentNullException">objectToWrap is null.</exception>
        public PropertyChangeNotifier(object objectToWrap, BindingFlags propertyFlags)
        {
            if (objectToWrap == null) throw new ArgumentNullException(nameof(objectToWrap));

            wrapped = objectToWrap;

            // get properties, remove properties that have no setter
            properties = objectToWrap.GetType().GetProperties(propertyFlags).Where(p => p.CanWrite).ToList();
        }

        #endregion

        #region Methods

        /// <summary>Attempts to get a property.</summary>
        /// <param name="binder">The binder with the info for property retrieval.</param>
        /// <param name="result">The result of the retrieval.</param>
        /// <returns>True if successfully retrieved otherwise false.</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            PropertyInfo property = properties.FirstOrDefault(p => p.Name == binder.Name);

            if (property != null)
            {
                result = property.GetValue(wrapped);

                return true;
            }
            else
            {
                result = null;

                return false;
            }
        }

        /// <summary>Attempts to set the value of a property.</summary>
        /// <param name="binder">The binder with the info for property retrieval.</param>
        /// <param name="value">The value to set for the property.</param>
        /// <returns>True if the value was set otherwise false.</returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // we are not adding properties this way, our properties will be generated from the constructor

            PropertyInfo property = properties.FirstOrDefault(p => p.Name == binder.Name);

            if (property == null)
            {
                return false;
            }

            // we will not put nulls in the dictionary
            property.SetValue(wrapped, value);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(binder.Name));

            return true;
        }

        #endregion
    }
}
