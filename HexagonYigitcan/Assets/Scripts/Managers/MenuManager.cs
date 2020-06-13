using System;
using System.Collections;
using Toolbox;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>
{
     GridManager gridManager;

     private int _score;

     private int moveCount;

     private int targetScore;

     [Header("Score")]
     [Space(5)]
     public Text scoreUIText;

     [Header("Move Count")]
     [Space(5)]
     public Text moveCountUIText;

     [Header("Menu Panel")]
     [Space(5)]
     public GameObject menuPanel;

     [Header("Game OVer Panel")]
     [Space(5)]
     public GameObject gameOver;

     [Header("Game Over Score")]
     [Space(5)]
     public Text scoreGOText;



     void Start()
     {
          targetScore = HexMetrics.BOMB_TARGET_SCORE;
          scoreUIText.text = "Score:0";
          moveCountUIText.text = "Moves: 0";
          gridManager = GridManager.Instance;
          gridManager.OnPlayerScore += HandleScrore;
          gridManager.OnMoveMade += HandleMove;

     }

     private void HandleMove()
     {
          moveCount++;
          print("move");
          moveCountUIText.text = $"Moves: {moveCount}";
     }

     private void HandleScrore(int score)
     {
          _score += score;
          scoreGOText.text = _score.ToString();
          scoreUIText.text = $"Score: {_score}";
          if (_score >= targetScore)
          {
               targetScore += HexMetrics.BOMB_TARGET_SCORE;
               gridManager.bombProduction = true;
          }
          else
          {
               gridManager.bombProduction = false;
          }
     }

     public void Play()
     {
          Time.timeScale = 1;
          gridManager.SetState(Toolbox.States.InitState);
          menuPanel.SetActive(false);

     }

     public void Exit()
     {
          Application.Quit();
     }
     public void GameOver()
     {

          menuPanel.SetActive(false);
          gameOver.SetActive(true);
          gridManager.SetState(Toolbox.States.MenuState);
          Time.timeScale = 0;

     }


     public void Retry()
     {
          SceneManager.LoadScene(0);
     }
}
