using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bomb : Hexagon
{

     [SerializeField]
     private int timer;
     [SerializeField]
     private Text text;


     private void Awake()
     {
          gridManager = GridManager.Instance;
          text.text = timer.ToString();
     }
     private void OnEnable()
     {
          gridManager = GridManager.Instance;
          timer = HexMetrics.BOMB_TIMER;
          text.text = timer.ToString();
          gridManager.BombTick += HandleTimer;
     }
     private void HandleTimer()
     {

               timer--;
               if (timer == 0)
               {
               MenuManager.Instance.GameOver();
               }
               text.text = timer.ToString();  
     }

     private void OnDisable()
     {
          gridManager.BombTick -= HandleTimer;
          gridManager = null;
     }

}
