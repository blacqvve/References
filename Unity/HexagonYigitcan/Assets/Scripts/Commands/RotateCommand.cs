using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text.RegularExpressions;
using Toolbox;
using UnityEngine;

public class RotateCommand : ICommand
{
     bool clockwise;
     GridManager gManager;
     public RotateCommand(bool _clockwise)
     {
          gManager = GridManager.Instance;
          clockwise = _clockwise;
     }
     // arrays for holding data
     Hexagon[] hexes = new Hexagon[3];
     int[] logicalPosX = new int[3];
     int[] logicalPosY = new int[3];
     Vector2[] worldPos = new Vector2[3];
     public void Execute()
     {
          // fill temp data arrays
          for (int i = 0; i < hexes.Length; i++)
          {
               hexes[i] = gManager.SelectedGroup.NeighbourGroup[i];
               logicalPosX[i] = hexes[i].GridX;
               logicalPosY[i] = hexes[i].GridY;
               worldPos[i] = hexes[i].transform.position;
          }

          if (clockwise)
          {
               // C to A
               Rotate(logicalPosX[0], logicalPosY[0], worldPos[0], hexes[2]);
               // A to B
               Rotate(logicalPosX[1], logicalPosY[1], worldPos[1], hexes[0]);
               // B to C
               Rotate(logicalPosX[2], logicalPosY[2], worldPos[2], hexes[1]);

          }
          else
          {
               // C to B
               Rotate(logicalPosX[1], logicalPosY[1], worldPos[1], hexes[2]);
               // A to C
               Rotate(logicalPosX[2], logicalPosY[2], worldPos[2], hexes[0]);
               // B to A
               Rotate(logicalPosX[0], logicalPosY[0], worldPos[0], hexes[1]);
          }

     }

     /// <summary>
     /// This method redue last command
     /// </summary>
     public void Undue()
     {
          //TODO: Implement this feature later

     }

     /// <summary>
     /// Change grid and world position of a Hexagon
     /// </summary>
     /// <param name="x"></param>
     /// <param name="y"></param>
     /// <param name="pos"></param>
     /// <param name="a"></param>
     void Rotate(int x, int y, Vector2 pos, Hexagon a)
     {
          a.ChangeGridPosition(x, y);
          a.ChangeWorldPosition(pos);
     }
}
