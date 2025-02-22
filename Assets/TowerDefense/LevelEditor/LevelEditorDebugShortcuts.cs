using Giacomo;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.Utilities;

public class LevelEditorDebugShortcuts : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Tile[] tiles = FindObjectsByType(typeof(Tile), FindObjectsSortMode.None) as Tile[];
                foreach (var t in tiles)
                {
                    if (!GridManager.Instance.Contains(t) && LevelEditor.Instance.lastHoveredTile != t)
                        Destroy(t.gameObject);
                }
            }

            if (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.A))
            {
                GridManager.Instance.GetAll().Clear();
                Tile[] tiles = FindObjectsByType(typeof(Tile), FindObjectsSortMode.None) as Tile[];
                tiles.ForEach((t) => { if (t != LevelEditor.Instance.placingTile) Destroy(t.gameObject); });
            }

        }
    }


}
