using System;
using UnityEngine;

namespace Framework
{
    public class SubtypeAttribute : PropertyAttribute
    {
        public Type Type;

        public SubtypeAttribute(Type type)
        {
            Type = type;
        }
    }
}
