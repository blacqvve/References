using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{

     public static class Helpers
     {
          /// <summary>
          /// Simple method for null check to every type
          /// </summary>
          /// <param name="o"></param>
          /// <returns></returns>
          public static bool CheckNull(object o)
          {
               return o == null ? true : false;
          }

          /// <summary>
          /// Get Grid Start Position In Unity World
          /// </summary>
          /// <returns></returns>
          public static float GetGridStartX(int gridWidth)
          {
               return gridWidth / 2 * -HexMetrics.OFFSET_X;
          }

          /// <summary>
          /// Calculate perfect grid postions with offset values
          /// </summary>
          /// <param name="x"></param>
          /// <param name="y"></param>
          /// <param name="startX"></param>
          /// <returns></returns>
          public static Vector2 HexOffset(float x, float y, float startX)
          {
               Vector2 position = Vector2.zero;

               if (x % 2 == 1)
               {
                    position.x = startX + (x * HexMetrics.OFFSET_X);
                    position.y = y * HexMetrics.OFFSET_Y - (HexMetrics.OFFSET_Y / 2) + HexMetrics.GRID_DECREASE_VALUE;
               }
               else
               {
                    position.x = startX + (x * HexMetrics.OFFSET_X);
                    position.y = y * HexMetrics.OFFSET_Y + HexMetrics.GRID_DECREASE_VALUE;
               }

               return position;
          }
     }

}