using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace State_Machines
{
    public class BattleStateMachine : MonoBehaviour
    {
        public static BattleStateMachine Instance;
    
        public enum PerformAction
        {
            WAIT,
            TAKEACTION,
            PERFORMACTION
        }

        public PerformAction battleState;
    
        public List<HandleTurn> performActionsList = new List<HandleTurn>();
        public List<GameObject> heroesInBattle = new List<GameObject>();
        public List<GameObject> enemiesInBattle = new List<GameObject>();
    
        public enum HeroGUI
        {
            ACTIVATE,
            WAITING,
            INPUT1,
            INPUT2,
            DONE
        }
        public HeroGUI heroInput;
    
        public List<GameObject> heroesToManage = new List<GameObject>();
        private HandleTurn _heroChoice;

        public GameObject enemyButton;
        public Transform spacer;

        public GameObject attackPanel;
        public GameObject enemySelectPanel;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            battleState = PerformAction.WAIT;
            enemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
            heroesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
            heroInput = HeroGUI.ACTIVATE;
            attackPanel.SetActive(false);
            enemySelectPanel.SetActive(false);
            EnemyButtons();
        }

        private void Update()
        {
            switch (battleState)
            {
                case (PerformAction.WAIT):
                    if (performActionsList.Count > 0) battleState = PerformAction.TAKEACTION;
                    break;
                case PerformAction.TAKEACTION:
                    GameObject performer = GameObject.Find(performActionsList[0].Attacker);
                    if (performActionsList[0].Type == "Enemy")
                    {
                        EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                        ESM.heroToAttack = performActionsList[0].TargetObject;
                        ESM.currentTurnState = EnemyStateMachine.TurnState.ACTION;
                    }
                    else if (performActionsList[0].Type == "Hero")
                    {
                        HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                        HSM.enemyToAttack = performActionsList[0].TargetObject;
                        HSM.currentTurnState = HeroStateMachine.TurnState.ACTION;
                    }
                    battleState = PerformAction.PERFORMACTION;
                    break;
                case PerformAction.PERFORMACTION:
                    break;
            }

            switch (heroInput)
            {
                case (HeroGUI.ACTIVATE):
                    if (heroesToManage.Count > 0)
                    {
                        heroesToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                        _heroChoice = new HandleTurn();
                        attackPanel.SetActive(true);
                        heroInput = HeroGUI.WAITING;
                    }
                    break;
                case (HeroGUI.WAITING):
                    break;
                case (HeroGUI.DONE):
                    HeroInputDone();
                    break;
            }
        }

        public void CollectActions(HandleTurn input)
        {
            performActionsList.Add(input);
        }

        private void EnemyButtons()
        {
            foreach (GameObject enemy in enemiesInBattle)
            {
                GameObject newButton = Instantiate(enemyButton, spacer, false) as GameObject;
                EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();
                EnemyStateMachine currentEnemy = enemy.GetComponent<EnemyStateMachine>();
                TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = currentEnemy.enemy.actorName;
                button.enemyPrefab = enemy;
            }
        }

        public void Input1()
        {
            _heroChoice.Attacker = heroesToManage[0].name;
            _heroChoice.AttackerObject = heroesToManage[0];
            _heroChoice.Type = "Hero";
            attackPanel.SetActive(false);
            enemySelectPanel.SetActive(true);
        }

        public void Input2(GameObject chosenEnemy)
        {
            _heroChoice.TargetObject = chosenEnemy;
            heroInput = HeroGUI.DONE;
        }

        private void HeroInputDone()
        {
            performActionsList.Add(_heroChoice);
            enemySelectPanel.SetActive(false);
            heroesToManage[0].transform.Find("Selector").gameObject.SetActive(false);
            heroesToManage.RemoveAt(0);
            heroInput = HeroGUI.ACTIVATE;
        }
    }
}
