using UnityEngine;

namespace Common
{
    public class BackgroundParallaxEffect : MonoBehaviour
    {
        public Camera mainCamera;
        public float amountOfParallax;

        private float _startingPos;

        private void Start()
        {
            _startingPos = transform.position.x;
        }

        private void Update()
        {
            var position = mainCamera.transform.position;
            var distance = position.x * amountOfParallax;
            var newPosition = new Vector3(_startingPos + distance, transform.position.y, transform.position.z);
            transform.position = newPosition;
        }
    
    }
}
