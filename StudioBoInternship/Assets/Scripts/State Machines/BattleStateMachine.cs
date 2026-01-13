using System.Collections.Generic;
using Base_Classes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        public GameObject magicPanel;

        public Transform actionSpacer;
        public Transform magicSpacer;
        public GameObject actionButton;
        private List<GameObject> actionButtons = new List<GameObject>();

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
            magicPanel.SetActive(false);
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
                        bool found = false;
                        for (int i = 0; i < heroesInBattle.Count; i++)
                        {
                            if (performActionsList[0].TargetObject == heroesInBattle[i])
                            {
                                ESM.heroToAttack = performActionsList[0].TargetObject;
                                ESM.currentTurnState = EnemyStateMachine.TurnState.ACTION;
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            performActionsList[0].TargetObject = heroesInBattle[Random.Range(0, heroesInBattle.Count)];
                            ESM.heroToAttack = performActionsList[0].TargetObject;
                            ESM.currentTurnState = EnemyStateMachine.TurnState.ACTION;
                        }
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
                        CreateActionButtons();
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
            _heroChoice.chosenAttack = heroesToManage[0].GetComponent<HeroStateMachine>().hero.actorAttacks[0];
            attackPanel.SetActive(false);
            enemySelectPanel.SetActive(true);
        }

        public void MagicInput(BaseAttack chosenMagicAttack)
        {
            _heroChoice.Attacker = heroesToManage[0].name;
            _heroChoice.AttackerObject = heroesToManage[0];
            _heroChoice.Type = "Hero";
            _heroChoice.chosenAttack = chosenMagicAttack;
            magicPanel.SetActive(false);
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
            foreach (var attackButton in actionButtons)
            {
                Destroy(attackButton);
            }
            actionButtons.Clear();
            heroesToManage[0].transform.Find("Selector").gameObject.SetActive(false);
            heroesToManage.RemoveAt(0);
            heroInput = HeroGUI.ACTIVATE;
        }

        private void CreateActionButtons()
        {
            GameObject attackButton = Instantiate(actionButton, actionSpacer, false);
            TextMeshProUGUI attackButtonText = attackButton.transform.GetComponentInChildren<TextMeshProUGUI>();
            attackButtonText.text = "Attack";
            attackButton.GetComponent<Button>().onClick.AddListener(() => Input1());
            actionButtons.Add(attackButton);
            
            if (heroesToManage[0].GetComponent<HeroStateMachine>().hero.magicAttacks.Count > 0)
            {
                GameObject magicButton = Instantiate(actionButton, actionSpacer, false);
                TextMeshProUGUI magicButtonText = magicButton.transform.GetComponentInChildren<TextMeshProUGUI>();
                magicButtonText.text = "Magic";
                magicButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    attackPanel.SetActive(false);
                    magicPanel.SetActive(true);
                    foreach (BaseAttack magicAttack in heroesToManage[0].GetComponent<HeroStateMachine>().hero.magicAttacks)
                    {
                        GameObject magicButton = Instantiate(actionButton, magicSpacer, false);
                        TextMeshProUGUI magicText = magicButton.transform.GetComponentInChildren<TextMeshProUGUI>();
                        magicText.text = magicAttack.attackName;
                        magicButton.GetComponent<Button>().onClick.AddListener(() => MagicInput(magicAttack));
                    }
                });
                actionButtons.Add(magicButton);
            }
        }
    }
}
