using State_Machines;
using UnityEngine;

public class EnemySelectButton : MonoBehaviour
{
    public GameObject enemyPrefab;
    private bool _showSelector;

    public void SelectEnemy()
    {
        BattleStateMachine.Instance.EnemySelectionInput(enemyPrefab);
    }

    public void ToggleSelector()
    {
        enemyPrefab.transform.Find("Selector").gameObject.SetActive(!_showSelector);
        _showSelector = !_showSelector;
    }
}
