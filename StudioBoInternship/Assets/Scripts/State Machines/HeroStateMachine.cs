using System.Collections;
using Base_Classes;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace State_Machines
{
    public class HeroStateMachine : MonoBehaviour
    {
        public BaseHero hero;

        public enum TurnState
        {
            PROCESSING,
            ADDTOLIST,
            WAITING,
            DEFENDING,
            ITEM,
            ACTION,
            DEAD
        }
    
        public TurnState currentTurnState;
    
        private Image progressBar;

        [SerializeField] private GameObject selector;
    
        private float _currentCooldown;
        private float _maxCooldown = 7.5f;

        public GameObject enemyToAttack;
        private bool _actionStarted;
        private Vector3 _startPosition;
        private float _animationSpeed = 10f;
    
        private bool _isAlive = true;

        public int chosenItem;

        private HeroPanelStats stats;
        [SerializeField] private GameObject heroPanel;

        private void Start()
        {
            CreateHeroPanel();
            _currentCooldown = Random.Range(0f, 2.5f);
            currentTurnState = TurnState.PROCESSING;
            _startPosition = this.transform.position;
            selector.SetActive(false);
        }

        private void Update()
        {
            switch (currentTurnState)
            {
                case (TurnState.PROCESSING):
                    UpdateProgressBar();
                    break;
                case (TurnState.ADDTOLIST):
                    BattleStateMachine.Instance.heroesToManage.Add(this.gameObject);
                    currentTurnState = TurnState.WAITING;
                    break;
                case (TurnState.WAITING):
                    break;
                case (TurnState.DEFENDING):
                    StartCoroutine(TimeForDefence());
                    break;
                case TurnState.ITEM:
                    StartCoroutine(TimeForItem());
                    break;
                case (TurnState.ACTION):
                    StartCoroutine(TimeForAction());
                    break;
                case (TurnState.DEAD):
                    if (!_isAlive) return;
                    this.gameObject.tag = "DeadHero";
                    BattleStateMachine.Instance.heroesInBattle.Remove(this.gameObject);
                    selector.SetActive(false);
                    // BattleStateMachine.Instance.attackPanel.SetActive(false);
                    // BattleStateMachine.Instance.enemySelectPanel.SetActive(false);
                    // TODO Make sure this works properly and doesn't go out of range.
                    if (BattleStateMachine.Instance.heroesInBattle.Count > 0)
                    {
                        for (int i = 0; i < BattleStateMachine.Instance.performActionsList.Count; i++)
                        {
                            if (BattleStateMachine.Instance.performActionsList[i].AttackerObject == this.gameObject)
                            {
                                BattleStateMachine.Instance.performActionsList.Remove(BattleStateMachine.Instance.performActionsList[i]);
                            }
                            else if (BattleStateMachine.Instance.performActionsList[i].TargetObject == this.gameObject)
                            {
                                BattleStateMachine.Instance.performActionsList[i].TargetObject = BattleStateMachine.Instance.heroesInBattle[Random.Range(0, BattleStateMachine.Instance.heroesInBattle.Count)];
                            }
                        }
                    }
                    this.gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
                    if (BattleStateMachine.Instance.heroesToManage.Count > 0)
                    {
                        if (BattleStateMachine.Instance.heroesToManage[0] == this.gameObject)
                        {
                            BattleStateMachine.Instance.heroesToManage.Remove(this.gameObject);
                            BattleStateMachine.Instance.heroInput = BattleStateMachine.HeroGUI.ACTIVATE;
                        }
                        else
                        {
                            BattleStateMachine.Instance.heroesToManage.Remove(this.gameObject);
                        }
                    }
                    BattleStateMachine.Instance.battleState = BattleStateMachine.PerformAction.CHECKALIVE;
                    _isAlive = false;
                    break;
            }
        }

        private void UpdateProgressBar()
        {
            _currentCooldown += Time.deltaTime + (hero.agility/10000);
            progressBar.transform.localScale = new Vector3(Mathf.Clamp((_currentCooldown / _maxCooldown), 0, 1f),
                progressBar.transform.localScale.y, progressBar.transform.localScale.z);
            if (_currentCooldown >= _maxCooldown)
            {
                currentTurnState = TurnState.ADDTOLIST;
            }
        }
    
        private IEnumerator TimeForAction()
        {
            if (_actionStarted)
            {
                yield break;
            }

            _actionStarted = true;
        
            Vector3 enemyPosition = new Vector3(enemyToAttack.transform.position.x - 1.5f, enemyToAttack.transform.position.y,
                enemyToAttack.transform.position.z);
            while (MoveTowardsTarget(enemyPosition)) yield return null;

            yield return new WaitForSeconds(0.5f);
        
            DoDamage();
            UpdateHeroPanel();

            while (MoveTowardsTarget(_startPosition)) yield return null;

            BattleStateMachine.Instance.performActionsList.RemoveAt(0);

            if (BattleStateMachine.Instance.battleState != BattleStateMachine.PerformAction.WIN &&
                BattleStateMachine.Instance.battleState != BattleStateMachine.PerformAction.LOSE)
            {
                BattleStateMachine.Instance.battleState = BattleStateMachine.PerformAction.WAIT;
                _currentCooldown = 0f;
                currentTurnState = TurnState.PROCESSING;
            }
            else
            {
                currentTurnState = TurnState.WAITING;
            }
            _actionStarted = false;
        }

        private IEnumerator TimeForDefence()
        {
            if (_actionStarted)
            {
                yield break;
            }

            _actionStarted = true;

            StartCoroutine(UpDefence());

            BattleStateMachine.Instance.performActionsList.RemoveAt(0);

            if (BattleStateMachine.Instance.battleState != BattleStateMachine.PerformAction.WIN &&
                BattleStateMachine.Instance.battleState != BattleStateMachine.PerformAction.LOSE)
            {
                BattleStateMachine.Instance.battleState = BattleStateMachine.PerformAction.WAIT;
                _currentCooldown = 0f;
                currentTurnState = TurnState.PROCESSING;
            }
            else
            {
                currentTurnState = TurnState.WAITING;
            }
            _actionStarted = false;
        }

        private IEnumerator TimeForItem()
        {
            if (_actionStarted)
            {
                yield break;
            }

            _actionStarted = true;

            foreach (InventorySaveData item in GameManager.Instance.availableItems)
            {
                if (item.itemID != chosenItem) continue;
                BaseHero targetHero = enemyToAttack.GetComponent<HeroStateMachine>().hero;
                switch (chosenItem)
                {
                    case 0:
                        targetHero.CurrentHp = Mathf.Min(targetHero.BaseHp, targetHero.CurrentHp + 100);
                        break;
                    case 1:
                        targetHero.CurrentMp = Mathf.Min(targetHero.BaseMp, targetHero.CurrentMp + 20);
                        break;
                    default:
                        break;
                }
                GameManager.Instance.availableItems.Remove(item);
                break;
            }

            BattleStateMachine.Instance.performActionsList.RemoveAt(0);

            if (BattleStateMachine.Instance.battleState != BattleStateMachine.PerformAction.WIN &&
                BattleStateMachine.Instance.battleState != BattleStateMachine.PerformAction.LOSE)
            {
                BattleStateMachine.Instance.battleState = BattleStateMachine.PerformAction.WAIT;
                _currentCooldown = 0f;
                currentTurnState = TurnState.PROCESSING;
            }
            else
            {
                currentTurnState = TurnState.WAITING;
            }
            _actionStarted = false;
        }

        private IEnumerator UpDefence()
        {
            hero.Defence += 5f;
            yield return new WaitForSeconds(5f);
            hero.Defence -= 5f;
        }

        private bool MoveTowardsTarget(Vector3 target)
        {
            return target != (transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * _animationSpeed));
        }

        public void TakeDamage(float damageAmount)
        {
            damageAmount = Mathf.Max(damageAmount - Mathf.Floor(hero.agility/8) - hero.Defence, 1);
            hero.CurrentHp -= damageAmount;
            if (hero.CurrentHp <= 0)
            {
                hero.CurrentHp = 0;
                currentTurnState = TurnState.DEAD;
            }
            UpdateHeroPanel();
        }

        private void DoDamage()
        {
            enemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(hero.Attack);
        }

        public void CreateHeroPanel()
        {
            stats = heroPanel.GetComponentInChildren<HeroPanelStats>();
            stats.heroName.text = hero.ActorName;
            stats.heroHP.text = "HP: " + hero.CurrentHp + "/" + hero.BaseHp;
            stats.heroMP.text = "MP: " + hero.CurrentMp + "/" + hero.BaseMp;
            progressBar = stats.progressbar;
        }

        private void UpdateHeroPanel()
        {
            stats.heroHP.text = "HP: " + hero.CurrentHp + "/" + hero.BaseHp;
            stats.heroMP.text = "MP: " + hero.CurrentMp + "/" + hero.BaseMp;
        }
    }
}
