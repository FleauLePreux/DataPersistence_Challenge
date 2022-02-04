using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text BestScoreText;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    private DataManager dataManager;

    
    // Start is called before the first frame update
    void Start()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        m_Points = -1;
        AddPoint(1);

        if(dataManager.BestPlayer != null)
        {
            BestScoreText.text = "Best Score: " + dataManager.BestPlayer + ": " + dataManager.BestScore;
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = dataManager.PlayerName + $" Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        if(m_Points > dataManager.BestScore)
        {
            dataManager.BestScore = m_Points;
            dataManager.BestPlayer = dataManager.PlayerName;
        }

        SaveGame(dataManager.PlayerName, dataManager.BestPlayer , dataManager.BestScore);
    }

    [System.Serializable]
    public class SaveFile
    {
        public string playerName;
        public string bestPlayer;
        public int bestScore;
    }

    public void InitialiserSaveFile(SaveFile saveFile, string NameP, string bestP, int bestS)
    {
        saveFile.playerName = NameP;
        saveFile.bestPlayer = bestP;
        saveFile.bestScore = bestS;
    }

    public void SaveGame(string NameP, string bestP, int bestS)
    {
        SaveFile saveFileInstance = new SaveFile();
        InitialiserSaveFile(saveFileInstance, NameP, bestP, bestS);
        string saveText = JsonUtility.ToJson(saveFileInstance);
        File.WriteAllText(Application.persistentDataPath + "/saveFile.json", saveText);
    }
}
