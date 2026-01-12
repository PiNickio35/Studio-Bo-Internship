using System;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        battleState = PerformAction.WAIT;
        enemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        heroesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
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
                    
                }
                battleState = PerformAction.PERFORMACTION;
                break;
            case PerformAction.PERFORMACTION:
                break;
        }
    }

    public void CollectActions(HandleTurn input)
    {
        performActionsList.Add(input);
    }
}
