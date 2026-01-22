using System.Collections.Generic;
using Base_Classes;
using Interfaces;
using UnityEngine;

public class RestorationStation : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator playerAnimator;

    public void Interact()
    {
        SoundEffectManager.Play("Restoration");
        playerAnimator.Play("Restore");
        List<BaseHero> heroes = GameManager.Instance.updatedHeroes;
        foreach (BaseHero hero in heroes)
        {
            hero.CurrentHp = hero.BaseHp;
            hero.CurrentMp = hero.BaseMp;
        }
        PlayerPage.Instance.UpdateStats();
    }

    public bool CanInteract()
    {
        return true;
    }
}
