using Giacomo;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEditor;

public class LevelEditorShortcuts : MonoBehaviour
{
    [SerializeField] List<GameObject> tiles;


    void Update()
    {
        // 1, 2... > Select tiles
        if(Input.GetKeyDown(KeyCode.Alpha1))
            LevelEditor.Instance.SetPlacingTile(tiles[0]);
        if(Input.GetKeyDown(KeyCode.Alpha2))
            LevelEditor.Instance.SetPlacingTile(tiles[1]);
        if(Input.GetKeyDown(KeyCode.Alpha3))
            LevelEditor.Instance.SetPlacingTile(tiles[2]);
        if(Input.GetKeyDown(KeyCode.Alpha4))
            LevelEditor.Instance.SetPlacingTile(tiles[3]);

        //Ctrl+S > Save
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
        {
            var path = EditorUtility.SaveFilePanel(
                title:      "Save level",
                directory:  "",
                defaultName:"level.td",
                extension:  "td");

            if (path.Length != 0)
            {
                GridImportExport.ExportGrid(GridManager.Instance, path);
            }
        }
        
        //Ctrl+L > Load level
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L))
        {
            var path = EditorUtility.OpenFilePanel(
                title:      "Load level",
                directory:  "",
                extension:  "td");

            if (path.Length != 0)
            {
                GridImportExport.ImportGrid(GridManager.Instance, path);
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl))
        {
            //Ctrl+Shift+F > Fix broken tiles
            if (Input.GetKeyDown(KeyCode.F))
            {
                Tile[] tiles = FindObjectsByType(typeof(Tile), FindObjectsSortMode.None) as Tile[];
                foreach (var t in tiles)
                {
                    if (!GridManager.Instance.Contains(t) && LevelEditor.Instance.lastHoveredTile != t)
                        Destroy(t.gameObject);
                }
            }

            //Ctrl+Shift+C+A > Clear grid
            if (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.A))
            {
                GridManager.Instance.Clear();
                //GridManager.Instance.GetAll().Clear();
                //Tile[] tiles = FindObjectsByType(typeof(Tile), FindObjectsSortMode.None) as Tile[];
                //tiles.ForEach((t) => { if (t != LevelEditor.Instance.placingTile) Destroy(t.gameObject); });
            }

        }
    }


}
