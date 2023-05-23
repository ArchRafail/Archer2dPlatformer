using System;
using Common;
using UnityEngine;

namespace PlayerControllers
{
    public class PlayerFightController : MonoBehaviour
    {
        public GameObject gameGuide;
        public GameObject bow;
        public GameObject arrow;
        public GameObject arrowPrefab;
        public float fireRate;

        public bool IsShooting { get; private set; }
        public bool StopFire { get; set; }

        private const float DelayOnFirstShoot = 0.5f;

        private GameGuideController _gameGuideController;
        private Animator _animator;
        private Transform _firePointTransform;
        private AudioSource _audioSource;
        private float _charge;
        private float _fireDelay;
        private float _fightAnimationClipTime;
        private static readonly int Shoot = Animator.StringToHash("shoot");
    
        private void Start()
        {
            _gameGuideController = gameGuide.GetComponent<GameGuideController>();
            _animator = GetComponent<Animator>();
            bow.GetComponent<Renderer>().enabled = false;
            arrow.GetComponent<Renderer>().enabled = false;
            _audioSource = GetComponent<AudioSource>();
            
            _fireDelay = 60 / fireRate;
            _charge = _fireDelay - DelayOnFirstShoot;
        
            _firePointTransform = transform.Find("FirePoint").gameObject.transform;
        
            var animationClips = _animator.runtimeAnimatorController.animationClips;
            foreach (var animationClip in animationClips)
            {
                if (animationClip.name == "character_shoot")
                {
                    _fightAnimationClipTime = animationClip.length;
                    break;
                }
            }
            StopFire = false;
        }

        private void Update()
        {
            if (StopFire)
            {
                _animator.ResetTrigger(Shoot);
                if (IsInvoking(nameof(Fire)))
                {
                    CancelInvoke(nameof(Fire));
                }
                bow.GetComponent<Renderer>().enabled = false;
                arrow.GetComponent<Renderer>().enabled = false;
                _charge = 0.0f;
                IsShooting = false;
                StopFire = false;
            }

            _charge += Time.deltaTime;
        
            if (!_gameGuideController.EndGame && Input.GetButtonDown("Fire1") && _charge > _fireDelay)
            {
                IsShooting = true;
                bow.GetComponent<Renderer>().enabled = true;
                Invoke(nameof(ArrowAppear), 0.15f);
                _animator.SetTrigger(Shoot);
                Invoke(nameof(Fire), _fightAnimationClipTime);
                _charge = 0.0f;
            }
        }
    
        protected void Fire()
        {
            Instantiate(arrowPrefab, _firePointTransform.position, Quaternion.Euler(0, 0, Math.Sign(transform.localScale.x) > 0 ? 0 : 180));
            bow.GetComponent<Renderer>().enabled = false;
            arrow.GetComponent<Renderer>().enabled = false;
            IsShooting = false;
        }
    
        private void ArrowAppear()
        {
            arrow.GetComponent<Renderer>().enabled = true;
        }

        private void AudioPlay()
        {
            _audioSource.Play();
        }
    }
}
