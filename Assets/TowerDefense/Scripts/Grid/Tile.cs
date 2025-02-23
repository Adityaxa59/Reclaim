using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Giacomo
{
    [System.Serializable]
    public class Tile : MonoBehaviour
    {
        public string tileId;
        public enum TileType { path, ground, decoration }

        public TileType type;
        public Vector2Int position;
        public bool IsWalkable;
        public bool isHome;
        public bool canBuildOver;

        public Tower tower;

        private bool setupOnEnable;

        private void OnEnable()
        {
            if (setupOnEnable)
                SetupTile();
        }

        public virtual void SetupTile()
        {
            if (!gameObject.activeInHierarchy)
            {
                setupOnEnable = true;
                return;
            }
            setupOnEnable = false;


            UpdatePosition();

            var t = GetComponentInChildren<Tower>();
            if(t != null)
                PlaceTower(t);
        }

        private void OnDestroy()
        {
            if(GridManager.Instance && GridManager.Instance.Contains(this))
                GridManager.Instance.Remove(this);
        }


        [HideInPlayMode, Button("Update Tile Position")]
        private void UpdatePositionOutsidePlayMode()
        {
            position = new Vector2Int(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y));
            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }
        [HideInEditorMode, Button("Update Tile Position")]
        protected void UpdatePosition()
        {
            if (GridManager.Instance.Contains(this))
                GridManager.Instance.Remove(this);

            position = new Vector2Int(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y));

            if (GridManager.Instance.Contains(position))
                return;

            transform.position = new Vector3(position.x, position.y, transform.position.z);

            GridManager.Instance.AddTile(position, this);
        }

        public virtual void OnNearbyTileChanged()
        {

        }

        public bool CanPlace()
        {
            return canBuildOver && (tower == null);
        }
        public void PlaceTower(Tower t)
        {
            if (!CanPlace())
                return;
            tower = t;
            t.gameObject.SetActive(true);
            t.gameObject.transform.position = transform.position;
            t.gameObject.transform.parent = transform;
            t.Tile = this;
            name = "Tile [" + t.name + "]";
        }


    }

}