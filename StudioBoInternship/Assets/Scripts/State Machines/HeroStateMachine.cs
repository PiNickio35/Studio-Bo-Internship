using System.Collections;
using Base_Classes;
using State_Machines;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HeroStateMachine : MonoBehaviour
{
    public BaseHero hero;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }
    
    public TurnState currentTurnState;
    
    private Image progressBar;

    [SerializeField] private GameObject selector;
    
    private float _currentCooldown;
    private float _maxCooldown = 5f;

    public GameObject enemyToAttack;
    private bool _actionStarted;
    private Vector3 _startPosition;
    private float _animationSpeed = 10f;
    
    private bool _isAlive = true;

    private HeroPanelStats stats;
    [SerializeField] private GameObject heroPanel;
    private Transform heroPanelSpacer;

    private void Start()
    {
        heroPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("HeroPanel").transform.Find("Spacer");
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
            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                break;
            case (TurnState.DEAD):
                if (!_isAlive) return;
                this.gameObject.tag = "DeadHero";
                BattleStateMachine.Instance.heroesInBattle.Remove(this.gameObject);
                BattleStateMachine.Instance.heroesToManage.Remove(this.gameObject);
                selector.SetActive(false);
                BattleStateMachine.Instance.attackPanel.SetActive(false);
                BattleStateMachine.Instance.enemySelectPanel.SetActive(false);
                for (int i = 0; i < BattleStateMachine.Instance.performActionsList.Count; i++)
                {
                    if (BattleStateMachine.Instance.performActionsList[i].AttackerObject == this.gameObject)
                    {
                        BattleStateMachine.Instance.performActionsList.Remove(BattleStateMachine.Instance.performActionsList[i]);
                    }
                }
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
                BattleStateMachine.Instance.heroInput = BattleStateMachine.HeroGUI.ACTIVATE;
                _isAlive = false;
                break;
        }
    }

    private void UpdateProgressBar()
    {
        _currentCooldown += Time.deltaTime;
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
        
        // Do damage

        while (MoveTowardsTarget(_startPosition)) yield return null;

        BattleStateMachine.Instance.performActionsList.RemoveAt(0);

        BattleStateMachine.Instance.battleState = BattleStateMachine.PerformAction.WAIT;
        _actionStarted = false;
        
        _currentCooldown = 0f;
        currentTurnState = TurnState.PROCESSING;
    }

    private bool MoveTowardsTarget(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * _animationSpeed));
    }

    public void TakeDamage(float damageAmount)
    {
        hero.currentHP -= damageAmount;
        if (hero.currentHP <= 0)
        {
            hero.currentHP = 0;
            currentTurnState = TurnState.DEAD;
        }
        UpdateHeroPanel();
    }

    private void CreateHeroPanel()
    {
        heroPanel = Instantiate(heroPanel, heroPanelSpacer, false) as GameObject;
        stats = heroPanel.GetComponentInChildren<HeroPanelStats>();
        stats.heroName.text = hero.actorName;
        stats.heroHP.text = "HP: " + hero.currentHP + "/" + hero.baseHP;
        stats.heroMP.text = "MP: " + hero.currentMP + "/" + hero.baseMP;
        progressBar = stats.progressbar;
    }

    private void UpdateHeroPanel()
    {
        stats.heroHP.text = "HP: " + hero.currentHP + "/" + hero.baseHP;
        stats.heroMP.text = "MP: " + hero.currentMP + "/" + hero.baseMP;
    }
}
