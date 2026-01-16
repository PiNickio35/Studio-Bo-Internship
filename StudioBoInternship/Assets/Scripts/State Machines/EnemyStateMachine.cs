using System.Collections;
using Base_Classes;
using UnityEngine;

namespace State_Machines
{
    public class EnemyStateMachine : MonoBehaviour
    {
        public BaseEnemy enemy;
    
        public enum TurnState
        {
            PROCESSING,
            CHOOSEACTION,
            WAITING,
            ACTION,
            DEAD
        }
    
        public TurnState currentTurnState;
    
        private float _currentCooldown;
        private float _maxCooldown = 10f;
    
        private Vector3 _startPosition;
        [SerializeField] private GameObject selector;

        private bool _actionStarted;
        public GameObject heroToAttack;
        private float _animationSpeed = 10f;
        
        private bool _isAlive = true;

        private void Start()
        {
            currentTurnState = TurnState.PROCESSING;
            selector.SetActive(false);
            _startPosition = transform.position;
        }

        private void Update()
        {
            switch (currentTurnState)
            {
                case (TurnState.PROCESSING):
                    UpdateProgress();
                    break;
                case (TurnState.CHOOSEACTION):
                    ChooseAction();
                    currentTurnState = TurnState.WAITING;
                    break;
                case (TurnState.WAITING):
                    break;
                case (TurnState.ACTION):
                    StartCoroutine(TimeForAction());
                    break;
                case (TurnState.DEAD):
                    if (!_isAlive) return;
                    this.gameObject.tag = "DeadEnemy";
                    BattleStateMachine.Instance.enemiesInBattle.Remove(this.gameObject);
                    selector.SetActive(false);
                    // TODO Make sure this works properly and doesn't go out of range.
                    if (BattleStateMachine.Instance.enemiesInBattle.Count > 0)
                    {
                        for (int i = 0; i < BattleStateMachine.Instance.performActionsList.Count; i++)
                        {
                            if (BattleStateMachine.Instance.performActionsList[i].AttackerObject == this.gameObject)
                            {
                                BattleStateMachine.Instance.performActionsList.Remove(BattleStateMachine.Instance.performActionsList[i]);
                            }

                            else if (BattleStateMachine.Instance.performActionsList[i].TargetObject == this.gameObject)
                            {
                                BattleStateMachine.Instance.performActionsList[i].TargetObject = BattleStateMachine.Instance.enemiesInBattle[Random.Range(0, BattleStateMachine.Instance.enemiesInBattle.Count)];
                            }
                        }
                    }
                    this.gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
                    _isAlive = false;
                    BattleStateMachine.Instance.EnemyButtons();
                    BattleStateMachine.Instance.battleState = BattleStateMachine.PerformAction.CHECKALIVE;
                    break;
            }
        }
    
        private void UpdateProgress()
        {
            _currentCooldown += Time.deltaTime;
            if (_currentCooldown >= _maxCooldown)
            {
                currentTurnState = TurnState.CHOOSEACTION;
            }
        }

        private void ChooseAction()
        {
            HandleTurn myAttack = new HandleTurn();
            myAttack.Attacker = enemy.actorName;
            myAttack.Type = "Enemy";
            myAttack.AttackerObject = this.gameObject;
            myAttack.TargetObject = BattleStateMachine.Instance.heroesInBattle[Random.Range(0, BattleStateMachine.Instance.heroesInBattle.Count)];
            myAttack.chosenAttack = enemy.actorAttacks[Random.Range(0, enemy.actorAttacks.Count)];
            Debug.Log(this.gameObject.name + " has chosen " + myAttack.chosenAttack.attackName + " and does " + myAttack.chosenAttack.attackDamage + " damage!");
            
            BattleStateMachine.Instance.CollectActions(myAttack);
        }

        private IEnumerator TimeForAction()
        {
            if (_actionStarted)
            {
                yield break;
            }

            _actionStarted = true;
        
            Vector3 heroPosition = new Vector3(heroToAttack.transform.position.x + 1.5f, heroToAttack.transform.position.y,
                heroToAttack.transform.position.z);
            while (MoveTowardsTarget(heroPosition)) yield return null;

            yield return new WaitForSeconds(0.5f);
        
            DoDamage();

            while (MoveTowardsTarget(_startPosition)) yield return null;

            BattleStateMachine.Instance.performActionsList.RemoveAt(0);
            if (BattleStateMachine.Instance.battleState == BattleStateMachine.PerformAction.LOSE) yield break;
            BattleStateMachine.Instance.battleState = BattleStateMachine.PerformAction.WAIT;
            _actionStarted = false;
        
            _currentCooldown = 0f;
            currentTurnState = TurnState.PROCESSING;
        }

        private bool MoveTowardsTarget(Vector3 target)
        {
            return target != (transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * _animationSpeed));
        }

        private void DoDamage()
        {
            float calcDamage = enemy.currentATK +
                               BattleStateMachine.Instance.performActionsList[0].chosenAttack.attackDamage;
            heroToAttack.GetComponent<HeroStateMachine>().TakeDamage(calcDamage);
        }

        public void TakeDamage(float damage)
        {
            enemy.currentHP -= damage;
            if (enemy.currentHP <= 0)
            {
                enemy.currentHP = 0;
                currentTurnState = TurnState.DEAD;
            }
        }
    }
}
