using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] protected int currentCash;
    protected bool gameOver;
    
    private void OnEnable()
    {
        GameEvents.UpdateCash += GetCash;
        GameEvents.Restart += Restart;
        GameEvents.Quit += Quit;
        GameEvents.GameOver += GameOver;
        GameEvents.NextLevel += NextLevel;
    }

    private void OnDisable()
    {
        GameEvents.UpdateCash -= GetCash;
        GameEvents.Restart -= Restart;
        GameEvents.Quit -= Quit;
        GameEvents.GameOver -= GameOver;
        GameEvents.NextLevel -= NextLevel;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GameOver()
    {
        gameOver = true;
        Time.timeScale = 0f;
    }

    private void Restart()
    {
        if (SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex) == SceneManager.GetActiveScene())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            Debug.LogError("Scene not in Build Index");
        }
    }

    private void Quit()
    {
        print("Quit to Menu");
    }

    private void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void GetCash(int cash)
    {
        currentCash += cash;
        GameEvents.DisplayCash(currentCash);
    }
}
