using System.Collections.Generic;
using Base_Classes;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
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
        SceneManager.LoadScene(lastScene);
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
        SceneManager.LoadScene(currentRegion.battleScene);
        isWalking = false;
        gotAttacked = false;
        
    }
}
