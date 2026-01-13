using UnityEngine;

public class EnemySelectButton : MonoBehaviour
{
    public GameObject enemyPrefab;

    public void SelectEnemy()
    {
        BattleStateMachine.Instance.Input2(enemyPrefab);
    }
}
