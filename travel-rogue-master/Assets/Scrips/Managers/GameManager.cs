using System;
using System.Collections;
using System.Collections.Generic;
using Ability;
using UnityEngine;

[DefaultExecutionOrder(-200)]
public class GameManager : Singleton<GameManager>
{
    public Player playerPrefab;
    public Level levelPrefab;
    public Camera playerCamera;
    public Camera uiCamera;
    public MapPiecesController MapPiecesController;

    [HideInInspector] public Player player;
    [HideInInspector] public Level level;

    public event Action OnLoadGame;

    void Start()
    {
        LoadNewGame();
        Resources.Load<BuffIndexer>("BuffIndexer").Initialize();
    }

    // Update is called once per frame
    void Update()
    {
    }

    //加载初始化
    private void LoadNewGame()
    {
        player = Instantiate(playerPrefab);
        level = Instantiate(levelPrefab);
        OnLoadGame?.Invoke();
    }
    
    public void PauseGame()
    {
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0;
    }

    // public void PausePlayer()
    // {
    //     player.PlayerPause();
    // }
}