using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SocialPlatforms.Impl;

public class GridManager : Singleton<GridManager>
{
     [Range(5,11)]
     public int gridWidth;
     [Range(5, 13)]
     public int gridHeight;

     //Actions for listeners
     #region Actions

     public Action<int> OnPlayerScore;
     public Action OnMoveMade;
     public Action<SoundTypes> PlaySound;
     public Action BombTick;
     #endregion


     //Properties for handling get; set;
     #region Properties

     private List<Hexagon> bombs;
     public List<Hexagon> Bombs
     {
          get { return bombs; }
          set { bombs = value; }
     }


     private CellGroup selectedGroup;

     public CellGroup SelectedGroup
     {
          get { return selectedGroup; }
          set { selectedGroup = value; }
     }

     [SerializeField]
     private List<Color> colorList = new List<Color>();
     public List<Color> ColorList
     {
          get { return colorList; }
          set { colorList = value; }
     }

     Hexagon[,] hexagons;
     public Hexagon[,] Hexagons
     {
          get { return hexagons; }
     }

     private bool[,] logicalGrid;
     public bool[,] LogicalGrid
     {
          get { return logicalGrid; }
          set { logicalGrid = value; }
     }

     private States state;
     public States State
     {
          get { return state; }
     }

     #endregion
     [HideInInspector]
     public bool bombProduction = false;

     private new void  Awake()
     {
          SetState(States.MenuState);
          bombs = new List<Hexagon>();
          hexagons = new Hexagon[gridWidth, gridHeight];
          DrawLogicalGrid();


     }


     /// <summary>
     /// This method will set outline selected hexagon and neighbours
     /// </summary>
     /// <param name="_selected"></param>
     /// <param name="_hitPos"></param>
     public void MarkSelectedHexagonGroup(Hexagon _selected, Vector2 _hitPos)
     {
          if (Helpers.CheckNull(_selected))
               return;
          CellGroup group = _selected.SelectionGain(_hitPos);

          if (Helpers.CheckNull(group.B) || Helpers.CheckNull(group.C))
          {
               return;
          }
          selectedGroup = group;
          OutlineManager.Instance.AddOutline(selectedGroup);
          PlaySound?.Invoke(SoundTypes.Selection);

     }

     /// <summary>
     /// Filling out logical grid with open to place positions
     /// </summary>
     void DrawLogicalGrid()
     {
          if (state == States.InitState)
          {
               bool[,] missingCells = new bool[gridWidth, gridHeight];
               for (int i = 0; i < gridWidth; i++)
               {
                    for (int j = 0; j < gridHeight; j++)
                    {
                         missingCells[i, j] = true;
                    }
               }

               logicalGrid = missingCells;
               SetState(States.PlacingState);
               StartCoroutine(PlaceCellsOnGrid(missingCells, false));

          }
     }

     /// <summary>
     /// Place hexagons to open to place logical grid positions
     /// </summary>
     /// <param name="cells"></param>
     /// <param name="replace"></param>
     /// <returns></returns>
     IEnumerator PlaceCellsOnGrid(bool[,] cells, bool replace)
     {
          // an empty referance for previous placed hexagon.
          Hexagon prev = null;

          // Start placing hexagons in unity world
          if (state == States.PlacingState)
          {
               for (int x = 0; x < cells.GetLength(0); x++)
               {
                    for (int y = 0; y < cells.GetLength(1); y++)
                    {
                         // we are creating new hex if grid x,y open to place
                         if (cells[x, y])
                         {
                              //creating new hexagon
                              var hexagon = CreateNewHexagon(prev, x, y);
                              // replace is a flag for controlling color
                              if (!replace)
                                   prev = hexagon;
                         }

                         yield return new WaitForSeconds(HexMetrics.PLACE_DELAY);
                    }
               }
               yield return new WaitForSeconds(HexMetrics.PLACING_DELAY);
          }
          //Calculating matches after placing hexagons
          var mList = CalculateMatches(hexagons);

          if (mList.Count > 0)
          {
               SetState(States.ExplosionState);
               StartCoroutine(HandleMatch(mList));
          }
          SetState(States.InputState);
     }

     /// <summary>
     /// This methods rotates hexagons. Rotate information comes from input manager
     /// </summary>
     /// <param name="clockwise"></param>
     /// <returns></returns>
     public IEnumerator CheckRotate(bool clockwise)
     {

          List<Hexagon> matchList = new List<Hexagon>();
          // temporary group
          CellGroup tempGroup = selectedGroup;
          // Initialize our rotate command
          ICommand rotateCommand;
          for (int i = 0; i < selectedGroup.NeighbourGroup.Length; i++)
          {
               SetState(States.RotatingState);
               // create new rotate command for start rotating
               rotateCommand = new RotateCommand(clockwise);
               rotateCommand.Execute();
               // adding new command to command buffer. It can be used for later events
               CommandManager.Instance.AddCommand(rotateCommand);
               yield return new WaitForSeconds(HexMetrics.ROTATION_DELAY);

               // calculation for matches
               matchList = CalculateMatches(hexagons);

               if (matchList.Count > 0)
               {
                    OnMoveMade?.Invoke();
                    SetState(States.ExplosionState);
                    StartCoroutine(HandleMatch(matchList));
                    break;
               }
          }
          // if everything is compleated we are going to take our next input
          SetState(States.InputState);
     }

