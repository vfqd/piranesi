using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class PhysicsUtils
    {

        private static RaycastHit[] _raycastResults;

        public static bool RaycastIgnoring(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance, int layerMask, Collider ignoreCollider)
        {
            if (_raycastResults == null) _raycastResults = new RaycastHit[100];

            hit = new RaycastHit();
            float minDistance = Mathf.Infinity;

            int count = Physics.RaycastNonAlloc(origin, direction, _raycastResults, maxDistance, layerMask);
            for (int i = 0; i < count; i++)
            {
                if (_raycastResults[i].collider != ignoreCollider && _raycastResults[i].distance < minDistance)
                {
                    hit = _raycastResults[i];
                    minDistance = _raycastResults[i].distance;
                }
            }

            return minDistance < Mathf.Infinity;
        }


        public static bool RaycastIgnoring(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance, int layerMask, IList<Collider> ignoreColliders)
        {
            if (_raycastResults == null) _raycastResults = new RaycastHit[100];

            hit = new RaycastHit();
            float minDistance = Mathf.Infinity;

            int count = Physics.RaycastNonAlloc(origin, direction, _raycastResults, maxDistance, layerMask);
            for (int i = 0; i < count; i++)
            {
                if (!ignoreColliders.Contains(_raycastResults[i].collider) && _raycastResults[i].distance < minDistance)
                {
                    hit = _raycastResults[i];
                    minDistance = _raycastResults[i].distance;
                }
            }

            return minDistance < Mathf.Infinity;
        }



        public static bool Raycast(Vector3 from, Vector3 to, LayerMask layerMask)
        {
            Vector3 line = to - from;
            return Physics.Raycast(new Ray(from, line.normalized), line.magnitude, layerMask);
        }

        public static RaycastHit[] RaycastAll(Vector3 from, Vector3 to, LayerMask layerMask)
        {
            Vector3 line = to - from;
            return Physics.RaycastAll(new Ray(from, line.normalized), line.magnitude, layerMask);
        }


        public static T GetNearestInSphere<T>(Vector3 point, float radius, LayerMask layermask) where T : Component
        {
            Collider[] colliders = Physics.OverlapSphere(point, radius, layermask);
            float minDist = Mathf.Infinity;
            T nearest = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                T item = colliders[i].GetComponentInParent<T>();

                if (item != null)
                {
                    float dist = (item.transform.position - point).sqrMagnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearest = item;
                    }
                }
            }

            return nearest;
        }

        public static T GetNearestInSphereWhen<T>(Vector3 point, float radius, LayerMask layermask, Predicate<T> filter) where T : Component
        {
            Collider[] colliders = Physics.OverlapSphere(point, radius, layermask);
            float minDist = Mathf.Infinity;
            T nearest = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                T item = colliders[i].GetComponentInParent<T>();

                if (item != null && filter(item))
                {
                    float dist = (item.transform.position - point).sqrMagnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearest = item;
                    }
                }
            }

            return nearest;
        }

        public static LayerMask GetCollisionMatrixMask(int layer)
        {
            int mask = 0;
            for (int i = 0; i < 32; i++)
            {
                if (!Physics.GetIgnoreLayerCollision(layer, i))
                {
                    mask |= 1 << i;
                }
            }

            return mask;
        }

        public static RaycastHit[] FanCast(Vector3 origin, Vector3 direction, Vector3 normal, float angle, float distance, int rays, LayerMask layerMask)
        {
            float interval = angle / rays;

            List<RaycastHit> hits = new List<RaycastHit>();
            Quaternion rotation = Quaternion.LookRotation(direction.normalized, normal) * Quaternion.Euler(0, -angle * 0.5f, 0);

            for (int i = 0; i <= rays; i++)
            {
                Ray ray = new Ray(origin, rotation * Quaternion.Euler(0, interval * i, 0) * Vector3.forward);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, distance, layerMask))
                {
                    hits.Add(hit);

                    //     DebugUtils.DrawLine(origin, origin + ray.direction, Color.red);
                }
                else
                {
                    //    DebugUtils.DrawLine(origin, origin + ray.direction);
                }


            }



            return hits.ToArray();
        }

    }
}
