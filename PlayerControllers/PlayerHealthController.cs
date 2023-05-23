using UnityEngine;

namespace PlayerControllers
{
    public class PlayerHealthController : MonoBehaviour
    {
        public int maxHealth;
        public GameObject defaultFace;
        public GameObject painFace;
        public GameObject deadFace;
        public float timeToDestroyPlayer;
        public float delayBeforeDead;
        public bool setIsHittenMode;

        public int HitBody { get; set; }
        public bool IsHitten { get; set; }

        private Animator _animator;
        private PlayerFightController _playerFightController;
        private bool _isChangingFaces;
        private bool _isDestroing;
        private static readonly int Dead = Animator.StringToHash("dead");
        private static readonly int Hitten = Animator.StringToHash("hitten");

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _playerFightController = GetComponent<PlayerFightController>();
            defaultFace.GetComponent<Renderer>().enabled = true;
            painFace.GetComponent<Renderer>().enabled = false;
            deadFace.GetComponent<Renderer>().enabled = false;
            painFace.SetActive(false);
            deadFace.SetActive(false);
            HitBody = 0;
            IsHitten = false;
            _isDestroing = false;
        }

        private void Update()
        {
            #if UNITY_EDITOR
                if (setIsHittenMode)
                {
                    IsHitten = true;
                    setIsHittenMode = false;
                }
            #endif

            if (HitBody >= maxHealth && !_isDestroing)
            {
                Invoke(nameof(DestroyPlayer), delayBeforeDead);
                _isDestroing = true;
            }

            if (IsHitten)
            {
                if (HitBody < maxHealth - 1)
                {
                    if (_playerFightController.IsShooting)
                    {
                        _playerFightController.StopFire = true;
                    }
                    _animator.SetTrigger(Hitten);
                }
                HitBody++;
                IsHitten = false;
            }
        }

        private void DestroyPlayer()
        {
            _animator.SetTrigger(Dead);
            Destroy(gameObject, timeToDestroyPlayer);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("ZombieFireball"))
            {
                IsHitten = true;
                Destroy(other.gameObject);
            }
        }
        
        private void ShowFaces()
        {
            painFace.SetActive(true);
            deadFace.SetActive(true);
        }

        private void HideFacesDuringHitten()
        {
            painFace.SetActive(false);
            deadFace.SetActive(false);
        }
        
        private void HideFacesDuringDeath()
        {
            painFace.SetActive(false);
            defaultFace.SetActive(false);
        }
    }
}
