using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public class StarsController : MonoBehaviour
    {
        public GameObject gameGuide;
        public GameObject greyStarPrefab;
        public GameObject starWrapperPrefab;
        public float delayBetweenStars;

        public bool ShowStars { get; set; }

        private const int MaximumStarsQuantity = 3;
        private const float DelayForGreyStarDisappear = 0.8f;

        private Animator _firstStarAnimator;
        private Animator _secondStarAnimator;
        private Animator _thirdStarAnimator;
        private GameGuideController _gameGuideController;
        private List<GameObject> _greyStars;
        private List<GameObject> _stars;
        private AudioSource _audioSource;
        private int _starsQuantity;
        private bool _greyStarShowed;
        private int _indexOfStar;
        private bool _isShowing;
        private static readonly int Move = Animator.StringToHash("move");

        private void Start()
        {
            _gameGuideController = gameGuide.GetComponent<GameGuideController>();

            _greyStars = new List<GameObject>();
            _stars = new List<GameObject>();
            
            for (int i = 0; i < MaximumStarsQuantity; i++)
            {
                var greyStar = Instantiate(greyStarPrefab, new Vector3(2 * i, 0, 0), Quaternion.identity,
                    gameObject.transform);
                var grayStarPosition = greyStar.transform.position;
                greyStar.transform.position = new Vector3(
                    grayStarPosition.x + transform.position.x,
                    grayStarPosition.y + transform.position.y,
                    grayStarPosition.z + transform.position.z);
                greyStar.SetActive(false);
                _greyStars.Add(greyStar);
                
                var starWrapper = Instantiate(starWrapperPrefab, new Vector3(2 * i, 0, 0), Quaternion.identity,
                    gameObject.transform);
                var starWrapperPosition = starWrapper.transform.position;
                starWrapper.transform.position = new Vector3(
                    starWrapperPosition.x + transform.position.x,
                    starWrapperPosition.y + transform.position.y,
                    starWrapperPosition.z + transform.position.z
                );
                var starIcon = starWrapper.transform.Find("StarIcon").gameObject;
                var starIconPosition = starIcon.transform.position;
                starIcon.transform.position = new Vector3(starIconPosition.x - starWrapperPosition.x, starIconPosition.y, starIconPosition.z);
                starWrapper.SetActive(false);
                _stars.Add(starWrapper);
            }

            _firstStarAnimator = _stars[0].transform.Find("StarIcon").gameObject.GetComponent<Animator>();
            _secondStarAnimator = _stars[1].transform.Find("StarIcon").gameObject.GetComponent<Animator>();
            _thirdStarAnimator = _stars[2].transform.Find("StarIcon").gameObject.GetComponent<Animator>();
           
            _starsQuantity = 0;
            _greyStarShowed = false;
            _indexOfStar = 0;
            ShowStars = false;

            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (ShowStars)
            {
                _starsQuantity = _gameGuideController.StarsQuantity;
                if (!_greyStarShowed)
                {
                    foreach (var greyStar in _greyStars)
                    {
                        greyStar.SetActive(true);
                    }
                    _greyStarShowed = true;
                }
                if (_starsQuantity != 0 && _indexOfStar < _starsQuantity)
                {
                    StartCoroutine(ShowStar(_indexOfStar));
                }
                if (_indexOfStar >= _starsQuantity)
                {
                    ShowStars = false;
                }
            }
        }

        private IEnumerator ShowStar(int indexOfStar)
        {
            if (_isShowing) yield break;
            _isShowing = true;
            
            _audioSource.Play();
            _stars[indexOfStar].SetActive(true);
            switch (_indexOfStar)
            {
                case 0:
                    _firstStarAnimator.SetTrigger(Move);
                    break;
                case 1:
                    _secondStarAnimator.SetTrigger(Move);
                    break;
                case 2:
                    _thirdStarAnimator.SetTrigger(Move);
                    break;
            }
            
            yield return new WaitForSeconds(DelayForGreyStarDisappear);
            _greyStars[indexOfStar].SetActive(false);
            
            yield return new WaitForSeconds(delayBetweenStars);
            _indexOfStar += 1;
            _isShowing = false;
        }
    }
}
