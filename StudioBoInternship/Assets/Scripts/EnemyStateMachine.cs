using System.Collections;
using UnityEngine;

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

    private bool _actionStarted;
    public GameObject heroToAttack;
    private float _animationSpeed = 10f;

    private void Start()
    {
        currentTurnState = TurnState.PROCESSING;
        _startPosition = transform.position;
    }

    private void Update()
    {
        switch (currentTurnState)
        {
            case (TurnState.PROCESSING):
                UpdateProgressBar();
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
                break;
        }
    }
    
    private void UpdateProgressBar()
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
        myAttack.Attacker = enemy.name;
        myAttack.Type = "Enemy";
        myAttack.AttackerObject = this.gameObject;
        myAttack.TargetObject = BattleStateMachine.Instance.heroesInBattle[UnityEngine.Random.Range(0, BattleStateMachine.Instance.heroesInBattle.Count)];
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
}
