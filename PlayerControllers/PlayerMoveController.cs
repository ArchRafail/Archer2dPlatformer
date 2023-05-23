using System;
using Common;
using UnityEngine;

namespace PlayerControllers
{
    public class PlayerMoveController : MonoBehaviour
    {
        public GameObject gameGuide;
        public LayerMask groundLayer;
        public float maxRayLength = 0.4f;
        public float groundMoveForce = 600;
        public float airMoveForce = 50;
        public float groundVerticalMoveForce = 7;
        public float maximumVelocity;

        private GameGuideController _gameGuideController;
        private Rigidbody2D _rigidbody;
        private Animator _animator;
        private Vector2 _lastPosition;
        private bool _isGrounded;
        private Vector2 _lastGoodPosition;
        private static readonly int IsMoving = Animator.StringToHash("isMoving");
        private static readonly int IsJumping = Animator.StringToHash("isJumping");

        private void Start()
        {
            _gameGuideController = gameGuide.GetComponent<GameGuideController>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _lastPosition = transform.position;
            _isGrounded = true;
        }

        private void Update()
        {
            _isGrounded = CheckGrounding();
            if (_isGrounded)
            {
                _lastGoodPosition = transform.position;
            }
            if (!_gameGuideController.EndGame)
            {
                Move();
            }

            CorrectingVelocityX();
            CheckState();
            Debug.DrawRay(transform.position, Vector2.down * maxRayLength, Color.red);
    
            UpdateCameraPosition();
            _lastPosition = transform.position;

            if (Input.GetKeyDown(KeyCode.R))
            {
                transform.position = _lastGoodPosition;
                _rigidbody.velocity = new Vector2(0, 0);
            }
        }

        private void Move()
        {
            var horizontalInput = Input.GetAxisRaw("Horizontal");

            if (horizontalInput != 0)
            {
                if (_isGrounded)
                {
                    _rigidbody.AddForce(groundMoveForce * Time.deltaTime * horizontalInput * Vector2.right, ForceMode2D.Force);
                }
                else
                {
                    _rigidbody.AddForce(airMoveForce * Time.deltaTime * horizontalInput * Vector2.right, ForceMode2D.Force);
                }
                transform.localScale = new Vector2(
                    Math.Sign(horizontalInput) * Math.Abs(transform.localScale.x),
                    transform.localScale.y);
            }

            if (Input.GetButtonDown("Jump") && _isGrounded)
            {
                _rigidbody.AddForce(Vector2.up * groundVerticalMoveForce, ForceMode2D.Impulse);
            }
        }

        private void CheckState()
        {
            if (!_isGrounded)
            {
                _animator.SetBool(IsJumping, true);
                _animator.SetBool(IsMoving, false);
            }
            else
            {
                _animator.SetBool(IsJumping, false);
            }

            if (Math.Abs(_rigidbody.velocity.x) >= 0.25 && !_animator.GetBool(IsJumping))
            {
                _animator.SetBool(IsMoving, true);
            }
            else
            {
                _animator.SetBool(IsMoving, false);
            }
        }

        private bool CheckGrounding()
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.down, maxRayLength, groundLayer);
            return hit;
        }

        private void UpdateCameraPosition()
        {
            var positionDiffX = transform.position.x - _lastPosition.x;
            if (transform.position.x is > 0 and < 82 && positionDiffX != 0)
            {
                var cameraPosition = Camera.main.transform.position;
                Camera.main.transform.position =
                    new Vector3(
                        Mathf.Clamp(cameraPosition.x + positionDiffX, 0, 82),
                        cameraPosition.y,
                        cameraPosition.z
                    );
            }
        }

        private void CorrectingVelocityX()
        {
            var newVelocity = _rigidbody.velocity;
            newVelocity.x = Mathf.Clamp(_rigidbody.velocity.x, -maximumVelocity, maximumVelocity);
            _rigidbody.velocity = new Vector2(newVelocity.x, _rigidbody.velocity.y);
        }
    }
}
