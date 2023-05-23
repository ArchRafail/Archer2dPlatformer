using PlayerControllers;
using TMPro;
using UnityEngine;

namespace UiControllers
{
    public class CoinsShowController : MonoBehaviour
    {
        public TMP_Text coinShowText;
        public GameObject character;

        private PlayerCollectingController _playerCollectingController;
    
        private void Start()
        {
            _playerCollectingController = character.GetComponent<PlayerCollectingController>();
        }

        private void Update()
        {
            coinShowText.text = _playerCollectingController.Coins.ToString();
        }
    }
}
