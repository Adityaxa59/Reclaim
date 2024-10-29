using UnityEngine;

namespace Alchemy.Scripts
{
    public class PotMovement : MonoBehaviour
    {
        [SerializeField] float maxY = 7;
        private Vector3 _rawMousePosition;
        private Vector3 _translatedPosition;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0)) //on drag
            {
                _rawMousePosition = Input.mousePosition;
                // ReSharper disable once PossibleNullReferenceException
                _translatedPosition = _camera.ScreenToWorldPoint(_rawMousePosition); //converts screen position of mouse to game world position
                
                if (_translatedPosition.x > maxY) // screen boundary right
                {
                    _translatedPosition.x = maxY;
                }
                else if (_translatedPosition.x < -maxY) // screen boundary left
                {
                    _translatedPosition.x = -maxY;
                }
                transform.position = new Vector3(_translatedPosition.x, transform.position.y, transform.position.z);// moves object. vector z becomes -10(pos of camera) which hides the object
            }
        }
    }
}