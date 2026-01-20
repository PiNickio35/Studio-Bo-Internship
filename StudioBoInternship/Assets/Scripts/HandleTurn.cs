using Base_Classes;
using UnityEngine;

[System.Serializable]
public class HandleTurn
{
    public bool isDefending;
    public string Attacker;
    public string Type;
    public GameObject AttackerObject;
    public GameObject TargetObject;

    public BaseAttack chosenAttack;
}
