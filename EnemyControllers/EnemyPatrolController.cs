using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

namespace EnemyControllers
{
    public class EnemyPatrolController : MonoBehaviour
    {
        public float halfPatrolDistanceX;
        public float travelTimeToPoint;
        public float minPointWaitingTime;
        public float maxPointWaitingTime;
        public Transform castFrontPoint;
        public Transform castBackPoint;
        public float maximumFrontDetectingDistance;
        public float maximumBackDetectingDistance;
        

        public bool Moving { get; private set; }
        public bool StopMove { get; set; }
        public bool ResetPoints { get; set; }
        public bool MovingAnimationParameter { get; set; }
        public Vector2 FirstPatrolPoint { get; private set; }

        private EnemyAttackController _enemyAttackController;
        private EnemyHealthController _enemyHealthController;
        private Animator _animator;
        private Coroutine _moveCoroutine;
        private Vector2[] _movementPoints;
        private bool[] _visitedPoints;
        private Random _random;
        private bool _resetVisitedPoints;
        private static readonly int IsMoving = Animator.StringToHash("isMoving");

        private void Start()
        {
            _enemyAttackController = GetComponent<EnemyAttackController>();
            _enemyHealthController = GetComponent<EnemyHealthController>();

            _random = new Random();
            _animator = transform.GetComponent<Animator>();
            var myPosition = transform.position;
            halfPatrolDistanceX = halfPatrolDistanceX == 0 ? 0.5f : halfPatrolDistanceX;
            _movementPoints = new[]
            {
                new Vector2(myPosition.x - halfPatrolDistanceX, myPosition.y),
                new Vector2(myPosition.x, myPosition.y),
                new Vector2(myPosition.x + halfPatrolDistanceX, myPosition.y),
                new Vector2(myPosition.x, myPosition.y)
            };
            FirstPatrolPoint = _movementPoints[1];
            _visitedPoints = new bool[4];
            MovingAnimationParameter = false;
            StopMove = false;
            ResetPoints = false;
            maximumBackDetectingDistance *= -1;
        }

        private void Update()
        {
            CheckState();
            
            var indexOfMovement = -1;

            if (!_enemyAttackController.Attacked && !_enemyAttackController.ReturnToPatrol && !_enemyHealthController.Destroyed)
            {
                var qtyOfVisitedPoints = 0;
                foreach (var visitedPoint in _visitedPoints)
                {
                    if (!visitedPoint)
                    {
                        indexOfMovement = Array.IndexOf(_visitedPoints, visitedPoint);
                        break;
                    }
                    qtyOfVisitedPoints++;
                }
                if (qtyOfVisitedPoints == _visitedPoints.Length)
                {
                    indexOfMovement = 0;
                    for (int i = 0; i < _visitedPoints.Length; i++)
                    {
                        _visitedPoints[i] = false;
                    }
                }
                _moveCoroutine = StartCoroutine(Move(indexOfMovement));

                if (DetectPlayer(castFrontPoint, maximumFrontDetectingDistance) ||
                    DetectPlayer(castBackPoint, maximumBackDetectingDistance))
                {
                    _enemyAttackController.Attacked = true;
                }
            }
            
            if (StopMove)
            {
                if (Moving)
                {
                    Moving = false;
                    MovingAnimationParameter = false;
                    if (_moveCoroutine != null)
                    {
                        StopCoroutine(_moveCoroutine);
                    }
                }
            }
            if (ResetPoints)
            {
                for (int i = 0; i < _visitedPoints.Length; i++)
                {
                    _visitedPoints[i] = false;
                }
                ResetPoints = false;
            }
        }

        private void CheckState()
        {
            _animator.SetBool(IsMoving, MovingAnimationParameter);
        }

        private IEnumerator Move(int indexOfMovement)
        {
            if(Moving) yield break;
            Moving = true;
            MovingAnimationParameter = true;

            var timeToReachTargetElapsed = 0.0f;
            var timeToReachTargetDuration = travelTimeToPoint;
            Vector2 startPosition = transform.position;
            var targetPosition = _movementPoints[indexOfMovement];
        
            transform.localScale = new Vector2(
                Math.Sign(targetPosition.x - startPosition.x) * Math.Abs(transform.localScale.x),
                transform.localScale.y);
        
            while (timeToReachTargetElapsed < timeToReachTargetDuration)
            {
                if (!Moving)
                {
                    yield break;
                }
                transform.position = Vector2.Lerp(startPosition, targetPosition, timeToReachTargetElapsed / timeToReachTargetDuration);
                timeToReachTargetElapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;

            _visitedPoints[indexOfMovement] = true;

            if (indexOfMovement is 0 or 2)
            {
                MovingAnimationParameter = false;
                var waitTime = _random.NextDouble() * (maxPointWaitingTime - minPointWaitingTime) + minPointWaitingTime;
                yield return new WaitForSeconds((float)waitTime);
            }
            Moving = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_enemyHealthController.Destroyed && other.gameObject.CompareTag("PlayerArrow"))
            {
                _enemyAttackController.Attacked = true;
                _enemyHealthController.IsHitten = true;
                Destroy(other.gameObject);
            }
        }

        private bool DetectPlayer(Transform castPoint, float distance)
        {
            var value = false;
            var castDistance = distance * Math.Sign(transform.localScale.x);
    
            Vector2 endPointPosition = castPoint.position + Vector3.right * castDistance;
            RaycastHit2D hit = Physics2D.Linecast(castPoint.position, endPointPosition, 1 << LayerMask.NameToLayer("Action"));
    
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    value = true;
                }
            }
            Debug.DrawLine(castPoint.position, endPointPosition, Color.blue);
            return value;
        }
    }
}
