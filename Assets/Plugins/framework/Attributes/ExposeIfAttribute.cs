using System.Reflection;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Causes the field to only be exposed in the editor if some other field or property condition is true.
    /// </summary>
    public class ExposeIfAttribute : PropertyAttribute
    {
        private string _fieldName;
        private object _value;


        /// <summary>
        /// Causes the field to only be exposed in the editor if some other field or property condition is true. 
        /// </summary>
        /// <param name="fieldName">The field name to check</param>
        /// <param name="value">The value the field must be equal to</param>
        public ExposeIfAttribute(string fieldName, object value)
        {
            _fieldName = fieldName;
            _value = value;

        }
        /// <summary>
        /// Causes the field to only be exposed in the editor if some other field or property is true.
        /// </summary>
        /// <param name="fieldName">The field name to check</param>
        public ExposeIfAttribute(string fieldName)
        {
            _fieldName = fieldName;
            _value = true;
        }

        public bool EvaluateCondition(object obj)
        {

            FieldInfo field = obj.GetType().GetField(_fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                return field.GetValue(obj).Equals(_value);
            }


            PropertyInfo prop = obj.GetType().GetProperty(_fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop != null) return prop.GetValue(obj, null).Equals(_value);

            return true;

        }


    }
}
