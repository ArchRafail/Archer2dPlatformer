using System;
using UnityEngine;

namespace Common
{
    public class MoveForward : MonoBehaviour
    {
        public float speed;

        private void Update()
        {
            transform.Translate(Vector2.right * (speed * Time.deltaTime * Math.Sign(transform.localScale.x)));
        }
    }
}
