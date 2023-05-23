using System.Collections;
using EnemyControllers;
using PlayerControllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common
{
    public class GameGuideController : MonoBehaviour
    {
        public GameObject player;
        public GameObject starsPlate;
        public GameObject uiWinText;
        public GameObject uiLosingText;
        public GameObject uiButtonSet;
        public int coinsQuantityFor3Stars;
        public int minCoinsQuantityFor2Stars;
        public int minCoinsQuantityFor1Star;
        public bool endGameMode;
        public AudioSource gameAudioSource;
        public AudioSource winAudioSource;
        public AudioSource loseAudioSource;

        public bool EndGame { get; private set; }
        public int StarsQuantity { get; private set; }
        
        private const float DelayForUiShowing = 0.5f;
        private const float DelayByStarShowing = 1.8f;
        private const float EndGameXCoordinate = 88.8f;

        private PlayerCollectingController _collectingController;
        private StarsController _starsController;
        private bool _isShowing;
        private bool _showWin;
        private bool _showLose;
        
        private void Start()
        {
            _starsController = starsPlate.GetComponent<StarsController>();
            EndGame = false;
            
            uiWinText.SetActive(false);
            uiLosingText.SetActive(false);
            uiButtonSet.SetActive(false);

            _collectingController = player.GetComponent<PlayerCollectingController>();
            StarsQuantity = 0;
            _showWin = false;
            _showLose = false;
        }

        private void Update()
        {
            TakeScores();
            
            #if UNITY_EDITOR
                if (!EndGame && endGameMode)
                {
                    EndGame = true;
                    _showWin = true;
                }
            #endif

            if (_showWin)
            {
                StartCoroutine(EndGameUiShow(uiWinText));
            }

            if (_showLose)
            {
                StartCoroutine(EndGameUiShow(uiLosingText));
            }
             
            if (!EndGame && player != null && player.transform.position.x >= EndGameXCoordinate)
            {
                EndGame = true;
                gameAudioSource.Stop();
                RemoveEnemies();
                _showWin = true;
            }
            
            if (!EndGame && player == null)
            {
                EndGame = true;
                gameAudioSource.Stop();
                _showLose = true;
            }
            
            if (Input.GetButton("Cancel"))
            {
                EndGame = true;
                Invoke(nameof(ForceBackToMenu), 0.3f);
            }
        }
        
        private IEnumerator EndGameUiShow(GameObject uiText)
        {
            if (_isShowing) yield break;
            _isShowing = true;
            
            yield return new WaitForSeconds(DelayForUiShowing);
            
            uiText.SetActive(true);
            _starsController.ShowStars = true;
            uiButtonSet.SetActive(true);

            if (_showWin)
            {
                Invoke(nameof(WinAudioPlaying), StarsQuantity * DelayByStarShowing);
                _showWin = false;
            }
            
            if (_showLose)
            {
                loseAudioSource.Play();
                _showLose = false;
            }
            _isShowing = true;
        }

        private void TakeScores()
        {
            if (_collectingController.Coins > minCoinsQuantityFor1Star && _collectingController.Coins < minCoinsQuantityFor2Stars)
            {
                StarsQuantity = 1;
            }
            
            if (_collectingController.Coins >= minCoinsQuantityFor2Stars && _collectingController.Coins < coinsQuantityFor3Stars)
            {
                StarsQuantity = 2;
            }

            if (_collectingController.Coins >= coinsQuantityFor3Stars)
            {
                StarsQuantity = 3;
            }
        }

        private void ForceBackToMenu()
        {
            SceneManager.LoadScene(0);
        }

        private void RemoveEnemies()
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0) return;
            foreach (var enemy in enemies)
            {
                var enemyPatrolController = enemy.GetComponent<EnemyPatrolController>();
                enemyPatrolController.maximumFrontDetectingDistance = 0;
                enemyPatrolController.maximumBackDetectingDistance = 0;
                var enemyAttackController = enemy.GetComponent<EnemyAttackController>();
                enemyAttackController.Attacked = false;
            }
        }

        private void WinAudioPlaying()
        {
            winAudioSource.Play();
        }
    }
}
