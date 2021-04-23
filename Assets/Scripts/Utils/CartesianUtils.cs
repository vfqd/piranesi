using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class CartesianUtils
    {
        public static Vector2Int[] GetFilledCircle(int radius, Vector2Int origin)
        {
            List<Vector2Int> res = new List<Vector2Int>();
            for (int y = -radius; y <= radius; ++y)
            {
                for (int x = -radius; x <= radius; ++x)
                {
                    if(x*x+y*y < radius*radius + radius)
                    {
                        res.Add(new Vector2Int(origin.x+x, origin.y+y));
                    }
                }
            }

            return res.ToArray();
        }
        
        public static Vector2Int[] GetEmptyCircle(int radius,Vector2Int origin)
        {
            int d = (5 - radius * 4) / 4;
            int x = 0;
            int y = radius;
 
            List<Vector2Int> res = new List<Vector2Int>();
            do
            {
                res.Add(new Vector2Int(origin.x + x, origin.y + y));
                res.Add(new Vector2Int(origin.x + x, origin.y - y));
                res.Add(new Vector2Int(origin.x - x, origin.y + y));
                res.Add(new Vector2Int(origin.x - x, origin.y - y));
                res.Add(new Vector2Int(origin.x + y, origin.y + x));
                res.Add(new Vector2Int(origin.x + y, origin.y - x));
                res.Add(new Vector2Int(origin.x - y, origin.y + x));
                res.Add(new Vector2Int(origin.x - y, origin.y - x));
                if (d < 0)
                {
                    d += 2 * x + 1;
                }
                else
                {
                    d += 2 * (x - y) + 1;
                    y--;
                }
                x++;
            } while (x <= y);

            return res.ToArray();
        }
        
        public static Vector2Int[] GetFilledImperfectCircle(int radius, Vector2Int origin, float noiseFunction, int delta)
        {
            int startingRad = radius;
            delta = Mathf.Clamp(delta, 1, 2);
            List<Vector2Int> res = new List<Vector2Int>();
            for (int y = -radius; y <= radius; ++y)
            {
                for (int x = -radius; x <= radius; ++x)
                {

                    if (Random.value > noiseFunction)
                    {
                        radius+=delta;
                    }
                    else
                    {
                        radius-=delta;
                    }

                    radius = Mathf.Clamp(radius, startingRad/2, startingRad);
                
                    if(x*x+y*y < radius*radius + radius)
                    {
                        res.Add(new Vector2Int(origin.x+x, origin.y+y));
                    }
                }
            }

            return res.ToArray();
        }
        
        public static Vector2Int GetRandomPointOnCircle(int radius, Vector2Int origin)
        {
            float angle = Random.value*Mathf.PI*2;
            float x = Mathf.Cos(angle)*radius;
            float y = Mathf.Sin(angle)*radius;
            return origin + new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
        }
        
        public static Vector2Int[] GetRect(Vector2Int bottomLeft, Vector2Int topRight)
        {
            List<Vector2Int> res = new List<Vector2Int>();

            for (int i = Mathf.Min(bottomLeft.x,topRight.x); i < Mathf.Max(bottomLeft.x,topRight.x); i++)
            {
                for (int j = Mathf.Min(bottomLeft.y,topRight.y); j < Mathf.Max(bottomLeft.y,topRight.y); j++)
                {
                    res.Add(new Vector2Int(i,j));
                }
            }

            return res.ToArray();
        }
        
        private static List<Vector2Int> _cluster;
        public static Vector2Int[] GetClusterAt(Vector2Int centre, float falloff, int maxDistance)
        {
            _cluster = new List<Vector2Int>();
            GetClusterAtPrivate(centre,falloff,maxDistance,1,0);
            return _cluster.ToArray();
        }

        private static void GetClusterAtPrivate(Vector2Int v, float falloff, int maxDistance, float randomVal, int distanceElapsed)
        {
            if (distanceElapsed > maxDistance)
            {
                return;
            }
            if (Random.value < randomVal)
            {
                _cluster.Add(v);
                Vector2Int u = new Vector2Int(v.x,v.y+1);
                Vector2Int d = new Vector2Int(v.x,v.y-1);
                Vector2Int l = new Vector2Int(v.x-1,v.y);
                Vector2Int r = new Vector2Int(v.x+1,v.y);
            
                if (!_cluster.Contains(u)) GetClusterAtPrivate(u,falloff,maxDistance,randomVal/falloff,distanceElapsed+1);
                if (!_cluster.Contains(d)) GetClusterAtPrivate(d,falloff,maxDistance,randomVal/falloff,distanceElapsed+1);
                if (!_cluster.Contains(l)) GetClusterAtPrivate(l,falloff,maxDistance,randomVal/falloff,distanceElapsed+1);
                if (!_cluster.Contains(r)) GetClusterAtPrivate(r,falloff,maxDistance,randomVal/falloff,distanceElapsed+1);
            }
        }
        
        public static int[,] RotateMatrix(int[,] matrix, bool clockwise = true)
        {
            int n = matrix.GetLength(0);
            int[,] ret = new int[n, n];

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (clockwise) ret[i, j] = matrix[n - j - 1, i];
                    else           ret[i, j] = matrix[j,n - i - 1];
                }
            }

            return ret;
        }
        
        private static List<Vector2Int> _flooded;
        public static Vector2Int[] FloodFill(Vector2Int v, HashSet<Vector2Int> toSearchThrough)
        {
            _flooded = new List<Vector2Int>();
            FloodFillPrivate(v,toSearchThrough);
            return _flooded.ToArray();
        }
    
        private static void FloodFillPrivate(Vector2Int v, HashSet<Vector2Int> toSearchThrough)
        {
            if (toSearchThrough.Contains(v) && !_flooded.Contains(v))
            {
                _flooded.Add(v);
                Vector2Int u = new Vector2Int(v.x,v.y+1);
                Vector2Int d = new Vector2Int(v.x,v.y-1);
                Vector2Int l = new Vector2Int(v.x-1,v.y);
                Vector2Int r = new Vector2Int(v.x+1,v.y);
            
                if (toSearchThrough.Contains(u)) FloodFillPrivate(u,toSearchThrough);
                if (toSearchThrough.Contains(d)) FloodFillPrivate(d,toSearchThrough);
                if (toSearchThrough.Contains(l)) FloodFillPrivate(l,toSearchThrough);
                if (toSearchThrough.Contains(r)) FloodFillPrivate(r,toSearchThrough);
            }
        }
    }
}