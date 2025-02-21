using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Giacomo
{
    public class GridManager : Singleton<GridManager>
    {
        [ShowInInspector] Dictionary<Vector2Int, Tile> tiles;

        public bool isSetup {  get; protected set; }
        protected void Awake()
        {
            if (isSetup) return;
            tiles = new Dictionary<Vector2Int, Tile>();
            BroadcastMessage("SetupTile");
            isSetup = true;
        }


        public void AddTile(Vector2Int position, Tile tile)
        {
            if (tiles.ContainsKey(position))
            {
                Debug.Log("the slot " + position + " is already occupied");
                return;
            }

            tiles[position] = tile;
            tile.position = position;

            GetAdjacentTiles(position).ForEach(x=>x.OnNearbyTileChanged());
        }

        public List<Tile> GetAdjacentTiles(Vector2Int position)
        {
            List<Tile> neighbors = new List<Tile>();

            if (tiles.TryGetValue(position + Vector2Int.right, out Tile right))
                neighbors.Add(right);
            if (tiles.TryGetValue(position + Vector2Int.left, out Tile left))
                neighbors.Add(left);
            if (tiles.TryGetValue(position + Vector2Int.up, out Tile up))
                neighbors.Add(up);
            if (tiles.TryGetValue(position + Vector2Int.down, out Tile down))
                neighbors.Add(down);

            return neighbors;
        }

        public Tile GetHome()
        {
            if (!isSetup)
                Awake();
            return tiles.FirstOrDefault(x => x.Value.isHome).Value;
        }


        public static Vector2Int FixCoordinates(Vector2 position) => new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        public static Vector2Int FixCoordinates(Vector3 position) => new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        public Tile Get(Vector3 position) => Get(FixCoordinates(position));
        public Tile Get(Vector2Int position)
        {
            if (tiles.ContainsKey(position))
                return tiles[position];
            return null;
        }

        public Dictionary<Vector2Int, Tile> GetAll() => tiles;

        public void Remove(Tile tile)
        {
            tiles?.Remove(tiles.FirstOrDefault(x => x.Value == tile).Key);
            GetAdjacentTiles(tile.position).ForEach(x=>x.OnNearbyTileChanged());
        }
        public void Remove(Vector2Int position)
        {
            tiles?.Remove(position);
            GetAdjacentTiles(position).ForEach(x=>x.OnNearbyTileChanged());
        }


        public bool Contains(Vector2Int position) => tiles.ContainsKey(position);
        public bool Contains(Tile tile) => tiles.ContainsValue(tile);
    }
}