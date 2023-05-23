using System.Collections.Generic;
using PlayerControllers;
using UnityEngine;

namespace UiControllers
{
    public class HpPlateController : MonoBehaviour
    {
        public GameObject character;
        public GameObject redLifeIconPrefab;
        public GameObject greyLifeIconPrefab;

        private const float StepBetweenIcons = 0.4f;
    
        private PlayerHealthController _playerHealthController;
        private int _maxPlayerHealthPoints;
        private List<GameObject> _redHearts;
        private List<GameObject> _greyHearts;
        private int _remainHealthPoints;
    
        void Start()
        {
            _playerHealthController = character.GetComponent<PlayerHealthController>();
            _maxPlayerHealthPoints = _playerHealthController.maxHealth;
            _redHearts = new List<GameObject>();
            _greyHearts = new List<GameObject>();

            for (int i = 0; i < _maxPlayerHealthPoints; i++)
            {
                var redHeart = Instantiate(
                    redLifeIconPrefab,
                    new Vector3(StepBetweenIcons * i, 0, 0),
                    Quaternion.Euler(0, 0, 0),
                    gameObject.transform
                );
                redHeart.transform.position = new Vector2(redHeart.transform.position.x + transform.position.x, redHeart.transform.position.y + transform.position.y);
                _redHearts.Add(redHeart);
                var greyHeart = Instantiate(
                    greyLifeIconPrefab,
                    new Vector3(redHeart.transform.position.x, redHeart.transform.position.y, 0),
                    Quaternion.Euler(0, 0, 0),
                    gameObject.transform
                );
                greyHeart.SetActive(false);
                _greyHearts.Add(greyHeart);
            }
        }

        void Update()
        {
            if (character == null) return;

            for (int i = 0; i < _redHearts.Count; i++)
            {
                _redHearts[i].SetActive(true);
                _greyHearts[i].SetActive(false);
            }
        
            _remainHealthPoints = _playerHealthController.HitBody;
            for (int i = _redHearts.Count - 1; i > _redHearts.Count - 1 - _remainHealthPoints; i--)
            {
                if (_redHearts[i].activeSelf)
                {
                    _redHearts[i].SetActive(false);
                }

                if (!_greyHearts[i].activeSelf)
                {
                    _greyHearts[i].SetActive(true);
                }
            }
        }
    }
}
