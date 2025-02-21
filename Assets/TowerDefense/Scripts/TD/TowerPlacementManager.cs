using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;


namespace Giacomo
{

    public class TowerPlacementManager : Singleton<TowerPlacementManager>
    {
        [ShowInInspector, ReadOnly] 
        public Tower placingTower { get; private set; }

        protected Action onPlace;
        protected Action onCancelPlacing;

        protected GameObject preview;
        protected SpriteRenderer[] previewRenderers;

        protected ScaleWithStat rangePreview;
        protected bool startedPlacingThisFrame;

        private void Awake()
        {
            rangePreview = Instantiate(Resources.Load("TowerDefense/Prefabs/RangePreview"), transform).GetComponent<ScaleWithStat>();
            rangePreview.multiply = 2;
            rangePreview.gameObject.SetActive(false);
        }


        public void Update()
        {
            if(placingTower == null)
                return;

            //prevent placing the tower in the same frame it's being selected
            if (startedPlacingThisFrame)
            {
                startedPlacingThisFrame = false;
                return;
            }

            //cancel placing
            if (Input.GetKey(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            { 
                CancelPlacing();
                return;
            }


            //get current tile
            var mousePos = Helpers.Camera.ScreenToWorldPoint(Input.mousePosition);
            var intCoords = GridManager.FixCoordinates(mousePos);
            Tile hoveringTile = GridManager.Instance.Get(intCoords);

            //place tower
            if (Input.GetMouseButtonDown(0))
            {
                if (hoveringTile != null && hoveringTile.CanPlace())
                {
                    InputManager.Instance.SetPlacingStatus(false);
                    var t = Instantiate(placingTower.gameObject).GetComponent<Tower>();
                    t.name = placingTower.name;
                    hoveringTile.PlaceTower(t);
                    onPlace?.Invoke();

                    AudioController.Instance.PlaySound2D("ui_confirm");

                    StopPlacing();
                    return;
                }
            }


            //draw preview
            Vector3 coords = new Vector3(intCoords.x, intCoords.y);
            rangePreview.transform.position = coords;
            preview.transform.position = coords;
            if (hoveringTile && hoveringTile.CanPlace())
            {
                previewRenderers.ForEach(x => x.color = Color.white);
            }
            else
            {
                previewRenderers.ForEach(x => x.color = Color.red);
            }
        }

        public void CancelPlacing()
        {
            onCancelPlacing?.Invoke();
            StopPlacing();
            AudioController.Instance.PlaySound2D("ui_cancel");
        }

        protected void StopPlacing()
        {
            placingTower = null;
            onPlace = null;
            onCancelPlacing = null;
            Destroy(preview);
            rangePreview.gameObject.SetActive(false);
            InputManager.Instance.SetPlacingStatus(false);
        }

        public void StartPlacing(Tower tower, Action onPlace = null, Action cancelPlacing = null)
        {
            if (placingTower == tower)
            {
                CancelPlacing();
                return;
            }

            InputManager.Instance.SetPlacingStatus(true);
            this.placingTower = tower;
            this.onPlace = onPlace;
            this.onCancelPlacing = cancelPlacing;
            startedPlacingThisFrame = true;

            //Range preview
            // range = tower.b_maxRange*2;
            //float range =  * 2;

            var hoveringCoords = GridManager.FixCoordinates(Input.mousePosition);
            rangePreview.transform.position = new Vector3(hoveringCoords.x, hoveringCoords.y, rangePreview.transform.position.z);
            
            rangePreview.SetStat(tower.MaxRange);
            //rangePreview.transform.localScale = Vector3.one * range;
            rangePreview.gameObject.SetActive(true);

            //Tower preview
            var pos = Helpers.Camera.ScreenToWorldPoint(Input.mousePosition);
            preview = Instantiate(tower.transform.Find("GFX"), pos, Quaternion.identity).gameObject;
            previewRenderers = preview.GetComponentsInChildren<SpriteRenderer>();

            AudioController.Instance.PlaySound2D("ui_confirm");
        }
    }
}