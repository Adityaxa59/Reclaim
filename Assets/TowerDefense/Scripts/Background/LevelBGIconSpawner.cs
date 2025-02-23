using Sirenix.Utilities;
using UnityEngine;

namespace Giacomo
{
    public class LevelBGIconSpawner : BGIconSpawner
    {
        protected override void SpawnTiles()
        {
            GridManager.Instance.OnTileAdded += DisableBGIconsNearTile;
            GridManager.Instance.OnTileRemoved += OnTileRemoved;

            base.SpawnTiles();

            foreach (var tile in GridManager.Instance.GetAll())
                DisableBGIconsNearTile(tile.Value);
        }

        private void OnDestroy()
        {
            if(!GridManager.Instance)
                return;
            GridManager.Instance.OnTileAdded -= DisableBGIconsNearTile;
            GridManager.Instance.OnTileRemoved -= OnTileRemoved;
        }

        public void DisableBGIconsNearTile(Tile tile)
        {
            if(grid.TryGetValue(tile.position, out var go))
                go.SetActive(false);
            if (grid.TryGetValue(tile.position + Vector2Int.down, out go))
                go.SetActive(false);
            if (grid.TryGetValue(tile.position + Vector2Int.left, out go))
                go.SetActive(false);
            if (grid.TryGetValue(tile.position + new Vector2Int(-1, -1), out go))
                go.SetActive(false);
        }
        public void OnTileRemoved(Vector2Int tile)
        {
            if (grid.TryGetValue(tile, out var go))
                go.SetActive(CanEnable(tile));
            if(grid.TryGetValue(tile + Vector2Int.down, out go))
                go.SetActive(CanEnable(tile + Vector2Int.down));
            if (grid.TryGetValue(tile + Vector2Int.left, out go))
                go.SetActive(CanEnable(tile + Vector2Int.left));
            if (grid.TryGetValue(tile + new Vector2Int(-1, -1), out go))
                go.SetActive(CanEnable(tile + new Vector2Int(-1, -1)));
        }
        protected bool CanEnable(Vector2Int position)
        {
            return !GridManager.Instance.Contains(position)
                && !GridManager.Instance.Contains(position + Vector2Int.up)
                && !GridManager.Instance.Contains(position + Vector2Int.right)
                && !GridManager.Instance.Contains(position + new Vector2Int(1, 1));
        }    

    }
}