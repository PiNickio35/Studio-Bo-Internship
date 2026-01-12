using System;
using UnityEngine;
using UnityEngine.UI;

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
    
    private float _currentCooldown;
    private float _maxCooldown = 5f;

    private void Start()
    {
        currentTurnState = TurnState.PROCESSING;
    }

    private void Update()
    {
        switch (currentTurnState)
        {
            case (TurnState.PROCESSING):
                UpdateProgressBar();
                break;
            case (TurnState.ADDTOLIST):
                break;
            case (TurnState.WAITING):
                break;
            case (TurnState.SELECTING):
                break;
            case (TurnState.ACTION):
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
}
