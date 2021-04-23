using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Extension methods for matrices
    /// </summary>
    public static class MatrixExtensions
    {

        /// <summary>
        /// Gets the rotation component from a TRS matrix.
        /// </summary>
        /// <returns>The matrix's rotation</returns>
        public static Quaternion GetRotation(this Matrix4x4 m)
        {
            return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
        }

        /// <summary>
        /// Gets the position component from a TRS matrix.
        /// </summary>
        /// <returns>The matrix's position</returns>
        public static Vector3 GetTranslation(this Matrix4x4 m)
        {
            Vector4 vector4Position = m.GetColumn(3);
            return new Vector3(vector4Position.x, vector4Position.y, vector4Position.z);
        }


        /// <summary>
        /// Gets the scale component from a TRS matrix.
        /// </summary>
        /// <returns>The matrix's scale</returns>
        public static Vector3 GetScale(this Matrix4x4 m)
        {
            Vector3 scale;
            scale.x = new Vector4(m.m00, m.m10, m.m20, m.m30).magnitude;
            scale.y = new Vector4(m.m01, m.m11, m.m21, m.m31).magnitude;
            scale.z = new Vector4(m.m02, m.m12, m.m22, m.m32).magnitude;
            return scale;
        }

        public static Matrix4x4 WithTranslation(this Matrix4x4 TRSMatrix, Vector3 translation)
        {
            return Matrix4x4.TRS(translation, TRSMatrix.GetRotation(), TRSMatrix.GetScale());
        }

        public static Matrix4x4 WithRotation(this Matrix4x4 TRSMatrix, Quaternion rotation)
        {
            return Matrix4x4.TRS(TRSMatrix.GetTranslation(), rotation, TRSMatrix.GetScale());
        }

        public static Matrix4x4 WithScale(this Matrix4x4 TRSMatrix, Vector3 scale)
        {
            return Matrix4x4.TRS(TRSMatrix.GetTranslation(), TRSMatrix.GetRotation(), scale);
        }

#if UNITY_EDITOR

        public static Matrix4x4 GetMatrix4x4Value(this UnityEditor.SerializedProperty property)
        {
            Matrix4x4 matrix = new Matrix4x4();

            matrix.m00 = property.FindPropertyRelative("e00").floatValue;
            matrix.m01 = property.FindPropertyRelative("e01").floatValue;
            matrix.m02 = property.FindPropertyRelative("e02").floatValue;
            matrix.m03 = property.FindPropertyRelative("e03").floatValue;

            matrix.m10 = property.FindPropertyRelative("e10").floatValue;
            matrix.m11 = property.FindPropertyRelative("e11").floatValue;
            matrix.m12 = property.FindPropertyRelative("e12").floatValue;
            matrix.m13 = property.FindPropertyRelative("e13").floatValue;

            matrix.m20 = property.FindPropertyRelative("e20").floatValue;
            matrix.m21 = property.FindPropertyRelative("e21").floatValue;
            matrix.m22 = property.FindPropertyRelative("e22").floatValue;
            matrix.m23 = property.FindPropertyRelative("e23").floatValue;

            matrix.m30 = property.FindPropertyRelative("e30").floatValue;
            matrix.m31 = property.FindPropertyRelative("e31").floatValue;
            matrix.m32 = property.FindPropertyRelative("e32").floatValue;
            matrix.m33 = property.FindPropertyRelative("e33").floatValue;

            return matrix;
        }

        public static void SetMatrix4x4Value(this UnityEditor.SerializedProperty property, Matrix4x4 matrix)
        {
            property.FindPropertyRelative("e00").floatValue = matrix.m00;
            property.FindPropertyRelative("e01").floatValue = matrix.m01;
            property.FindPropertyRelative("e02").floatValue = matrix.m02;
            property.FindPropertyRelative("e03").floatValue = matrix.m03;

            property.FindPropertyRelative("e10").floatValue = matrix.m10;
            property.FindPropertyRelative("e11").floatValue = matrix.m11;
            property.FindPropertyRelative("e12").floatValue = matrix.m12;
            property.FindPropertyRelative("e13").floatValue = matrix.m13;

            property.FindPropertyRelative("e20").floatValue = matrix.m20;
            property.FindPropertyRelative("e21").floatValue = matrix.m21;
            property.FindPropertyRelative("e22").floatValue = matrix.m22;
            property.FindPropertyRelative("e23").floatValue = matrix.m23;

            property.FindPropertyRelative("e30").floatValue = matrix.m30;
            property.FindPropertyRelative("e31").floatValue = matrix.m31;
            property.FindPropertyRelative("e32").floatValue = matrix.m32;
            property.FindPropertyRelative("e33").floatValue = matrix.m33;
        }

#endif

    }
}
