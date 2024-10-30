using UnityEngine;

namespace Giacomo
{
    //connecting logic doesn't need inheritance anymore, could make this as a separate MonoBehaviour
    public class ConnectingTile : Tile
    {
        [SerializeField] GameObject leftBorder;
        [SerializeField] GameObject rightBorder;
        [SerializeField] GameObject upBorder;
        [SerializeField] GameObject downBorder;

        private void Start()
        {
            var neighbours = GridManager.Instance.GetNeighbors(position);
            foreach (var neighbor in neighbours)
            {
                if (!neighbor.type.Equals(type))
                    continue;

                var diff = neighbor.position - position;

                if (diff == Vector2Int.left)
                    leftBorder.SetActive(false);
                if (diff == Vector2Int.right)
                    rightBorder.SetActive(false);
                if (diff == Vector2Int.up)
                    upBorder.SetActive(false);
                if (diff == Vector2Int.down)
                    downBorder.SetActive(false);
            }
        }


    }

}