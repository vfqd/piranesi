using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.AStar
{
    public interface IPathNode<TUserContext>
    {
        Boolean IsWalkable(TUserContext inContext);
    }

    /// <summary>
    /// Uses about 50 MB for a 1024x1024 grid.
    /// </summary>
    public class SpatialAStar<TPathNode, TUserContext> where TPathNode : IPathNode<TUserContext>
    {
        private OpenCloseMap _closedSet;
        private OpenCloseMap _openSet;
        private PriorityQueue<PathNode> _mOrderedOpenSet;
        private PathNode[,] _mCameFrom;
        private OpenCloseMap _mRuntimeGrid;
        private PathNode[,] _mSearchSpace;

        public TPathNode[,] SearchSpace { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        protected class PathNode : IPathNode<TUserContext>, IComparer<PathNode>, IIndexedObject
        {
            public static readonly PathNode COMPARER = new PathNode(0, 0, default(TPathNode));

            public TPathNode UserContext { get; internal set; }
            public Double G { get; internal set; }
            public Double H { get; internal set; }
            public Double F { get; internal set; }
            public int Index { get; set; }

            public Boolean IsWalkable(TUserContext inContext)
            {
                return UserContext.IsWalkable(inContext);
            }

            public int X { get; internal set; }
            public int Y { get; internal set; }

            public int Compare(PathNode x, PathNode y)
            {
                if (x.F < y.F)
                    return -1;
                else if (x.F > y.F)
                    return 1;

                return 0;
            }

            public PathNode(int inX, int inY, TPathNode inUserContext)
            {
                X = inX;
                Y = inY;
                UserContext = inUserContext;
            }
        }

        public SpatialAStar(TPathNode[,] inGrid)
        {
            SearchSpace = inGrid;
            Width = inGrid.GetLength(0);
            Height = inGrid.GetLength(1);
            _mSearchSpace = new PathNode[Width, Height];
            _closedSet = new OpenCloseMap(Width, Height);
            _openSet = new OpenCloseMap(Width, Height);
            _mCameFrom = new PathNode[Width, Height];
            _mRuntimeGrid = new OpenCloseMap(Width, Height);
            _mOrderedOpenSet = new PriorityQueue<PathNode>(PathNode.COMPARER);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (inGrid[x, y] == null)
                        throw new ArgumentNullException();

                    _mSearchSpace[x, y] = new PathNode(x, y, inGrid[x, y]);
                }
            }
        }

        protected virtual Double Heuristic(PathNode inStart, PathNode inEnd)
        {
            return Math.Sqrt((inStart.X - inEnd.X) * (inStart.X - inEnd.X) + (inStart.Y - inEnd.Y) * (inStart.Y - inEnd.Y));
        }

        private static readonly Double SQRT_2 = Math.Sqrt(2);

        protected virtual Double NeighborDistance(PathNode inStart, PathNode inEnd)
        {
            int diffX = Math.Abs(inStart.X - inEnd.X);
            int diffY = Math.Abs(inStart.Y - inEnd.Y);

            switch (diffX + diffY)
            {
                case 1: return 1;
                case 2: return SQRT_2;
                case 0: return 0;
                default:
                    throw new ApplicationException();
            }
        }

        //private List<Int64> elapsed = new List<long>();

        /// <summary>
        /// Returns null, if no path is found. Start- and End-Node are included in returned path. The user context
        /// is passed to IsWalkable().
        /// </summary>
        public LinkedList<TPathNode> Search(Vector2Int inStartNode, Vector2Int inEndNode, TUserContext inUserContext)
        {
            PathNode startNode = _mSearchSpace[inStartNode.x, inStartNode.y];
            PathNode endNode = _mSearchSpace[inEndNode.x, inEndNode.y];

            //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            //watch.Start();

            if (startNode == endNode)
                return new LinkedList<TPathNode>(new TPathNode[] {startNode.UserContext});

            PathNode[] neighborNodes = new PathNode[8];

            _closedSet.Clear();
            _openSet.Clear();
            _mRuntimeGrid.Clear();
            _mOrderedOpenSet.Clear();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    _mCameFrom[x, y] = null;
                }
            }

            startNode.G = 0;
            startNode.H = Heuristic(startNode, endNode);
            startNode.F = startNode.H;

            _openSet.Add(startNode);
            _mOrderedOpenSet.Push(startNode);

            _mRuntimeGrid.Add(startNode);

            int nodes = 0;


            while (!_openSet.IsEmpty)
            {
                PathNode x = _mOrderedOpenSet.Pop();

                if (x == endNode)
                {
                    // watch.Stop();

                    //elapsed.Add(watch.ElapsedMilliseconds);

                    LinkedList<TPathNode> result = ReconstructPath(_mCameFrom, _mCameFrom[endNode.X, endNode.Y]);

                    result.AddLast(endNode.UserContext);

                    return result;
                }

                _openSet.Remove(x);
                _closedSet.Add(x);

                StoreNeighborNodes(x, neighborNodes);

                for (int i = 0; i < neighborNodes.Length; i++)
                {
                    PathNode y = neighborNodes[i];
                    Boolean tentativeIsBetter;

                    if (y == null)
                        continue;

                    if (!y.UserContext.IsWalkable(inUserContext))
                        continue;

                    if (_closedSet.Contains(y))
                        continue;

                    nodes++;

                    Double tentativeGScore = _mRuntimeGrid[x].G + NeighborDistance(x, y);
                    Boolean wasAdded = false;

                    if (!_openSet.Contains(y))
                    {
                        _openSet.Add(y);
                        tentativeIsBetter = true;
                        wasAdded = true;
                    }
                    else if (tentativeGScore < _mRuntimeGrid[y].G)
                    {
                        tentativeIsBetter = true;
                    }
                    else
                    {
                        tentativeIsBetter = false;
                    }

                    if (tentativeIsBetter)
                    {
                        _mCameFrom[y.X, y.Y] = x;

                        if (!_mRuntimeGrid.Contains(y))
                            _mRuntimeGrid.Add(y);

                        _mRuntimeGrid[y].G = tentativeGScore;
                        _mRuntimeGrid[y].H = Heuristic(y, endNode);
                        _mRuntimeGrid[y].F = _mRuntimeGrid[y].G + _mRuntimeGrid[y].H;

                        if (wasAdded)
                            _mOrderedOpenSet.Push(y);
                        else
                            _mOrderedOpenSet.Update(y);
                    }
                }
            }

            return null;
        }

        private LinkedList<TPathNode> ReconstructPath(PathNode[,] cameFrom, PathNode currentNode)
        {
            LinkedList<TPathNode> result = new LinkedList<TPathNode>();

            ReconstructPathRecursive(cameFrom, currentNode, result);

            return result;
        }

        private void ReconstructPathRecursive(PathNode[,] cameFrom, PathNode currentNode, LinkedList<TPathNode> result)
        {
            PathNode item = cameFrom[currentNode.X, currentNode.Y];

            if (item != null)
            {
                ReconstructPathRecursive(cameFrom, item, result);

                result.AddLast(currentNode.UserContext);
            }
            else
                result.AddLast(currentNode.UserContext);
        }

        private void StoreNeighborNodes(PathNode inAround, PathNode[] inNeighbors)
        {
            int x = inAround.X;
            int y = inAround.Y;

            if ((x > 0) && (y > 0))
                inNeighbors[0] = _mSearchSpace[x - 1, y - 1];
            else
                inNeighbors[0] = null;

            if (y > 0)
                inNeighbors[1] = _mSearchSpace[x, y - 1];
            else
                inNeighbors[1] = null;

            if ((x < Width - 1) && (y > 0))
                inNeighbors[2] = _mSearchSpace[x + 1, y - 1];
            else
                inNeighbors[2] = null;

            if (x > 0)
                inNeighbors[3] = _mSearchSpace[x - 1, y];
            else
                inNeighbors[3] = null;

            if (x < Width - 1)
                inNeighbors[4] = _mSearchSpace[x + 1, y];
            else
                inNeighbors[4] = null;

            if ((x > 0) && (y < Height - 1))
                inNeighbors[5] = _mSearchSpace[x - 1, y + 1];
            else
                inNeighbors[5] = null;

            if (y < Height - 1)
                inNeighbors[6] = _mSearchSpace[x, y + 1];
            else
                inNeighbors[6] = null;

            if ((x < Width - 1) && (y < Height - 1))
                inNeighbors[7] = _mSearchSpace[x + 1, y + 1];
            else
                inNeighbors[7] = null;
        }

        private class OpenCloseMap
        {
            private PathNode[,] _mMap;
            public int Width { get; private set; }
            public int Height { get; private set; }
            public int Count { get; private set; }

            public PathNode this[Int32 x, Int32 y]
            {
                get
                {
                    return _mMap[x, y];
                }
            }

            public PathNode this[PathNode node]
            {
                get
                {
                    return _mMap[node.X, node.Y];
                }
            }

            public bool IsEmpty
            {
                get
                {
                    return Count == 0;
                }
            }

            public OpenCloseMap(int inWidth, int inHeight)
            {
                _mMap = new PathNode[inWidth, inHeight];
                Width = inWidth;
                Height = inHeight;
            }

            public void Add(PathNode inValue)
            {
                PathNode item = _mMap[inValue.X, inValue.Y];

            #if DEBUG
                if (item != null)
                    throw new ApplicationException();
            #endif

                Count++;
                _mMap[inValue.X, inValue.Y] = inValue;
            }

            public bool Contains(PathNode inValue)
            {
                PathNode item = _mMap[inValue.X, inValue.Y];

                if (item == null)
                    return false;

            #if DEBUG
                if (!inValue.Equals(item))
                    throw new ApplicationException();
            #endif

                return true;
            }

            public void Remove(PathNode inValue)
            {
                PathNode item = _mMap[inValue.X, inValue.Y];

            #if DEBUG
                if (!inValue.Equals(item))
                    throw new ApplicationException();
            #endif

                Count--;
                _mMap[inValue.X, inValue.Y] = null;
            }

            public void Clear()
            {
                Count = 0;

                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        _mMap[x, y] = null;
                    }
                }
            }
        }
    }
}