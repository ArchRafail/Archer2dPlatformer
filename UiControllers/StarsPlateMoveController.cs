using UnityEngine;

namespace UiControllers
{
    public class StarsPlateMoveController : MonoBehaviour
    {
        public GameObject mainCamera;

        private const float LeftDisplacement = 2f;
        private const float PlacementHeight = 1.5f;

        private void Update()
        {
            transform.position = new Vector2(mainCamera.transform.position.x - LeftDisplacement, PlacementHeight);
        }
    }
}
