using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Framework
{
    /// <summary>
    /// Extension methods for miscellaneous types.
    /// </summary>
    public static class MiscExtensions
    {
        private static readonly Type _objectType = typeof(object);
        private static readonly Type _unityObjectType = typeof(Object);

        public static FieldInfo GetFieldIncludingParentTypes(this Type type, string fieldName, BindingFlags bindingFlags)
        {
            Type objectType = typeof(object);

            if (type != objectType)
            {
                do
                {
                    FieldInfo field = type.GetField(fieldName, bindingFlags | BindingFlags.DeclaredOnly);
                    if (field != null)
                    {
                        return field;
                    }

                    type = type.BaseType;
                } while (type != objectType);
            }

            return null;
        }

        public static FieldInfo[] GetFieldsIncludingParentTypes(this Type type, BindingFlags bindingFlags, bool dontIncludeUnityObjectFields = true)
        {
            List<FieldInfo> fields = new List<FieldInfo>();


            if (type != _objectType && (!dontIncludeUnityObjectFields || type != _unityObjectType))
            {
                do
                {
                    fields.AddRange(type.GetFields(bindingFlags | BindingFlags.DeclaredOnly));
                    type = type.BaseType;
                } while (type != _objectType && (!dontIncludeUnityObjectFields || type != _unityObjectType));
            }

            return fields.ToArray();
        }

        public static PropertyInfo GetPropertyIncludingParentTypes(this Type type, string fieldName, BindingFlags bindingFlags)
        {
            Type objectType = typeof(object);

            if (type != objectType)
            {
                do
                {
                    PropertyInfo property = type.GetProperty(fieldName, bindingFlags | BindingFlags.DeclaredOnly);
                    if (property != null)
                    {
                        return property;
                    }

                    type = type.BaseType;
                } while (type != objectType);
            }

            return null;
        }

        public static PropertyInfo[] GetPropertiesIncludingParentTypes(this Type type, BindingFlags bindingFlags, bool dontIncludeUnityObjectFields = true)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();

            if (type != _objectType && (!dontIncludeUnityObjectFields || type != _unityObjectType))
            {
                do
                {
                    properties.AddRange(type.GetProperties(bindingFlags | BindingFlags.DeclaredOnly));
                    type = type.BaseType;
                } while (type != _objectType && (!dontIncludeUnityObjectFields || type != _unityObjectType));
            }

            return properties.ToArray();
        }

        public static bool IsCurrentStateName(this Animator animator, string stateName, int layer = 0)
        {
            return animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == Animator.StringToHash(stateName);
        }

        public static bool IsNextStateName(this Animator animator, string stateName, int layer = 0)
        {
            return animator.GetNextAnimatorStateInfo(layer).shortNameHash == Animator.StringToHash(stateName);
        }

        public static AnimationClip GetClip(this Animator animator, string clipName)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name == clipName)
                {
                    return clips[i];
                }
            }

            return null;
        }

        public static Dictionary<string, AnimationClip> GetClipDictionary(this Animator animator)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            Dictionary<string, AnimationClip> dict = new Dictionary<string, AnimationClip>();

            for (int i = 0; i < clips.Length; i++)
            {
                dict.Add(clips[i].name, clips[i]);
            }

            return dict;
        }

        public static T GetAttribute<T>(this FieldInfo field, bool inherit = false) where T : Attribute
        {
            object[] attributes = field.GetCustomAttributes(typeof(T), inherit);
            if (attributes.Length > 0)
            {
                return (T)attributes[0];
            }

            return null;
        }


        public static float GetVolume(this Bounds bounds)
        {
            return bounds.size.x * bounds.size.y * bounds.size.z;
        }

        public static Frustum GetFrustum(this Camera camera)
        {
            return new Frustum(camera);
        }

        public static Ray NormalizedScreenPointToRay(this Camera camera, Vector2 normalizedScreenPoint)
        {
            return camera.ScreenPointToRay(new Vector3(normalizedScreenPoint.x * camera.pixelWidth, normalizedScreenPoint.y * camera.pixelHeight, 0));
        }

        public static Ray GetForwardRay(this Camera camera)
        {
            return camera.ScreenPointToRay(new Vector3(camera.pixelWidth * 0.5f, camera.pixelHeight * 0.5f, 0));
        }
        public static Ray GetMouseRay(this Camera camera)
        {
            return camera.ScreenPointToRay(Mouse.Position);
        }

        public static bool Raycast(this Plane plane, Ray ray, out Vector3 intersectionPoint)
        {
            float dist;
            intersectionPoint = Vector3.zero;

            if (plane.Raycast(ray, out dist))
            {
                intersectionPoint = ray.GetPoint(dist);
                return true;
            }
            return false;
        }



        public static bool AreBytesTheSame(this byte[] bytes, byte[] otherBytes)
        {
            if (bytes == otherBytes) return true;
            if (bytes.Length != otherBytes.Length) return false;

            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] != otherBytes[i]) return false;
            }

            return true;
        }

        public static TemporaryRandomState UseTemporarily(this Random.State randomState)
        {
            return new TemporaryRandomState(randomState);
        }

        public static string ToStringUnformatted(this Guid guid)
        {
            return guid.ToString().ToLower().Replace("-", string.Empty);
        }

        public static void Emit(this ParticleSystem particleSystem, Vector3 position, Quaternion rotation, int count = 1)
        {
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

            emitParams.position = position;
            emitParams.rotation3D = rotation.eulerAngles;
            emitParams.applyShapeToPosition = true;

            particleSystem.Emit(emitParams, count);

        }

        public static void EnableEmission(this ParticleSystem particleSystem)
        {
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.enabled = true;
        }

        public static void DisableEmission(this ParticleSystem particleSystem)
        {
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.enabled = false;
        }


        public static void SetConstraint(this Rigidbody rigidbody, RigidbodyConstraints constraint, bool enabled)
        {
            if (enabled)
            {
                rigidbody.constraints |= constraint;
            }
            else
            {
                rigidbody.constraints &= ~constraint;
            }
        }

        public static bool GetConstraint(this Rigidbody rigidbody, RigidbodyConstraints constraint)
        {
            return ((1 << (int)constraint) & (int)rigidbody.constraints) > 0;
        }


        public static void AddAlignmentTorque(this Rigidbody rigidbody, Vector3 targetDirection, float alignmentTorque, ForceMode forceMode)
        {
            Vector3 rotationAxis = Vector3.Cross(rigidbody.transform.forward, targetDirection.normalized);
            Vector3 angularVelocityChange = rotationAxis.normalized * Mathf.Asin(rotationAxis.magnitude);

            Quaternion worldSpaceTensor = rigidbody.transform.rotation * rigidbody.inertiaTensorRotation;
            Vector3 alignment = worldSpaceTensor * Vector3.Scale(rigidbody.inertiaTensor, Quaternion.Inverse(worldSpaceTensor) * angularVelocityChange);

            if (!alignment.HasNaNComponent())
            {
                rigidbody.AddTorque((alignment * alignmentTorque), forceMode);
            }
        }

        public static void AddAlignmentTorque(this Rigidbody rigidbody, Vector3 targetDirection, float alignmentTorque, float uprightingTorque, ForceMode forceMode)
        {
            Vector3 rotationAxis = Vector3.Cross(rigidbody.transform.forward, targetDirection.normalized);
            Vector3 angularVelocityChange = rotationAxis.normalized * Mathf.Asin(rotationAxis.magnitude);

            Quaternion worldSpaceTensor = rigidbody.transform.rotation * rigidbody.inertiaTensorRotation;
            Vector3 alignment = worldSpaceTensor * Vector3.Scale(rigidbody.inertiaTensor, Quaternion.Inverse(worldSpaceTensor) * angularVelocityChange);

            float uprightingAngle = rigidbody.transform.rotation.eulerAngles.z;
            uprightingAngle = uprightingAngle > 180 ? uprightingAngle - 360f : uprightingAngle;
            Vector3 uprighting = -rigidbody.transform.forward * (uprightingAngle / 90f);

            if (alignment.HasNaNComponent()) alignment = Vector3.zero;
            if (uprighting.HasNaNComponent()) uprighting = Vector3.zero;

            rigidbody.AddTorque((uprighting * uprightingTorque) + (alignment * alignmentTorque), forceMode);
        }



        public static bool LerpTowards(this Rigidbody rigidbody, Vector3 target, float speed, float maxDistance = Mathf.Infinity)
        {
            Vector3 toTarget = target - rigidbody.worldCenterOfMass;
            rigidbody.velocity = Vector3.MoveTowards(rigidbody.velocity, toTarget * speed, maxDistance);

            return rigidbody.velocity.sqrMagnitude >= toTarget.sqrMagnitude;
        }

        public static bool LerpTowards(this Rigidbody rigidbody, Vector3 target, Vector3 worldPosition, float speed, float maxDistance = Mathf.Infinity)
        {
            Vector3 toTarget = target - worldPosition;
            rigidbody.velocity = Vector3.MoveTowards(rigidbody.velocity, toTarget * speed, maxDistance);

            return rigidbody.velocity.sqrMagnitude >= toTarget.sqrMagnitude;
        }

        public static Vector2 GetDimensions(this TextMesh textMesh)
        {
            float currentLineWidth = 0;
            float currentLineheight = 0;
            float longestLineWidth = 0;
            float totalHeight = 0;
            int numLines = 1;
            CharacterInfo info;

            for (int i = 0; i < textMesh.text.Length; i++)
            {
                char c = textMesh.text[i];

                if (c == '\n')
                {
                    numLines++;
                    totalHeight += currentLineheight;
                    currentLineheight = 0;
                    currentLineWidth = 0;
                    continue;
                }

                if (textMesh.font.GetCharacterInfo(c, out info, textMesh.fontSize, textMesh.fontStyle))
                {
                    currentLineWidth += info.advance;
                    if (currentLineWidth > longestLineWidth)
                    {
                        longestLineWidth = currentLineWidth;
                    }
                    if (info.glyphHeight > currentLineheight)
                    {
                        currentLineheight = info.glyphHeight;
                    }
                }
            }
            totalHeight += currentLineheight;

            if (textMesh.lineSpacing < 1f)
            {
                return new Vector2(longestLineWidth, textMesh.font.lineHeight * ((numLines * textMesh.lineSpacing) + (1 - textMesh.lineSpacing))) * textMesh.characterSize * 0.1f;
            }

            //        return new Vector2(longestLineWidth, numLines * textMesh.font.lineHeight * textMesh.lineSpacing) * textMesh.characterSize * 0.1f;

            return new Vector2(longestLineWidth, totalHeight * textMesh.lineSpacing * 1.5f) * textMesh.characterSize * 0.1f;
        }

        public static Transform[] GetChildren(this Transform transform)
        {
            Transform[] children = new Transform[transform.childCount];
            for (int i = 0; i < children.Length; i++)
            {
                children[i] = transform.GetChild(i);
            }

            return children;
        }


        public static T GetComponentInParent<T>(this MonoBehaviour component, bool includeInactive) where T : Component
        {

            Transform current = component.transform;
            while (current != null)
            {
                Component[] components = current.GetComponents<Component>();

                if (current.gameObject.activeInHierarchy || includeInactive)
                {
                    for (int i = 0; i < components.Length; i++)
                    {
                        if (components[i] is T) return components[i] as T;
                    }
                }

                current = current.parent;
            }

            return null;
        }



        public static Vector2 GetNormalizedPosition(this Rect rect, Vector2 position)
        {
            return new Vector2((position.x - rect.xMin) / rect.width, (position.y - rect.yMin) / rect.height);
        }

        public static bool ContainsCaseInsensitive(this string s, string substring)
        {
            return s.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Checks whether this object extends or implements a type.
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        /// <returns>True if the type is asignable from this object's type</returns>
        public static bool IsTypeOf<T>(this object obj)
        {
            return typeof(T).IsAssignableFrom(obj.GetType());
        }

        /// <summary>
        /// Returns whether or not the object is between two comparable bounds. Inclusive on the lower bound, exclusive on the upper one.
        /// </summary>
        /// <param name="lower">The lower bound</param>
        /// <param name="upper">The upper bound</param>
        /// <returns>Whether or not the object is between the bounds</returns>
        public static bool IsBetween<T>(this T actual, T lower, T upper) where T : IComparable<T>
        {
            return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) < 0;
        }



        public static string GetPath(this GameObject gameObject)
        {
            if (gameObject == null)
            {
                return "NULL";
            }

#if UNITY_EDITOR
            if (UnityEditor.AssetDatabase.Contains(gameObject))
            {
                return UnityEditor.AssetDatabase.GetAssetPath(gameObject);
            }
#endif

            Transform transform = gameObject.transform;
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }

            return gameObject.scene.name + "/" + path;
        }



        public static Coroutine InvokeRepeating(this MonoBehaviour mono, Action action, float delay, bool firstTimeIsRandom = false, bool useUnscaledTime = false)
        {
            return mono.StartCoroutine(InvokeRepeatingRoutine(action, delay, firstTimeIsRandom, useUnscaledTime));
        }

        public static Coroutine InvokeRandomly(this MonoBehaviour mono, Action action, float minDelay, float maxDelay, bool useUnscaledTime = false)
        {
            return mono.StartCoroutine(InvokeRandomlyRoutine(action, minDelay, maxDelay, useUnscaledTime));
        }

        public static Coroutine InvokeRandomly(this MonoBehaviour mono, Action action, FloatRange delayRange, bool useUnscaledTime = false)
        {
            return mono.StartCoroutine(InvokeRandomlyRoutine(action, delayRange.Min, delayRange.Max, useUnscaledTime));
        }

        public static Coroutine InvokeDelayed(this MonoBehaviour mono, float delay, Action action)
        {
            return mono.StartCoroutine(InvokeDelayedRoutine(action, delay, false));
        }

        public static Coroutine InvokeDelayedUnscaled(this MonoBehaviour mono, float delay, Action action)
        {
            return mono.StartCoroutine(InvokeDelayedRoutine(action, delay, true));
        }

        static IEnumerator InvokeRepeatingRoutine(Action action, float delay, bool firstTimeIsRandom, bool useUnscaledTime)
        {

            if (useUnscaledTime)
            {
                yield return new WaitForSecondsRealtime(firstTimeIsRandom ? delay * Random.value : delay);
            }
            else
            {
                yield return new WaitForSeconds(firstTimeIsRandom ? delay * Random.value : delay);
            }


            while (true)
            {
                action();

                if (useUnscaledTime)
                {
                    yield return new WaitForSecondsRealtime(delay);
                }
                else
                {
                    yield return new WaitForSeconds(delay);
                }
            }
        }

        static IEnumerator InvokeRandomlyRoutine(Action action, float minDelay, float maxDelay, bool useUnscaledTime)
        {
            while (true)
            {
                if (useUnscaledTime)
                {
                    yield return new WaitForSecondsRealtime(Random.Range(minDelay, maxDelay));
                }
                else
                {
                    yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
                }

                action();
            }
        }

        /// <summary>
        /// Will execute imediately if delay is 0
        /// </summary>
        static IEnumerator InvokeDelayedRoutine(Action action, float delay, bool useUnscaledTime)
        {
            if (delay == 0)
            {
                action();
            }
            else
            {

                if (useUnscaledTime)
                {
                    yield return new WaitForSecondsRealtime(delay);
                }
                else
                {
                    yield return new WaitForSeconds(delay);
                }

                action();
            }
        }

        public static Coroutine InvokeFor(this MonoBehaviour behaviour, float duration, Action action)
        {
            return behaviour.StartCoroutine(InvokeForRoutine(duration, action));
        }

        static IEnumerator InvokeForRoutine(float duration, Action action)
        {
            Timer timer = new Timer(duration);

            while (!timer.HasFinished)
            {
                action();
                yield return null;
            }
        }

        public static Coroutine InvokeFor(this MonoBehaviour behaviour, float duration, Action<float> action)
        {
            return behaviour.StartCoroutine(InvokeForRoutine(duration, action));
        }

        static IEnumerator InvokeForRoutine(float duration, Action<float> action)
        {
            Timer timer = new Timer(duration);

            while (!timer.HasFinished)
            {
                action(timer.ProportionElapsed);
                yield return null;
            }
        }

        public static void SetMaterial(this Renderer renderer, Material material, int index)
        {
            Material[] materials = new Material[renderer.materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = i == index ? material : renderer.materials[i];
            }

            renderer.materials = materials;
        }

        public static void SetSharedMaterial(this Renderer renderer, Material material, int index)
        {
            Material[] materials = new Material[renderer.sharedMaterials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = i == index ? material : renderer.sharedMaterials[i];
            }

            renderer.sharedMaterials = materials;
        }


        /// <summary>
        /// Returns the dimensions of the camera in world units.
        /// </summary>
        /// <returns>Worldspace camera dimensions</returns>
        public static Vector2 WorldspaceDimensions(this Camera camera)
        {
            float height = 2 * camera.orthographicSize;
            return new Vector2(height * camera.aspect, height);
        }

        public static Rect GetScreenspaceRect(this Camera camera, Vector3 cornerA, Vector3 cornerB)
        {
            cornerA = camera.WorldToScreenPoint(cornerA);
            cornerB = camera.WorldToScreenPoint(cornerB);

            return new Rect(Mathf.Min(cornerA.x, cornerB.x), Screen.height - Mathf.Max(cornerA.y, cornerB.y), Mathf.Abs(cornerA.x - cornerB.x), Mathf.Abs(cornerA.y - cornerB.y));
        }

        /// <summary>
        /// Returns the value of the float squared.
        /// </summary>
        /// <returns>Value * value</returns>
        public static float Squared(this float f)
        {
            return f * f;
        }

        /// <summary>
        /// Checks whether this Type is a descendant of some other type.
        /// </summary>
        /// <param name="parentType">The base Type to check</param>
        /// <returns>True if this Type is assiagnable from the parentType</returns>
        public static bool IsTypeOf(this Type childType, Type parentType)
        {
            return parentType.IsAssignableFrom(childType);
        }

        /// <summary>
        /// Checks whether this Type is a descendant of some other type.
        /// </summary>
        /// <param name="T">The base Type to check</param>
        /// <returns>True if this Type is assiagnable from the parentType</returns>
        public static bool IsTypeOf<T>(this Type childType)
        {
            return typeof(T).IsAssignableFrom(childType);
        }

        public static Type[] GetAllSubtypesInUnityAssemblies(this Type type)
        {
            List<Type> subtypes = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                if (!assemblies[i].FullName.StartsWith("Assembly-CSharp")) continue;

                Type[] types = assemblies[i].GetTypes();

                for (int j = 0; j < types.Length; j++)
                {
                    if (type.IsAssignableFrom(types[j]) && types[j] != type)
                    {
                        subtypes.Add(types[j]);
                    }
                }
            }


            return subtypes.ToArray();
        }

        public static Type[] GetAllSubtypesInCurrentAssembly(this Type type)
        {
            List<Type> subtypes = new List<Type>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();

            for (int i = 0; i < types.Length; i++)
            {
                if (type.IsAssignableFrom(types[i]) && types[i] != type)
                {
                    subtypes.Add(types[i]);
                }
            }

            return subtypes.ToArray();
        }
        public static GUIContent WithTooltip(this GUIContent content, string tooltip)
        {
            content.tooltip = tooltip;
            return content;
        }

        /// <summary>
        /// Destroy the object immediately or delayed based on current editor playing state.
        /// </summary>
        public static void SmartDestroy(this UnityEngine.Object target)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                UnityEngine.Object.Destroy(target);
            else
                UnityEngine.Object.DestroyImmediate(target);
#else
        UnityEngine.Object.Destroy (target);
#endif
        }


#if UNITY_EDITOR
        public static void CenterInScreen(this UnityEditor.EditorWindow window)
        {
            Vector2Int resolution = ResolutionOption.GetLargestSupportedResolution().Dimensions;
            window.position = new Rect((resolution.x - window.position.width) * 0.5f, (resolution.y - window.position.height) * 0.5f, window.position.width, window.position.height);
        }

        public static void CenterInScreen(this UnityEditor.EditorWindow window, float width, float height)
        {
            Vector2Int resolution = ResolutionOption.GetLargestSupportedResolution().Dimensions;
            window.position = new Rect((resolution.x - width) * 0.5f, (resolution.y - height) * 0.5f, width, height);
        }



#endif

    }
}
