using UnityEngine;

namespace Framework
{
    public static class FontStyles
    {
        public static GUIStyle BoldCentered { get { Init(); return _boldCentered; } }
        public static GUIStyle Centered { get { Init(); return _centered; } }
        public static GUIStyle Bold { get { Init(); return _bold; } }
        public static GUIStyle LeftAligned { get { Init(); return _leftAligned; } }


        private static GUIStyle _boldCentered;
        private static GUIStyle _centered;
        private static GUIStyle _bold;
        private static GUIStyle _leftAligned;

        private static bool _initialized;


        static void Init()
        {
            if (!_initialized)
            {
                _initialized = true;

                _leftAligned = new GUIStyle(GUI.skin.GetStyle("Label"));

                _boldCentered = new GUIStyle(GUI.skin.GetStyle("Label"));
                _boldCentered.alignment = TextAnchor.MiddleCenter;
                _boldCentered.fontStyle = FontStyle.Bold;

                _centered = new GUIStyle(GUI.skin.GetStyle("Label"));
                _centered.alignment = TextAnchor.MiddleCenter;

                _bold = new GUIStyle(GUI.skin.GetStyle("Label"));
                _bold.fontStyle = FontStyle.Bold;
            }
        }

        public static GUIStyle WithFontSize(this GUIStyle stlye, int fontSize)
        {
            GUIStyle s = new GUIStyle(stlye);
            s.fontSize = fontSize;
            return s;
        }

    }
}