     /// <summary>
     /// This method will remove matched hexagons from game and open place in logical grid for missing hexagons
     /// </summary>
     /// <param name="matchList"></param>
     /// <returns></returns>
     public IEnumerator HandleMatch(List<Hexagon> matchList)
     {

          if (matchList.Count > 0 && state == States.ExplosionState)
          {
               // Sound and score delegates for listeners
               PlaySound?.Invoke(SoundTypes.Explosion);
               OnPlayerScore?.Invoke(HexMetrics.SCORE_MULTIPLIER * matchList.Count);
               foreach (var item in matchList)
               {
                    if (item.GetType()!=typeof(Bomb))
                    {
                         BombTick?.Invoke();
                    }
                    //Removing every hex from game logic
                    RemoveHexFromGame(item);

                    yield return new WaitForSeconds(HexMetrics.EXPLOSION_DELAY);
               }

               yield return new WaitForSeconds(HexMetrics.AFTER_EXPLOSION_DELAY);
               //this loop rearrange all hexes in correct positions
               for (int x = 0; x < logicalGrid.GetLength(0); x++)
               {
                    bool space = false;
                    int spaceY = 0;
                    var y =0;

                    while (y <= logicalGrid.GetLength(1)-1)
                    {
                         Hexagon h = hexagons[x, y];
                         if (space)
                         {
                              if (!Helpers.CheckNull(h))
                              {
                                   h.ChangeGridPosition(x, spaceY);
                                   logicalGrid[x, spaceY] = false;
                                   hexagons[x, y] = null;
                                   logicalGrid[x, y] = true;
                                   h.ChangeWorldPosition(Helpers.HexOffset(x, spaceY, Helpers.GetGridStartX(gridWidth)));
                                   space = false;
                                   y = spaceY;

                                   spaceY = 0;
                              }
                         }
                         else if (Helpers.CheckNull(h))
                         {
                              space = true;
                              if (spaceY==0)
                              {
                                   spaceY = y;
                              }
                         }
                         y++;
                    }
               }

               SetState(States.PlacingState);
               StartCoroutine(PlaceCellsOnGrid(logicalGrid, false));
          }
     }

     /// <summary>
     /// This method calculates every match on grid
     /// </summary>
     /// <param name="_hexagons"></param>
     /// <returns></returns>
     public List<Hexagon> CalculateMatches(Hexagon[,] _hexagons)
     {
          // this method can be improved later
          Hexagon cHex;
          Dictionary<NeighbourPos, Hexagon> currentNeighbours = new Dictionary<NeighbourPos, Hexagon>();
          Color cColor;

          List<Hexagon> matches = new List<Hexagon>();
          for (int x = 0; x < _hexagons.GetLength(0); x++)
          {
               for (int y = 0; y < _hexagons.GetLength(1); y++)
               {
                    if (!Helpers.CheckNull(_hexagons[x, y]))
                    {
                         if (!matches.Contains(_hexagons[x, y]))
                         {
                              cHex = _hexagons[x, y];
                              cColor = cHex.Color;
                              currentNeighbours = cHex.GetNeighbours();
                              // using linq for execute queries faster
                              currentNeighbours = currentNeighbours.Where(h => h.Value.Color == cColor).ToDictionary(h => h.Key, h => h.Value);
                              List<Hexagon> neighbourList = new List<Hexagon>();
                              if (currentNeighbours.Count >= 2)
                              {
                                   if (currentNeighbours.ContainsKey(NeighbourPos.DOWN))
                                   {
                                        neighbourList.Add(currentNeighbours[NeighbourPos.DOWN]);
                                        if (currentNeighbours.ContainsKey(NeighbourPos.DL))
                                        {
                                             neighbourList.Add(currentNeighbours[NeighbourPos.DL]);
                                        }
                                        if (currentNeighbours.ContainsKey(NeighbourPos.DR))
                                        {
                                             neighbourList.Add(currentNeighbours[NeighbourPos.DR]);
                                        }
                                   }
                                   else if ((currentNeighbours.ContainsKey(NeighbourPos.UP)))
                                   {
                                        neighbourList.Add(currentNeighbours[NeighbourPos.UP]);
                                        if (currentNeighbours.ContainsKey(NeighbourPos.UPL))
                                        {
                                             neighbourList.Add(currentNeighbours[NeighbourPos.UPL]);
                                        }
                                        if (currentNeighbours.ContainsKey(NeighbourPos.UPR))
                                        {
                                             neighbourList.Add(currentNeighbours[NeighbourPos.UPR]);
                                        }
                                   }
                              }
                              if (neighbourList.Count >= 2)
                              {
                                   neighbourList.Add(cHex);
                                   neighbourList.ForEach(hex =>
                                   {
                                        if (!matches.Contains(hex))
                                        {
                                             matches.Add(hex);
                                        }
                                   });
                              }
                         }
                    }
               }

          }
          return matches;
     }


