using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Toolbox;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
     private Vector2 lerp;
     private bool move;
     protected GridManager gridManager;
     CellGroup selectionGroup;

     #region Properties

     private Dictionary<NeighbourPos, Hexagon> neighbours;
     public Dictionary<NeighbourPos, Hexagon> Neighbours
     {
          get { return neighbours; }
          set { neighbours = value; }
     }

     [SerializeField]
     private int gridX;
     public int GridX
     {
          get { return gridX; }
          set { gridX = value; }
     }
     [SerializeField]
     private int gridY;
     public int GridY
     {
          get { return gridY; }
          set { gridY = value; }
     }

     private Color color;
     public Color Color
     {
          get { return color; }
          set { color = value; }
     }

     #endregion


     private void OnEnable()
     {
          gridManager = GridManager.Instance;
          neighbours = new Dictionary<NeighbourPos, Hexagon>();
          neighbours = GetNeighbours();
     }
     void Update()
     {
          if (move)
          {
               float worldX = Mathf.Lerp(transform.position.x, lerp.x, Time.deltaTime * HexMetrics.HEXAGON_SPEED);
               float worldY = Mathf.Lerp(transform.position.y, lerp.y, Time.deltaTime * HexMetrics.HEXAGON_SPEED);
               transform.position = new Vector2(worldX, worldY);

               if (Vector3.Distance(transform.position, lerp) < 0.05f)
               {
                    transform.position = lerp;
                    move = false;
               }
          }
     }

     /// <summary>
     /// Returns neighbours
     /// </summary>
     /// <returns></returns>
     public Dictionary<NeighbourPos, Hexagon> GetNeighbours()
     {
          Dictionary<NeighbourPos, Hexagon> neighboursList = new Dictionary<NeighbourPos, Hexagon>();
          int gWidth = gridManager.gridWidth - 1;
          int gHeight = gridManager.gridHeight - 1;
          bool isOdd = gridX % 2 == 1;
          // collect neighbours 
          neighboursList.Add(NeighbourPos.UP, gridY != gHeight ? gridManager.Hexagons[gridX, gridY + 1] : null);
          neighboursList.Add(NeighbourPos.DOWN, gridY != 0 ? gridManager.Hexagons[gridX, gridY - 1] : null);
          neighboursList.Add(NeighbourPos.DL, gridX != 0 && gridY != 0 ? gridManager.Hexagons[gridX - 1, isOdd ? gridY - 1 : gridY] : null);
          neighboursList.Add(NeighbourPos.DR, gridX != gWidth && gridY != 0 ? gridManager.Hexagons[gridX + 1, isOdd ? gridY - 1 : gridY] : null);
          neighboursList.Add(NeighbourPos.UPR, gridX != gWidth && gridY != gHeight ? gridManager.Hexagons[gridX + 1, isOdd ? gridY : gridY + 1] : null);
          neighboursList.Add(NeighbourPos.UPL, gridX != 0 && gridY != gHeight ? gridManager.Hexagons[gridX - 1, isOdd ? gridY : gridY + 1] : null);

          //we are adding non null values to return dictionary 
          neighboursList = neighboursList.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);

          neighbours = neighboursList;

          return neighboursList;
     }

     /// <summary>
     /// Sets selection group with calculating collider hit position
     /// </summary>
     /// <param name="pos"></param>
     /// <returns></returns>
     public CellGroup SelectionGain(Vector2 pos)
     {
          Hexagon b = null, c = null;
          GetNeighbours();
          CellGroup group = new CellGroup(this, b, c);

          // down, downleft neighbours
          if (pos.x < this.transform.position.x && pos.y < this.transform.position.y)
          {
               neighbours.TryGetValue(NeighbourPos.DOWN, out b);
               neighbours.TryGetValue(NeighbourPos.DL, out c);
               group = new CellGroup(this, b, c);
          }
          //up , up left neighbours
          else if (pos.x < this.transform.position.x && pos.y > this.transform.position.y)
          {
               neighbours.TryGetValue(NeighbourPos.UP, out c);
               neighbours.TryGetValue(NeighbourPos.UPL, out b);
               group = new CellGroup(this, b, c);
          }
          //up , up right neighbours
          else if (pos.x > this.transform.position.x && pos.y > this.transform.position.y)
          {
               neighbours.TryGetValue(NeighbourPos.UP, out b);
               neighbours.TryGetValue(NeighbourPos.UPR, out c);
               group = new CellGroup(this, b, c);
          }
          //down , down right neighbours
          else if (pos.x > this.transform.position.x && pos.y < this.transform.position.y)
          {
               neighbours.TryGetValue(NeighbourPos.DOWN, out c);
               neighbours.TryGetValue(NeighbourPos.DR, out b);
               group = new CellGroup(this, b, c);
          }
          selectionGroup = group;
          return group;
     }

     /// <summary>
     /// Change world position for interpolation movement
     /// </summary>
     /// <param name="newPosition"></param>
     public void ChangeWorldPosition(Vector2 newPosition)
     {
          lerp = newPosition;
          move = true;
     }

     /// <summary>
     /// Change grid position in hexagon parameters and grid manager
     /// </summary>
     /// <param name="x"></param>
     /// <param name="y"></param>
     public void ChangeGridPosition(int x, int y)
     {
          gridX = x;
          gridY = y;
          gridManager.SetNewGridPosition(gridX, gridY, this);
     }

     /// <summary>
     /// set sprite color
     /// </summary>
     /// <param name="c"></param>
     public void SetColor(Color c)
     {
          GetComponent<SpriteRenderer>().color = c; color = c;
     }
     private void OnDisable()
     {
          name = "PooledObject";
          gridManager = null;
          neighbours = null;
     }
}