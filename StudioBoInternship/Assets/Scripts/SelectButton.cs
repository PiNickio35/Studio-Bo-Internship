using State_Machines;
using UnityEngine;

public class SelectButton : MonoBehaviour
{
    public GameObject prefab;
    private bool _showSelector;

    public void Select()
    {
        BattleStateMachine.Instance.SelectionInput(prefab);
    }

    public void ToggleSelector()
    {
        prefab.transform.Find("Selector").gameObject.SetActive(!_showSelector);
        _showSelector = !_showSelector;
    }
}
