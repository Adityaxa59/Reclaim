using Giacomo;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GridImportExport
{
    const string TILE_LOCATION = "TowerDefense/Prefabs/Tiles/";

    public static void ExportGrid(GridManager grid, string saveLocation)
    {
        var tiles = grid.GetAll().Values.ToArray();
        
        var levelInfo = new LevelInfo(tiles);
        string gridData = JsonUtility.ToJson(levelInfo);
        File.WriteAllText(saveLocation, gridData);
    }

    public static void ImportGrid(GridManager grid, string loadLocation)
    {
        grid.Clear();
        string gridData = File.ReadAllText(loadLocation);
        var levelInfo = JsonUtility.FromJson<LevelInfo>(gridData);

        foreach (var t in levelInfo.tiles)
        {
            var tilePath = Path.Combine(TILE_LOCATION, t.tileId);
            var tilePrefab = Resources.Load(tilePath);
            var tile = GameObject.Instantiate(tilePrefab).GetComponent<Tile>();
            grid.AddTile(t.position, tile);
        }
    }

    [System.Serializable]
    class LevelInfo
    {
        public List<SerializedTile> tiles;
        public LevelInfo(Tile[] tiles)
        {
            var serializedTiles = new List<SerializedTile>();
            foreach (var tile in tiles)
                serializedTiles.Add(new SerializedTile(tile));

            this.tiles = serializedTiles;
        }
        
        
        
        
        [System.Serializable]
        public class SerializedTile
        {
            public string tileId;
            public Vector2Int position;
            /*public bool IsWalkable;
            public bool isHome;
            public bool canBuildOver;*/
            public SerializedTile(Tile tile)
            {
                tileId = tile.tileId;
                position = tile.position;
                /*IsWalkable = tile.IsWalkable;
                isHome = tile.isHome;
                canBuildOver = tile.canBuildOver;*/
            }
        }
    }
}
