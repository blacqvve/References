using UnityEngine;

public static class HexMetrics
{
     #region constants

     public const float OUTER_RADIUS = 0.5f;
     public const float INNER_RADIUS = OUTER_RADIUS * 0.866025404f;
     public const float HEXAGON_SPAWN_HEIGHT = 8f;
     public const float HEXAGON_SPEED = 11f;
     public const float TOUCH_TRESHOLD = 5f;
     public const float PLACING_DELAY = 0.25f;
     public const float EXPLOSION_DELAY = 0.03f;
     public const float AFTER_EXPLOSION_DELAY = 0.03f;
     public const float ROTATION_DELAY = 0.5f;
     public const float OFFSET_X = 0.4f;
     public const float OFFSET_Y = 0.45f;
     public const float PLACE_DELAY = 0.002f;
     public const float GRID_DECREASE_VALUE = -1.5f;
     public const float OFFSET_FOR_MAX_GRID_WIDTH = 0.0275f;
     public const int BOMB_TARGET_SCORE = 1000;
     public const int SCORE_MULTIPLIER = 5;
     public const int BOMB_TIMER = 7;

     #endregion

     public static Vector2[] corners = {
    new Vector2(0f, OUTER_RADIUS),
    new Vector2(INNER_RADIUS,  0.5f * OUTER_RADIUS),
    new Vector2(INNER_RADIUS, -0.5f * OUTER_RADIUS),
    new Vector2(0f, -OUTER_RADIUS),
    new Vector2(-INNER_RADIUS, -0.5f * OUTER_RADIUS ),
    new Vector2(-INNER_RADIUS, 0.5f * OUTER_RADIUS)
  };
}