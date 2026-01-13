using System.Collections;
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
    
    public Image progressBar;

    [SerializeField] private GameObject selector;
    
    private float _currentCooldown;
    private float _maxCooldown = 5f;

    public GameObject enemyToAttack;
    private bool _actionStarted;
    private Vector3 _startPosition;
    private float _animationSpeed = 10f;

    private void Start()
    {
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
}
