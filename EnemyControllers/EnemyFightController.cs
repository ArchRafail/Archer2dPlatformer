using System;
using System.Collections;
using PlayerControllers;
using UnityEngine;

namespace EnemyControllers
{
    public enum ZombieType
    {
        Melee,
        Mage
    }

    public class EnemyFightController : MonoBehaviour
    {
        public ZombieType zombieType;
        public GameObject sword;
        public GameObject fireball;
        public GameObject fireballPrefab;
        public Transform firePoint;
        public float maximumFightStartDistance;
        public float contactFightDistance;
        public float runSpeed;
        public float hitTime;
        public float calmDownTime;
        public AudioSource swordAudioSource;
        public AudioSource fireballAudioSource;
        
        private const float HeightFightClearance = 0.5f;
        private const float StrikeDistance = 1.35f;
        private const float CastFireballTime = 0.9f;
        private const float MageRetreatDistance = 2f; 
        
        private EnemyAttackController _enemyAttackController;
        private EnemyPatrolController _enemyPatrolController;
        private EnemyHealthController _enemyHealthController;
        private GameObject _player;
        private Animator _animator;
        private bool _swordPicked;
        private bool _isFighting;
        private bool _onPosition;
        private Vector2 _escapePoint;
        private bool _escapePointDefined;
        private float _calmDownTimer;

        private static readonly int Hit = Animator.StringToHash("hit");
        private static readonly int Cast = Animator.StringToHash("cast");

        private void Start()
        {
            _enemyAttackController = GetComponent<EnemyAttackController>();
            _enemyPatrolController = GetComponent<EnemyPatrolController>();
            _enemyHealthController = GetComponent<EnemyHealthController>();
            _animator = transform.GetComponent<Animator>();
            _player = GameObject.Find("Player").gameObject;
            sword.GetComponent<Renderer>().enabled = false;
            fireball.SetActive(false);
            fireball.GetComponent<Renderer>().enabled = false;
            _swordPicked = false;
            _onPosition = false;
            _escapePointDefined = false;
            _calmDownTimer = 0.0f;
        }

        private void Update()
        {
            var canDoSomething = !_enemyHealthController.Destroyed;
            
            if (canDoSomething && _enemyAttackController.Attacked && _player != null)
            {
                var playerPosition = _player.transform.position;
                if (!_onPosition && !_enemyAttackController.ReturnToPatrol)
                {
                    if (FightStartDistanceReached(playerPosition))
                    {
                        RunToFightPoint(playerPosition, runSpeed);
                    }
                    else
                    {
                        if (zombieType == ZombieType.Melee)
                        {
                            RunFromPlayer(playerPosition, maximumFightStartDistance / 2);
                        }
                        if (zombieType == ZombieType.Mage)
                        {
                            RunFromPlayer(playerPosition, MageRetreatDistance);
                        }
                    }

                    if (zombieType == ZombieType.Melee && !_swordPicked)
                    {
                        Invoke(nameof(PickUpSword), 0.5f);
                    }

                }

                if (_onPosition && !ContactFightDistanceReached(playerPosition) && !_escapePointDefined)
                {
                    _onPosition = false;
                }

                if (_onPosition && ContactFightDistanceReached(playerPosition))
                {
                    StartCoroutine(Fight(playerPosition));
                }

                if (_onPosition && _escapePointDefined)
                {
                    _calmDownTimer += Time.deltaTime;
                    if (FightStartDistanceReached(playerPosition))
                    {
                        _onPosition = false;
                        _calmDownTimer = 0.0f;
                        _escapePointDefined = false;
                        RunToFightPoint(playerPosition, runSpeed);
                    }
                }
            }
            
            if (canDoSomething && _enemyAttackController.Attacked && _player == null)
            {
                _calmDownTimer += Time.deltaTime;
            }
            
            if (canDoSomething && _enemyAttackController.Attacked && _calmDownTimer > calmDownTime)
            {
                _onPosition = false;
                _enemyAttackController.ReturnToPatrol = true;
                if (zombieType == ZombieType.Melee && _swordPicked)
                {
                    sword.GetComponent<Renderer>().enabled = false;
                    _swordPicked = false;
                }
                _enemyPatrolController.ResetPoints = true;
                RunToFightPoint(_enemyPatrolController.FirstPatrolPoint, runSpeed/2);
                if (_onPosition)
                {
                    _enemyAttackController.Attacked = false;
                    _enemyAttackController.ReturnToPatrol = false;
                    _calmDownTimer = 0.0f;
                    _onPosition = false;
                    _escapePointDefined = false;
                }
            }
        }

