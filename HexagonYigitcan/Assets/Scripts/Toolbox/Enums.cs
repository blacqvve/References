using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
     public enum States
     {
          MenuState,
          InitState,
          PlacingState,
          MovingState,
          RotatingState,
          ExplosionState,
          InputState
     } 

     public enum SoundTypes
     {
          Explosion,
          Selection
     }
     public enum NeighbourPos
     {
          UP,
          DOWN,
          UPL,
          UPR,
          DL,
          DR
     }
}