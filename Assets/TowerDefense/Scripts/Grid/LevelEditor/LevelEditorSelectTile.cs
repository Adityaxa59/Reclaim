using UnityEngine;

public class LevelEditorSelectTile : MonoBehaviour
{
    [SerializeField] GameObject tile;


    public void Click()
    {
        LevelEditor.Instance.SetPlacingTile(tile);
    }
}
