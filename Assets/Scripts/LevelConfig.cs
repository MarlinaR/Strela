using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Levels;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelConfig : MonoBehaviour
{
    private const string highscorePrefix = "hs_";
    private const int MaxHighscoreCnt = 10;

    public bool isGameEnd = false;
    public int score = 0;
    public int arrows = 10;
    public bool isTestRun = false;
    public int coin = 0;

    private InGameUI inGameUI;

    private List<ScoreInfo> highscores;

    private string levelName = string.Empty;
    
    public Dictionary<Vector2Int, GameObject> levelMap;

    private void LoadHighscores()
    {
        highscores = new List<ScoreInfo>();

        string filename = highscorePrefix + levelName;
        if (levelName == String.Empty)
        {
            filename = highscorePrefix + SceneManager.GetActiveScene().name;
        }
        
        if (!File.Exists(filename))
        {
            return;
        }

        using (var reader = new BinaryReader(File.Open(filename, FileMode.Open)))
        {
            int highscoresCnt = reader.ReadInt32();
            for (int i = 0; i < highscoresCnt; i++)
            {
                highscores.Add(ScoreInfo.Deserialize(reader));
            }
        }
    }

    private void SaveHighScores()
    {
        if (isTestRun)
        {
            return;
        }
        
        highscores.Sort();
        string filename = highscorePrefix + levelName;
        if (levelName == String.Empty)
        {
            filename = highscorePrefix + SceneManager.GetActiveScene().name;
        }
        
        using (var writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
        {
            int highscoresCnt = MaxHighscoreCnt;
            if (highscores.Count < highscoresCnt)
            {
                highscoresCnt = highscores.Count;
            }

            writer.Write(highscoresCnt);
            for (int i = 0; i < highscoresCnt; i++)
            {
                highscores[i].Serialize(writer);
            }
        }
    }
    
    void Awake()
    {
        if (levelMap == null)
        {
            levelMap = new Dictionary<Vector2Int, GameObject>();
        }
        
        GameSettings.currentLevel = this;
    }

    IEnumerator TimeScore()
    {
        while (!isGameEnd)
        {
            score++;
            yield return new WaitForSeconds(1);
        }
    }

    private void Start()
    {
        LoadHighscores();
       
        inGameUI = Instantiate(GameSettings.instance.InGameUI).GetComponent<InGameUI>();
        inGameUI.endGameMenu.SetActive(false);

        StartCoroutine(TimeScore());
    }

    public void Lose() {
        if (isGameEnd) {
            return;
        }
        isGameEnd = true;
        inGameUI.loseGameMenu.SetActive(true);
    }
    
    public void Win()
    {
        if (isGameEnd) {
            return;
        }
        isGameEnd = true;
        
        highscores.Add(new ScoreInfo(score, GameSettings.instance.Username) { isLast = true});
        highscores.Sort();
        
        int highscoresCnt = MaxHighscoreCnt;
        if (highscores.Count < highscoresCnt)
        {
            highscoresCnt = highscores.Count;
        }
        
        inGameUI.LoadHighscores(highscores, highscoresCnt);
        SaveHighScores();

        inGameUI.endGameMenu.SetActive(true);
    }
}