     /// <summary>
     /// Removes selected hexagon from game logic and retuns to object pool for reuse.
     /// </summary>
     /// <param name="hex"></param>
     void RemoveHexFromGame(Hexagon hex)
     {
          if (!Helpers.CheckNull(hex))
          {
               if (hex.GetType() == typeof(Bomb))
                    bombs.Remove(hex);

               hexagons[hex.GridX, hex.GridY] = null;
               logicalGrid[hex.GridX, hex.GridY] = true;
               OutlineManager.Instance.RemoveOutlines();
               selectedGroup = new CellGroup(null, null, null);
               hex.gameObject.transform.parent = HexagonPool.Instance.gameObject.transform;
               hex.gameObject.SetActive(false);
          }
     }
     /// <summary>
     /// Take a hexagon from object pool and set its new properties for use
     /// </summary>
     /// <param name="_prev"></param>
     /// <param name="x"></param>
     /// <param name="y"></param>
     /// <returns></returns>
     Hexagon CreateNewHexagon(Hexagon _prev, int x, int y)
     {
          System.Random rnd = new System.Random();
          Vector2 offsetPosition, worldPosition, startPosition;
          float startX = Helpers.GetGridStartX(gridWidth);

          // calculate offset world position
          offsetPosition = Helpers.HexOffset(x, y, startX);

          // assing offset positions to world position
          worldPosition = new Vector3(offsetPosition.x, offsetPosition.y, 0);
          startPosition = new Vector2(worldPosition.x, worldPosition.y + 20f);
          GameObject hexGO;
          // we are taking bomb from pool if bomb flag is true
          if (bombProduction)
          {
               hexGO = HexagonPool.Instance.GetPooledObject(true);
               //add to bomb list
               bombs.Add(hexGO.GetComponent<Bomb>());
               //and close bomb flag
               bombProduction = false;
          }
          else
          {
               hexGO = HexagonPool.Instance.GetPooledObject(false);
               hexGO.SetActive(true);
          }
          // get a game object from our pool and use it on grid
          hexGO.transform.position = startPosition;

          hexGO.transform.parent = gameObject.transform.GetChild(0);

          hexGO.name = $"X:{x} , Y:{y}";

          hexGO.SetActive(true);

          var hexagon = hexGO.GetComponent<Hexagon>();
          //setting world position
          hexagon.ChangeWorldPosition(worldPosition);
          //setting grid position
          hexagon.ChangeGridPosition(x, y);
          //Set random color
          hexagon.SetColor(colorList[rnd.Next(0, 1000) % colorList.Count]);

          // add new hexagon to out hexagon grid for keeping track
          hexagons[x, y] = hexagon;

          // close spot for production logical grid
          logicalGrid[x, y] = false;

          //calculating color match on creating
          if (!Helpers.CheckNull(_prev))
          {
               while (hexagon.Color == _prev.Color)
               {
                    hexagon.SetColor(colorList[rnd.Next(0, 1000) % colorList.Count]);
               }
          }
          return hexagon;
     }

     /// <summary>
     /// Set new grid grid position
     /// </summary>
     /// <param name="x"></param>
     /// <param name="y"></param>
     /// <param name="h"></param>
     public void SetNewGridPosition(int x, int y, Hexagon h)
     {
          hexagons[x, y] = h;
     }


     /// <summary>
     /// Sets game state to placing and execute special logic if needed.
     /// </summary>
     public void SetState(States _state)
     {
          switch (_state)
          {
               case States.MenuState:
                    state = States.MenuState;
                    break;
               case States.InitState:
                    state = States.InitState;
                    DrawLogicalGrid();
                    break;
               case States.PlacingState:
                    state = States.PlacingState;
                    break;
               case States.MovingState:
                    state = States.MovingState;
                    break;
               case States.RotatingState:
                    state = States.RotatingState;
                    break;
               case States.ExplosionState:
                    state = States.ExplosionState;
                    break;
               case States.InputState:
                    state = States.InputState;
                    break;
               default:
                    break;
          }

     }


     /// <summary>
     /// Fill color list
     /// </summary>
     /// <returns></returns>
     List<Color> FillColorList()
     {
          List<Color> _colorList = new List<Color>();
          _colorList.Add(Color.red);
          _colorList.Add(Color.yellow);
          _colorList.Add(Color.green);
          _colorList.Add(Color.blue);
          _colorList.Add(Color.magenta);
          return _colorList;
     }

}