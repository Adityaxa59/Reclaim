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

            SetPlacingTile(placingTile.gameObject);
            return;
        }

        if (remove && GridManager.Instance.Contains(intCoords))
        {
            if(lastHoveredTile)
                Destroy(lastHoveredTile.gameObject);

            lastHoveredTile = null;
            changedTileAfterPlacing = false;
            lastPlacedCoords = intCoords;

            //GridManager.Instance.Remove();
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


    public void SetPlacingTile(GameObject t)
    {
        var mousePos = Helpers.Camera.ScreenToWorldPoint(Input.mousePosition);
        var intCoords = GridManager.FixCoordinates(mousePos);

        placingTile = Instantiate(t).GetComponent<Tile>();
        placingTile.name = "placing";
        placingTile.gameObject.SetActive(true);
        placingTile.transform.position = new Vector3(intCoords.x, intCoords.y);
    }
}
