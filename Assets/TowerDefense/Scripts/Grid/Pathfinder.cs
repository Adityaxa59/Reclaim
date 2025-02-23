using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Giacomo
{
    public class Pathfinder
    {
        public static List<Tile> FindNearestTile(GridManager grid, Tile startTile, Func<Tile, bool> isDestination)
        {
            Dictionary<Tile, PathfinderTile> tileData = new Dictionary<Tile, PathfinderTile>();
            List<Tile> openSet = new List<Tile>();
            HashSet<Tile> closedSet = new HashSet<Tile>();

            openSet.Add(startTile);
            tileData.Add(startTile, new PathfinderTile(startTile, 0, int.MaxValue));

            while (openSet.Count > 0)
            {
                PathfinderTile currentTile = tileData[openSet[0]];

                if(isDestination(currentTile.originalTile))
                    return RetracePath(tileData[startTile], currentTile);

                openSet.Remove(currentTile.originalTile);
                closedSet.Add(currentTile.originalTile);

                foreach (Tile neighbour in grid.GetAdjacentTiles(currentTile.originalTile.position))
                {
                    if (!neighbour || !neighbour.IsWalkable || closedSet.Contains(neighbour))
                        continue;

                    int newMovementCostToNeighbour = currentTile.gCost + GetDistance(currentTile.originalTile, neighbour);

                    if (!tileData.ContainsKey(neighbour))
                    {
                        tileData.Add(neighbour, new PathfinderTile(neighbour, newMovementCostToNeighbour, currentTile.hCost - 1, currentTile));
                        openSet.Add(neighbour);
                    }
                }
            }

            return closedSet.ToList();
        }

        public static List<Tile> FindPath(GridManager grid, Tile startTile, Tile endTile)
        {
            Dictionary<Tile, PathfinderTile> tileData = new Dictionary<Tile, PathfinderTile>();
            List<Tile> openSet = new List<Tile>();
            HashSet<Tile> closedSet = new HashSet<Tile>();

            openSet.Add(startTile);
            tileData.Add(startTile, new PathfinderTile(startTile, 0, GetDistance(startTile, endTile)));

            while (openSet.Count > 0)
            {
                PathfinderTile currentTile = tileData[openSet[0]];
                for (int i = 1; i < openSet.Count; i++)
                    if (tileData[openSet[i]].fCost < currentTile.fCost || (
                        tileData[openSet[i]].fCost == currentTile.fCost && tileData[openSet[i]].hCost < currentTile.hCost))
                        currentTile = tileData[openSet[i]];

                openSet.Remove(currentTile.originalTile);
                closedSet.Add(currentTile.originalTile);

                if (currentTile.originalTile == endTile)
                    return RetracePath(tileData[startTile], tileData[endTile]);


                foreach (Tile neighbour in grid.GetAdjacentTiles(currentTile.originalTile.position))
                {
                    if (!neighbour || !neighbour.IsWalkable || closedSet.Contains(neighbour))
                        continue;
                    int newMovementCostToNeighbour = currentTile.gCost + GetDistance(currentTile.originalTile, neighbour);

                    if (!tileData.ContainsKey(neighbour))
                    {
                        tileData.Add(neighbour, new PathfinderTile(neighbour, newMovementCostToNeighbour, GetDistance(neighbour, endTile), currentTile));
                        openSet.Add(neighbour);
                    }

                    if (newMovementCostToNeighbour < tileData[neighbour].gCost)
                    {
                        tileData[neighbour].gCost = newMovementCostToNeighbour;
                        tileData[neighbour].parent = currentTile;
                    }
                }
            }

            List<Tile> noPath = new List<Tile>();
            noPath.Add(startTile);
            return noPath;
        }

        static List<Tile> RetracePath(PathfinderTile startTile, PathfinderTile endTile)
        {
            List<Tile> path = new List<Tile>();
            PathfinderTile currentTile = endTile;

            while (currentTile != startTile)
            {
                path.Add(currentTile.originalTile);
                currentTile = currentTile.parent;
            }
            //path.Add(startTile.originalTile);
            path.Reverse();
            return path;
        }

        public static int GetDistance(Tile t1, Tile t2)
        {
            return Mathf.RoundToInt(Mathf.Abs(t1.position.x - t2.position.x) + Mathf.Abs(t1.position.y - t2.position.y));
        }

        class PathfinderTile
        {
            public bool walkable;
            public int gCost;
            public int hCost;
            public int fCost { get { return gCost + hCost; } }
            public Tile originalTile;
            public PathfinderTile parent;


            public PathfinderTile(Tile _tile, int _gCost, int _hCost, PathfinderTile _parent = null)
            {
                originalTile = _tile;
                walkable = _tile.IsWalkable;
                gCost = _gCost;
                hCost = _gCost;
                parent = _parent;
            }

        }
    }
}