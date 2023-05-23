using UnityEngine;

namespace EnemyControllers
{
    public class EnemyHealthController : MonoBehaviour
    {
        public int maxHealth;
        public float timeToDestroyEnemy;
        public float delayBeforeDead;
        public GameObject defaultFace;
        public GameObject painFace;
        public GameObject deadFace;

        public bool Destroyed { get; private set; }
        public bool IsHitten { get; set; }

        private Animator _animator;
        private int _hitsNumber;
        private static readonly int Hitten = Animator.StringToHash("hitten");
        private static readonly int Dead = Animator.StringToHash("dead");

        private void Start()
        {
            _animator = GetComponent<Animator>();
            Destroyed = false;
            _hitsNumber = 0;
            defaultFace.GetComponent<Renderer>().enabled = true;
            painFace.GetComponent<Renderer>().enabled = false;
            deadFace.GetComponent<Renderer>().enabled = false;
            painFace.SetActive(false);
            deadFace.SetActive(false);
        }

        private void Update()
        {
            if (IsHitten)
            {
                if (_hitsNumber < maxHealth - 1)
                {
                    _animator.SetTrigger(Hitten);
                }
                _hitsNumber += 1;
                IsHitten = false;
            }

            if (!Destroyed && _hitsNumber >= maxHealth)
            {
                Destroyed = true;
                Invoke(nameof(DestroyEnemy), delayBeforeDead);
            }
        }

        private void DestroyEnemy()
        {
            _animator.SetTrigger(Dead);
            Destroy(gameObject, timeToDestroyEnemy);
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
