using UnityEngine;

namespace Common
{
    public class DestroyingFallObject : MonoBehaviour
    {
        public float heightOfObject;

        private Vector2 _min;
        
        private void Start()
        {
            _min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
            _min.y -= heightOfObject;
        }

        private void Update()
        {
            if (transform.position.y < _min.y)
            {
                Destroy(gameObject);
            }
        }
    }
}
