using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{

    public Player player;
    public Enemy enemy;

    public GameObject finishPanel;


    private void OnEnable()
    {
        Player.isDead += Finish;
        Enemy.isDead += Finish;
    }
    private void OnDisable()
    {
        Player.isDead -= Finish;
        Enemy.isDead -= Finish;
    }
    void Start()
    {
        AudioManager.Instance.PlayBgm(true);
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0f;
            AudioManager.Instance.EffectBgm(pause);
        }

        else
        {
            Time.timeScale = 1.0f;
            AudioManager.Instance.EffectBgm(pause);
        }
        
    }

    public void Finish(bool finish)
    {
        finishPanel.SetActive(finish);
        PauseGame(finish);
    }
    public void SceneRoad()
    {
        SceneManager.LoadScene("MainScene");
        PauseGame(false);
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }
}
