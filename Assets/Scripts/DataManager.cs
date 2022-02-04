using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


[System.Serializable]
public class DataManager : MonoBehaviour
{
    public DataManager Instance;
    public GameObject startButton;
    public Text playerNameText;
    public Text playerNameTextPreview;

    public string PlayerName;
    public string BestPlayer;
    public int BestScore = 0;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
        loadGame();
    }

    public void EnregistrerNom(Text name)
    {
        PlayerName = name.text;
    }

    public void ChargerNomJoueur(Text playerNameText)
    {
        playerNameText.text = PlayerName;
        playerNameTextPreview.text = PlayerName;
        startButton.SetActive(true);
    }

    public void loadGame()
    {
        string path = Application.persistentDataPath + "/saveFile.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            MainManager.SaveFile savedGame;
            savedGame = JsonUtility.FromJson<MainManager.SaveFile>(json);
            SynchroniserInstanceEtSauvegarde(Instance, savedGame);
            if(playerNameText != null) ChargerNomJoueur(playerNameText);
        }
    }

    public void SynchroniserInstanceEtSauvegarde(DataManager instance, MainManager.SaveFile saveFile)
    {
        instance.PlayerName = saveFile.playerName;
        instance.BestPlayer = saveFile.bestPlayer;
        instance.BestScore = saveFile.bestScore;
    }
}
