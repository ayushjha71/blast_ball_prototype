// GameManager.cs
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverPanelPrefab;

    private int mCurrentLevel = 1;
    private GameObject mCurrentGameOverPanel;
    private bool isGameOver = false;


    private TMP_Text mLevelText;

    public GridManager GridManager
    {
        get;
        private set;
    }
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isGameOver = false;
        GridManager = FindObjectOfType<GridManager>();

        mLevelText = GridManager.LevelText;


        if (mLevelText != null)
        {
            mLevelText.text = "Level " + mCurrentLevel;
        }
        if (mCurrentGameOverPanel != null)
        {
            Destroy(mCurrentGameOverPanel);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void CheckLevelCompletion(Dictionary<Color, int> colorCounts, Dictionary<Color, int> bottomRowColors)
    {
        bool hasColorsLeft = false;
        foreach (var count in colorCounts.Values)
        {
            if (count > 0)
            {
                hasColorsLeft = true;
                break;
            }
        }
        if (!hasColorsLeft)
        {
            ShowGameOverPanel();
            return;
        }
        if (bottomRowColors.Count == 0 && hasColorsLeft)
        {
            ShowGameOverPanel();
            return;
        }

        //Use to show gameover panel when slot get full and grid having color and no match found TODO
        // Check if any slot has a matching color in bottom row
        //bool possibleMatchExists = false;
        //Slot[] slots = FindObjectsOfType<Slot>();
        //foreach (Slot slot in slots)
        //{
        //    if (slot.IsFilled())
        //    {
        //        Color slotColor = slot.GetColor().Value;
        //        if (bottomRowColors.ContainsKey(slotColor))
        //        {
        //            possibleMatchExists = true;
        //            break;
        //        }
        //    }
        //}
    }

    private void ShowGameOverPanel()
    {
        if (gameOverPanelPrefab != null && !isGameOver)
        {
            isGameOver = true;
            mCurrentGameOverPanel = Instantiate(gameOverPanelPrefab);
        }
        else if (gameOverPanelPrefab == null)
        {
            Debug.LogError("Game Over Panel Prefab not assigned in GameManager!");
        }
    }

    public void NextLevel()
    {
        mCurrentLevel++;
        isGameOver = false;

        if (mCurrentGameOverPanel != null)
        {
            Destroy(mCurrentGameOverPanel);
        }
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        if (mCurrentLevel >= totalScenes)
        {
            mCurrentLevel = 1; 
            SceneManager.LoadScene(0); 
        }
        else
        {
            SceneManager.LoadScene("Level_" + mCurrentLevel);
            GridManager.LevelText.text = "Level " + mCurrentLevel;
        }
    }

    public void RestartLevel()
    {
        isGameOver = false;
        if (mCurrentGameOverPanel != null)
        {
            Destroy(mCurrentGameOverPanel);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}