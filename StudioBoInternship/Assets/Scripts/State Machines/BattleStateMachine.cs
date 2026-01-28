using System.Collections;
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
            PERFORMACTION,
            CHECKALIVE,
            WIN,
            LOSE
        }

        private int[] _levelThresholds = { 0, 100, 200, 400, 600, 800, 1000, 1200, 1500, 1800, 2200, 2600, 3000, 3500, 4000, 5000, 6000, 7000, 8000, 9000, 10000 };

        public PerformAction battleState;
    
        public List<HandleTurn> performActionsList = new();
        public List<GameObject> heroesAfterBattle = new(); 
        public List<GameObject> heroesInBattle = new();
        public List<GameObject> enemiesInBattle = new();
    
        public enum HeroGUI
        {
            ACTIVATE,
            WAITING,
            DONE
        }
        public HeroGUI heroInput;
    
        public List<GameObject> heroesToManage = new();
        private HandleTurn _heroChoice;

        public GameObject enemyButton;
        public Transform spacer;

        public GameObject attackPanel;
        public GameObject enemySelectPanel;
        public GameObject heroSelectionPanel;
        public GameObject magicPanel;
        public GameObject itemPanel;
        public GameObject losePanel;
        public GameObject levelUpPanel;

        [SerializeField] private GameObject noMagicPanel;
        [SerializeField] private SpriteRenderer battleBackground;

        public Transform actionSpacer;
        public Transform magicSpacer;
        public Transform itemSpacer;
        public GameObject actionButton;
        private List<GameObject> _actionButtons = new();
        
        private List<GameObject> _enemyButtons = new();
        
        public List<Transform> spawnPoints = new();

        public int experiencePool;

        private void Awake()
        {
            Instance = this;
            for (int i = 0; i < GameManager.Instance.enemyAmount; i++)
            {
                GameObject newEnemy = Instantiate(GameManager.Instance.enemiesToBattle[i], spawnPoints[i].position, Quaternion.identity);
                newEnemy.name = newEnemy.GetComponent<EnemyStateMachine>().enemy.ActorName + " " + (i + 1);
                newEnemy.GetComponent<EnemyStateMachine>().enemy.ActorName = newEnemy.name;
                enemiesInBattle.Add(newEnemy);
            }
            battleBackground.sprite = GameManager.Instance.currentRegion.battleBackground;
        }

        private void Start()
        {
            battleState = PerformAction.WAIT;
            heroesAfterBattle = new List<GameObject>(heroesInBattle);
            for (int i = 0; i < heroesInBattle.Count; i++)
            {
                HeroStateMachine statsHolder = heroesInBattle[i].GetComponent<HeroStateMachine>();
                statsHolder.hero = GameManager.Instance.updatedHeroes[i];
                statsHolder.CreateHeroPanel();
            }
            heroInput = HeroGUI.ACTIVATE;
            attackPanel.SetActive(false);
            magicPanel.SetActive(false);
            itemPanel.SetActive(false);
            enemySelectPanel.SetActive(false);
            heroSelectionPanel.SetActive(false);
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
                            if (heroesInBattle.Count <= 0)
                            {
                                ESM.currentTurnState = EnemyStateMachine.TurnState.WAITING;
                                battleState = PerformAction.LOSE;
                                return;
                            }
                            performActionsList[0].TargetObject = heroesInBattle[Random.Range(0, heroesInBattle.Count)];
                            ESM.heroToAttack = performActionsList[0].TargetObject;
                            ESM.currentTurnState = EnemyStateMachine.TurnState.ACTION;
                        }
                    }
                    else if (performActionsList[0].Type == "Hero")
                    {
                        if (performActionsList[0].isDefending)
                        {
                            performer.GetComponent<HeroStateMachine>().currentTurnState =
                                HeroStateMachine.TurnState.DEFENDING;
                        }
                        else if (performActionsList[0].isUsingItem)
                        {
                            HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                            HSM.enemyToAttack = performActionsList[0].TargetObject;
                            HSM.chosenItem = _heroChoice.itemToUse;
                            HSM.currentTurnState = HeroStateMachine.TurnState.ITEM;
                        }
                        else
                        {
                            HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                            HSM.enemyToAttack = performActionsList[0].TargetObject;
                            HSM.currentTurnState = HeroStateMachine.TurnState.ACTION;
                        }
                    }
                    battleState = PerformAction.PERFORMACTION;
                    break;
                case PerformAction.PERFORMACTION:
                    break;
                case PerformAction.CHECKALIVE:
                    if (heroesInBattle.Count < 1)
                    {
                        battleState = PerformAction.LOSE;
                    }
                    else if (enemiesInBattle.Count < 1)
                    {
                        battleState = PerformAction.WIN;
                    }
                    break;
                case PerformAction.WIN:
                    for (int i = 0; i < heroesInBattle.Count; i++)
                    {
                        heroesInBattle[i].GetComponent<HeroStateMachine>().currentTurnState = HeroStateMachine.TurnState.WAITING;
                        LevelUp(heroesInBattle[i].GetComponent<HeroStateMachine>().hero);
                    }
                    for (int i = 0; i < GameManager.Instance.updatedHeroes.Count; i++)
                    {
                        GameManager.Instance.updatedHeroes[i] = heroesAfterBattle[i].GetComponent<HeroStateMachine>().hero;
                    }
                    
                    SaveController.Instance.SaveBattle();
                    GameManager.Instance.gameState = GameManager.GameStates.WORLD;
                    GameManager.Instance.enemiesToBattle.Clear();
                    if (!GameManager.Instance.isFinalBattle) GameManager.Instance.LoadSceneAfterBattle();
                    else GameManager.Instance.LoadFinalScene();
                    battleState = PerformAction.WAIT;
                    break;
                case PerformAction.LOSE:
                    Debug.Log("The heroes have lost!");
                    if (losePanel.activeSelf) break;
                    foreach (GameObject enemy in enemiesInBattle)
                    {
                        enemy.GetComponent<EnemyStateMachine>().currentTurnState = EnemyStateMachine.TurnState.WAITING;
                    }
                    losePanel.SetActive(true);
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

        public void EnemyButtons()
        {
            foreach (GameObject enemySelectButton in _enemyButtons)
            {
                Destroy(enemySelectButton);
            }
            _enemyButtons.Clear();
            foreach (GameObject enemy in enemiesInBattle)
            {
                GameObject newButton = Instantiate(enemyButton, spacer, false) as GameObject;
                SelectButton button = newButton.GetComponent<SelectButton>();
                EnemyStateMachine currentEnemy = enemy.GetComponent<EnemyStateMachine>();
                TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = currentEnemy.enemy.ActorName;
                button.prefab = enemy;
                _enemyButtons.Add(newButton);
            }
        }

        public void AttackInput()
        {
            BaseHero hero = heroesToManage[0].GetComponent<HeroStateMachine>().hero;
            _heroChoice.Attacker = heroesToManage[0].name;
            _heroChoice.AttackerObject = heroesToManage[0];
            _heroChoice.Type = "Hero";
            _heroChoice.chosenAttack = hero.ActorAttacks[0];
            hero.Attack = 
                Mathf.Max(Mathf.Floor(hero.strength/4), 1f) * hero.ActorAttacks[0].attackDamage;
            _heroChoice.isDefending = false;
            _heroChoice.isUsingItem = false;
            attackPanel.SetActive(false);
            enemySelectPanel.SetActive(true);
        }

        public void MagicInput(BaseAttack chosenMagicAttack)
        {
            BaseHero hero = heroesToManage[0].GetComponent<HeroStateMachine>().hero;
            if (chosenMagicAttack.attackCost > hero.CurrentMp)
            {
                StartCoroutine(NotEnoughMagic());
                return;
            }
            _heroChoice.Attacker = heroesToManage[0].name;
            _heroChoice.AttackerObject = heroesToManage[0];
            _heroChoice.Type = "Hero";
            _heroChoice.chosenAttack = chosenMagicAttack;
            hero.Attack = 
                Mathf.Max(Mathf.Floor(hero.wisdom/4), 1f) * chosenMagicAttack.attackDamage;
            hero.CurrentMp = Mathf.Max(hero.CurrentMp - chosenMagicAttack.attackCost, 0);
            _heroChoice.isDefending = false;
            _heroChoice.isUsingItem = false;
            magicPanel.SetActive(false);
            enemySelectPanel.SetActive(true);
        }

        public void DefendInput()
        {
            _heroChoice.Attacker = heroesToManage[0].name;
            _heroChoice.AttackerObject = heroesToManage[0];
            _heroChoice.Type = "Hero";
            _heroChoice.chosenAttack = null;
            _heroChoice.isDefending = true;
            _heroChoice.isUsingItem = false;
            attackPanel.SetActive(false);
            heroInput = HeroGUI.DONE;
        }

        public void ItemInput(int itemID)
        {
            _heroChoice.Attacker = heroesToManage[0].name;
            _heroChoice.AttackerObject = heroesToManage[0];
            _heroChoice.Type = "Hero";
            _heroChoice.chosenAttack = null;
            _heroChoice.isDefending = false;
            _heroChoice.isUsingItem = true;
            _heroChoice.itemToUse = itemID;
            attackPanel.SetActive(false);
            heroSelectionPanel.SetActive(true);
        }

        public void SelectionInput(GameObject chosenActor)
        {
            _heroChoice.TargetObject = chosenActor;
            heroInput = HeroGUI.DONE;
        }

        private void HeroInputDone()
        {
            performActionsList.Add(_heroChoice);
            ClearAttackPanel();
            heroesToManage[0].transform.Find("Selector").gameObject.SetActive(false);
            heroesToManage.RemoveAt(0);
            heroInput = HeroGUI.ACTIVATE;
        }

        private void ClearAttackPanel()
        {
            enemySelectPanel.SetActive(false);
            attackPanel.SetActive(false);
            magicPanel.SetActive(false);
            itemPanel.SetActive(false);
            heroSelectionPanel.SetActive(false);
            foreach (var attackButton in _actionButtons)
            {
                Destroy(attackButton);
            }
            _actionButtons.Clear();
        }

        private void CreateActionButtons()
        {
            GameObject attackButton = Instantiate(actionButton, actionSpacer, false);
            TextMeshProUGUI attackButtonText = attackButton.transform.GetComponentInChildren<TextMeshProUGUI>();
            attackButtonText.text = "Attack";
            attackButton.GetComponent<Button>().onClick.AddListener(() => AttackInput());
            _actionButtons.Add(attackButton);
            
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
                        _actionButtons.Add(magicButton);
                    }
                });
                _actionButtons.Add(magicButton);
            }
            
            GameObject defendButton = Instantiate(actionButton, actionSpacer, false);
            TextMeshProUGUI defendButtonText = defendButton.transform.GetComponentInChildren<TextMeshProUGUI>();
            defendButtonText.text = "Defend";
            defendButton.GetComponent<Button>().onClick.AddListener(() => DefendInput());
            _actionButtons.Add(defendButton);

            if (GameManager.Instance.availableItems.Count >= 1)
            {
                GameObject itemButton = Instantiate(actionButton, actionSpacer, false);
                TextMeshProUGUI itemButtonText = itemButton.transform.GetComponentInChildren<TextMeshProUGUI>();
                itemButtonText.text = "Item";
                itemButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    attackPanel.SetActive(false);
                    itemPanel.SetActive(true);
                    foreach (InventorySaveData item in GameManager.Instance.availableItems)
                    {
                        GameObject itemButton = Instantiate(actionButton, itemSpacer, false);
                        TextMeshProUGUI itemText = itemButton.transform.GetComponentInChildren<TextMeshProUGUI>();
                        itemText.text = GameManager.Instance.GetComponentInChildren<ItemDictionary>()
                            .itemPrefabs[item.itemID].itemName;
                        itemButton.GetComponent<Button>().onClick.AddListener(() => ItemInput(item.itemID));
                        _actionButtons.Add(itemButton);
                    }
                });
                _actionButtons.Add(itemButton);
            }
        }

        private IEnumerator NotEnoughMagic()
        {
            ClearAttackPanel();
            heroInput = HeroGUI.ACTIVATE;
            noMagicPanel.SetActive(true);
            yield return new WaitForSeconds(1f);
            noMagicPanel.SetActive(false);
        }
        
        private void LevelUp(BaseHero hero)
        {
            hero.experience += experiencePool;
            if (hero.level == 20)
            {
                return;
            }
            if (hero.experience >= _levelThresholds[hero.level])
            {
                hero.experience = 0;
                hero.level++;
                GameManager.Instance.LevelUp(hero.ActorName, hero.level);
                levelUpPanel.SetActive(true);
            }
        }

        public void CallMenu()
        {
            GameManager.Instance.GoToMenu();
        }
    }
}
