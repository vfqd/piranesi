using UnityEngine;

namespace Framework
{
    public class NotEditableAttribute : PropertyAttribute
    {
        public bool OnlyShowInPlayMode;

        public NotEditableAttribute(bool onlyShowInPlayMode = false)
        {
            OnlyShowInPlayMode = onlyShowInPlayMode;
        }
    }
}


