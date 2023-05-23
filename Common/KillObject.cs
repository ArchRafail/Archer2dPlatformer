using UnityEngine;

namespace Common
{
    public class KillObject : MonoBehaviour
    {
        public bool kill;
        public float killTimer;

        private bool _isKilling;
    
        private void Start()
        {
            _isKilling = false;
        }

        private void Update()
        {
            #if UNITY_EDITOR
                if (kill && !_isKilling)
                {
                    Destroy(gameObject, killTimer);
                }
            #endif
        }
    }
}
