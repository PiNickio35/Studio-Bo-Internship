using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private string _saveLocation;
    [SerializeField] private GameObject instructionPanel;
    
    private void Awake()
    {
        _saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    public void StartGame()
    {
        SceneManager.LoadScene(!File.Exists(_saveLocation) ? "OpeningScene" : "ExploreScene");
    }

    public void Instructions()
    {
        instructionPanel.SetActive(true);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
