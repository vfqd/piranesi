using System;
using UnityEngine;

namespace Framework
{
    public class PrefabDropdownAttribute : PropertyAttribute
    {
        public Type Type => _type;
        public bool AllowNull => _allowNull;

        private Type _type;
        private bool _allowNull = true;

        public PrefabDropdownAttribute(Type type, bool allowNull = true)
        {
            _allowNull = allowNull;

            if (typeof(Component).IsAssignableFrom(type))
            {
                _type = type;
            }
            else
            {
                throw new ArgumentException("Type is not a component type: " + type);
            }
        }

        public PrefabDropdownAttribute(bool allowNull = true)
        {
            _allowNull = allowNull;
        }

    }
}
