using Giacomo;
using UnityEngine;

public class LevelEditor : Singleton<LevelEditor>
{
    [SerializeField] protected GameObject defaultTile;
    [SerializeField] public Tile placingTile;

    protected GameObject preview;
    protected SpriteRenderer[] previewRenderers;

    Vector2Int lastHoveredCoords;
    public Tile lastHoveredTile;

    Vector2Int lastPlacedCoords;
    bool changedTileAfterPlacing = true;

    private void Start()
    {
        SetPlacingTile(defaultTile);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            var tiles = GridManager.Instance.GetAll();

            foreach(var t in tiles)
            {
                if (t.Key != t.Value.position)
                    Debug.Log(t.Key + " " + t.Value.position);
            }
        }


        //get current tile
        var mousePos = Helpers.Camera.ScreenToWorldPoint(Input.mousePosition);
        var intCoords = GridManager.FixCoordinates(mousePos);

        if(intCoords != lastPlacedCoords)
            changedTileAfterPlacing = true;

        if (!placingTile || !changedTileAfterPlacing)
            return;

        DisplayPreview(intCoords);

        //place tile
        bool place = Input.GetMouseButton(0);
        bool remove = Input.GetMouseButton(1);

        if(place && changedTileAfterPlacing && !Helpers.IsOverUI)
        {
            if(lastHoveredTile)
                Destroy(lastHoveredTile.gameObject);

            lastHoveredTile = null;
            changedTileAfterPlacing = false;
            lastPlacedCoords = intCoords;

            if(!GridManager.Instance.Contains(placingTile))
                GridManager.Instance.AddTile(intCoords, placingTile);

            var t = placingTile;
            placingTile = null;
            SetPlacingTile(t.gameObject);
            return;
        }

        if (remove && GridManager.Instance.Contains(intCoords))
        {
            if(lastHoveredTile)
                Destroy(lastHoveredTile.gameObject);

            lastHoveredTile = null;
            changedTileAfterPlacing = false;
            lastPlacedCoords = intCoords;
        }
    }

    void DisplayPreview(Vector2Int pos)
    {
        if (lastHoveredCoords == pos)
            return;

        lastHoveredCoords = pos;

        //remove old preview
        if (GridManager.Instance.Contains(placingTile))
            GridManager.Instance.Remove(placingTile);

        //replace old tile
        if (lastHoveredTile)
        {
            GridManager.Instance.AddTile(lastHoveredTile.position, lastHoveredTile);
            lastHoveredTile.gameObject.SetActive(true);
        }

        //cache overridden tile
        lastHoveredTile = GridManager.Instance.Get(pos);
        if (lastHoveredTile)
        {
            GridManager.Instance.Remove(lastHoveredTile);
            lastHoveredTile?.gameObject.SetActive(false);
        }

        //add preview
        GridManager.Instance.AddTile(pos, placingTile.GetComponent<Tile>());
    }


    public void EnablePreview(bool enabled)
    {
        var toEnable = enabled ? placingTile : lastHoveredTile;
        var toDisable = enabled ? lastHoveredTile : placingTile;

        if (lastHoveredTile)
        {
            GridManager.Instance.AddTile(lastHoveredTile.position, lastHoveredTile);
            lastHoveredTile.gameObject.SetActive(true);
        }
        if (GridManager.Instance.Contains(placingTile))
        {
            GridManager.Instance.Remove(placingTile);

        }
    }

    public void SetPlacingTile(GameObject t)
    {
        if(placingTile)
            Destroy(placingTile.gameObject);

        var mousePos = Helpers.Camera.ScreenToWorldPoint(Input.mousePosition);
        var intCoords = GridManager.FixCoordinates(mousePos);

        placingTile = Instantiate(t).GetComponent<Tile>();
        placingTile.name = "placing";
        placingTile.gameObject.SetActive(true);
        placingTile.transform.position = new Vector3(intCoords.x, intCoords.y);
        lastHoveredCoords = Vector2Int.one * int.MaxValue;
    }
}
