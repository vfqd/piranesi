using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Extension methods for Colliders.
    /// </summary>
    public static class ColliderExtensions
    {
        public static Vector3 GetCenter(this Collider collider)
        {
            Type type = collider.GetType();

            if (type == typeof(BoxCollider)) return ((BoxCollider)collider).GetCenter();
            if (type == typeof(SphereCollider)) return ((SphereCollider)collider).GetCenter();
            if (type == typeof(CapsuleCollider)) return ((CapsuleCollider)collider).GetCenter();
            if (type == typeof(MeshCollider)) return ((MeshCollider)collider).GetCenter();

            throw new NotImplementedException();
        }

        public static Vector3 GetCenter(this CapsuleCollider collider)
        {
            return collider.transform.TransformPoint(collider.center);
        }

        public static Vector3 GetCenter(this SphereCollider collider)
        {
            return collider.transform.TransformPoint(collider.center);
        }

        public static Vector3 GetCenter(this BoxCollider collider)
        {
            return collider.transform.TransformPoint(collider.center);
        }

        public static Vector3 GetCenter(this MeshCollider collider)
        {
            return collider.bounds.center;
        }

        public static bool ContainsPoint(this Collider collider, Vector3 point)
        {
            Type type = collider.GetType();

            if (type == typeof(BoxCollider)) return ((BoxCollider)collider).ContainsPoint(point);
            if (type == typeof(SphereCollider)) return ((SphereCollider)collider).ContainsPoint(point);
            if (type == typeof(CapsuleCollider)) return ((CapsuleCollider)collider).ContainsPoint(point);
            if (type == typeof(MeshCollider)) return ((MeshCollider)collider).ContainsPoint(point);

            throw new NotImplementedException();
        }

        public static bool ContainsPoint(this CapsuleCollider collider, Vector3 point)
        {
            return new Capsule3(collider).ContainsPoint(point);
        }

        public static bool ContainsPoint(this SphereCollider collider, Vector3 point)
        {
            return new Sphere3(collider).ContainsPoint(point);
        }


        public static bool ContainsPoint(this BoxCollider collider, Vector3 point)
        {
            return new Box3(collider).ContainsPoint(point);
        }


        public static bool ContainsPoint(this MeshCollider collider, Vector3 point)
        {
            if (collider.bounds.Contains(point))
            {
                Collider[] colliders = Physics.OverlapSphere(point, 1 << collider.gameObject.layer);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i] == collider)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsColliding(this Collider collider, Collider otherCollider)
        {
            float distance;
            Vector3 direction;
            return Physics.ComputePenetration(collider, collider.transform.position, collider.transform.rotation, otherCollider, otherCollider.transform.position, otherCollider.transform.rotation, out direction, out distance);
        }

        public static void IgnoreCollisions(this IList<Collider> colliders, IList<Collider> otherColliders, bool ignore = true)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                for (int j = 0; j < otherColliders.Count; j++)
                {
                    Physics.IgnoreCollision(colliders[i], otherColliders[j], ignore);
                }
            }
        }

        public static void IgnoreCollisions(this IList<Collider> colliders, Collider otherCollider, bool ignore = true)
        {
            for (int j = 0; j < colliders.Count; j++)
            {
                Physics.IgnoreCollision(colliders[j], otherCollider, ignore);
            }
        }

        public static void IgnoreCollisions(this Collider collider, IList<Collider> otherColliders, bool ignore = true)
        {
            for (int j = 0; j < otherColliders.Count; j++)
            {
                Physics.IgnoreCollision(collider, otherColliders[j], ignore);
            }
        }

    }

}
