using UnityEngine;

namespace Common
{
    public class DestroyingObjectByTime : MonoBehaviour
    {
        public float time;

        private void Start()
        {
            Destroy(gameObject, time);
        }

    }
}
