using System.Collections.Generic;
using Base_Classes;
using TMPro;
using UnityEngine;

public class PlayerPage : MonoBehaviour
{
    public static PlayerPage Instance;
    [SerializeField] private TMP_Text sonoHp;
    [SerializeField] private TMP_Text sonoMp;
    [SerializeField] private TMP_Text mayHp;
    [SerializeField] private TMP_Text mayMp;
    [SerializeField] private TMP_Text andaniHp;
    [SerializeField] private TMP_Text andaniMp;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateStats()
    {
        List<BaseHero> heroes = GameManager.Instance.updatedHeroes;
        sonoHp.text = "HP: " + heroes[0].CurrentHp + "/" + heroes[0].BaseHp;
        sonoMp.text = "MP: " + heroes[0].CurrentMp + "/" + heroes[0].BaseMp;
        mayHp.text = "HP: " + heroes[1].CurrentHp + "/" + heroes[1].BaseHp;
        mayMp.text = "MP: " + heroes[1].CurrentMp + "/" + heroes[1].BaseMp;
        andaniHp.text = "HP: " + heroes[2].CurrentHp + "/" + heroes[2].BaseHp;
        andaniMp.text = "MP: " + heroes[2].CurrentMp + "/" + heroes[2].BaseMp;
    }
}
