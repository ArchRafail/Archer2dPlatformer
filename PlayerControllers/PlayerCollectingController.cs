using UnityEngine;

namespace PlayerControllers
{
    public class PlayerCollectingController : MonoBehaviour
    {
        public int takeCoinsMode;
        public bool forceTakeCoins;
            
        public int Coins { get; private set; }

        private PlayerHealthController _playerHealthController;
        
        private void Start()
        {
            Coins = 0;
            _playerHealthController = GetComponent<PlayerHealthController>();
        }

        private void Update()
        {
            #if UNITY_EDITOR
                if (forceTakeCoins)
                {
                    Coins += takeCoinsMode;
                    forceTakeCoins = false;
                }
            #endif
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Coin"))
            {
                Coins += 1;
                Destroy(other.gameObject);
            }
            if (other.gameObject.CompareTag("Diamond"))
            {
                Coins += 10;
                Destroy(other.gameObject);
            }
            if (other.gameObject.CompareTag("Life"))
            {
                if (_playerHealthController.HitBody > 0)
                {
                    _playerHealthController.HitBody--;
                }
                else
                {
                    Coins += 2;
                }
                Destroy(other.gameObject);
            }
        }
    }
}
