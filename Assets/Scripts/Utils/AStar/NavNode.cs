using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.AStar
{
    /// <summary>
    /// USAGE
    /// PathNode[,] nodes = new PathNode[MapSize,MapSize];
    /// // Add make some nodes walkable etc
    /// SpatialAStar<PathNode, Object> aStar = new SpatialAStar<PathNode, Object>(nodes);
    /// LinkedList<PathNode> path = aStar.Search(srcNode.index, dstNode.index, null);
    /// </summary>
    
    public class PathNode : IPathNode<Object>
    {
        public Vector3 Pos;
        public Vector2Int Index;
        public Boolean IsTraversable;

        public PathNode(Vector3 p, Vector2Int i, bool isTraversable)
        {
            Pos = p;
            Index = i;
            IsTraversable = isTraversable;
        }
        
        public bool IsWalkable(Object inContext)
        {
            return IsTraversable;
        }
    }
}