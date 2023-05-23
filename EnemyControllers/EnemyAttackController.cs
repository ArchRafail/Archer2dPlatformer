using UnityEngine;

namespace EnemyControllers
{
    public class EnemyAttackController : MonoBehaviour
    {
        private EnemyPatrolController _enemyPatrolController;

        public bool Attacked { get; set; }
        public bool ReturnToPatrol { get; set; }

        private void Start()
        {
            _enemyPatrolController = GetComponent<EnemyPatrolController>();
            
            Attacked = false;
            ReturnToPatrol = false;
        }

        private void Update()
        {
            if (Attacked)
            {
                if (_enemyPatrolController.Moving)
                {
                    _enemyPatrolController.StopMove = true;
                }
            }
            else
            {
                _enemyPatrolController.StopMove = false;
            }
        }
    }
}
