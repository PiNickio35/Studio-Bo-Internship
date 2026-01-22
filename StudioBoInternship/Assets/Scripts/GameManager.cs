using System.Collections;
using System.Collections.Generic;
using Base_Classes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<BaseHero> updatedHeroes = new List<BaseHero>();
    
    public List<GameObject> enemiesToBattle = new List<GameObject>();
    
    public BaseRegion currentRegion;
    
    public string nextSpawnPoint;
    
    [SerializeField] private GameObject heroPrefab;

    public Vector3 nextPlayerPosition;
    public Vector3 lastPlayerPosition;
    
    public string sceneToLoad;
    public string lastScene;

    public bool isWalking;
    public bool canGetEncounter;
    public bool gotAttacked;

    public int enemyAmount;
    
    private Animator _fadeAnim;
    private TMP_Text _fadeText;
    [SerializeField] private float fadeTime = 1f;

    public enum GameStates
    {
        WORLD,
        TOWN,
        BATTLE,
        IDLE
    }
    public GameStates gameState;

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
        DontDestroyOnLoad(gameObject);
        if (!GameObject.Find("Player"))
        {
            GameObject player = Instantiate(heroPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            player.name = "Player";
        }
        _fadeAnim = GetComponentInChildren<Animator>();
        _fadeText = GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameStates.WORLD:
                if (isWalking && canGetEncounter)
                {
                    RandomEncounter();
                }

                if (gotAttacked)
                {
                    gameState = GameStates.BATTLE;
                }
                break;
            case GameStates.TOWN:
                break;
            case GameStates.BATTLE:
                StartBattle();
                gameState = GameStates.IDLE;
                break;
            case GameStates.IDLE:
                break;
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void LoadSceneAfterBattle()
    {
        _fadeAnim.Play("FadeToMad");
        StartCoroutine(DelayFadeMad());
    }

    private void RandomEncounter()
    {
        if (Random.Range(0, 1000) <= 10)
        {
            gotAttacked = true;
        }
    }

    private void StartBattle()
    {
        gameObject.GetComponent<SaveController>().SaveGame();
        enemyAmount = Random.Range(1, currentRegion.maxEnemies + 1);
        for (int i = 0; i < enemyAmount; i++)
        {
            enemiesToBattle.Add(currentRegion.possibleEnemies[Random.Range(0, currentRegion.possibleEnemies.Count)]);
        }
        lastPlayerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        nextPlayerPosition = lastPlayerPosition;
        lastScene = SceneManager.GetActiveScene().name;
        _fadeAnim.Play("FadeToLaugh");
        StartCoroutine(DelayFadeLaugh());
        isWalking = false;
        gotAttacked = false;
    }

    private IEnumerator DelayFadeLaugh()
    {
        _fadeText.text = "Ha ha ha";
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(currentRegion.battleScene);
        _fadeAnim.Play("FadeFromLaugh");
    }

    private IEnumerator DelayFadeMad()
    {
        int temp = Random.Range(0, 2);
        switch (temp)
        {
            case 0:
                _fadeText.text = "Fools...";
                break;
            case 1:
                _fadeText.text = "This is meaningless.";
                break;
            case 2:
                _fadeText.text = "You got lucky...";
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(lastScene);
        _fadeAnim.Play("FadeFromMad");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