        private void RunToFightPoint(Vector2 playerPosition, float speedRun) 
        {
            RotateToPlayer(playerPosition.x);
            _enemyPatrolController.MovingAnimationParameter = true;
            
            if (!ContactFightDistanceReached(playerPosition))
            {
                transform.Translate(speedRun * Time.deltaTime * Vector2.right * Math.Sign(transform.localScale.x));
            }
            else
            {
                _enemyPatrolController.MovingAnimationParameter = false;
                _onPosition = true;
            }
        }
    
        private void RunFromPlayer(Vector2 playerPosition, float distanceRunning)
        {
            if (zombieType == ZombieType.Mage)
            {
                if (IsInvoking(nameof(Fire)))
                {
                    CancelInvoke(nameof(Fire));
                    _animator.ResetTrigger(Cast);
                }

                if (fireball.GetComponent<Renderer>().enabled)
                {
                    fireball.GetComponent<Renderer>().enabled = false;
                }

                if (fireball.activeSelf)
                {
                    fireball.SetActive(false);
                }
            }

            RotateTowardsPlayer(playerPosition.x);
            
            if (!_escapePointDefined)
            {
                _escapePoint =
                    new Vector2(
                        Math.Sign(transform.localScale.x) > 0 ?
                            transform.position.x + distanceRunning :
                            transform.position.x - distanceRunning,
                        transform.position.y
                    );
                _escapePointDefined = true;
            }

            _enemyPatrolController.MovingAnimationParameter = true;
            if (Math.Abs(_escapePoint.x - transform.position.x) > 0.1)
            {
                transform.Translate(runSpeed * Time.deltaTime * Vector2.right * Math.Sign(transform.localScale.x));
            }
            else
            {
                _enemyPatrolController.MovingAnimationParameter = false;
                RotateToPlayer(playerPosition.x);
                _onPosition = true;
            }
        }

        private void PickUpSword()
        {
            sword.GetComponent<Renderer>().enabled = true;
            _swordPicked = true;
        }
        
        private IEnumerator Fight(Vector2 playerPosition)
        {
            if (_isFighting) yield break;
            _isFighting = true;

            if (zombieType == ZombieType.Melee)
            {
                _animator.SetTrigger(Hit);
            }
            if (zombieType == ZombieType.Mage)
            {
                fireball.GetComponent<Renderer>().enabled = true;
                _animator.SetTrigger(Cast);
                Invoke(nameof(Fire), CastFireballTime);
            }
            yield return new WaitForSeconds(hitTime);
            if (zombieType == ZombieType.Mage)
            {
                RunFromPlayer(playerPosition, 1.0f);
            }
            
            _isFighting = false;
        }

        private void HitPlayer()
        {
            float distance = Vector2.Distance(_player.transform.position, transform.position);
            if (distance < StrikeDistance)
            {
                _player.GetComponent<PlayerHealthController>().IsHitten = true;
            }
        }

        private bool FightStartDistanceReached(Vector2 playerPosition)
        {
            return Math.Abs(playerPosition.x - transform.position.x) < maximumFightStartDistance &&
                   Math.Abs(playerPosition.y - transform.position.y) < HeightFightClearance;
        }

        private bool ContactFightDistanceReached(Vector2 playerPosition)
        {
            return Math.Abs(playerPosition.x - transform.position.x) <= contactFightDistance &&
                   Math.Abs(playerPosition.y - transform.position.y) < 0.5f;
        }

        private void RotateToPlayer(float playerPositionX)
        {
            transform.localScale = new Vector2(
                Math.Sign(playerPositionX - transform.position.x) * Math.Abs(transform.localScale.x),
                transform.localScale.y);
        }

        private void RotateTowardsPlayer(float playerPositionX)
        {
            transform.localScale = new Vector2(
                Math.Sign(playerPositionX - transform.position.x) * Math.Abs(transform.localScale.x) * -1,
                transform.localScale.y);
        }
        
        private void Fire()
        {
            var fireballProjectile = Instantiate(
                fireballPrefab,
                firePoint.position,
                Quaternion.Euler(0, 0, 0)
                );
            var fireballProjectileScale = fireballProjectile.transform.localScale;
            fireballProjectile.transform.localScale = new Vector3(
                transform.localScale.x > 0 ? fireballProjectileScale.x : fireballProjectileScale.x * -1,
                fireballProjectileScale.y,
                fireballProjectileScale.z
                );
            fireball.GetComponent<Renderer>().enabled = false;
            fireball.SetActive(false);
        }

        private void ShowFireball()
        {
            fireball.SetActive(true);
        }

        private void SwordHitAudioPlay()
        {
            swordAudioSource.Play();
        }

        private void FireballCastAudioPlay()
        {
            fireballAudioSource.Play();
        }

    }
}
