using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Toolbox
{
     public struct CellNeighbours
     {
          Hexagon UP;
          Hexagon DOWN;
          Hexagon UPL;
          Hexagon UPR;
          Hexagon DL;
          Hexagon DR;
     }

     public struct CellGroup
     {
          public Hexagon A;
          public Hexagon B;
          public Hexagon C;

          public Hexagon[] NeighbourGroup;

          public CellGroup(Hexagon a, Hexagon b, Hexagon c)
          {
               A = a;
               B = b;
               C = c;
               NeighbourGroup = new Hexagon[3] { A, B, C };
          }
     }
}