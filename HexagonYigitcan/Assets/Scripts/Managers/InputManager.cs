using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Toolbox;
using UnityEditor;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
     GridManager gridManager;
     public bool clokwise = true;
     Vector3 touchPos;
     void Start()
     {
          gridManager = GridManager.Instance;
     }

     void Update()
     {
          // check game state for input handling
          if (gridManager.State == States.InputState)
          {
               // editor controls
               if (Application.platform == RuntimePlatform.WindowsEditor)
               {
                    if (Input.GetMouseButtonDown(0))
                    {
                         RaycastMouse();
                    }
                    CheckMouseRotation();
                    if (Input.touchCount > 0)
                    {
                         HandleTouch();
                         CheckTouchRotate();

                    }
               }
               else if (Application.platform == RuntimePlatform.Android)
               {
                    if (Input.touchCount > 0)
                    {

                         HandleTouch();
                         CheckTouchRotate();
                    }

               }

          }
     }

     void HandleTouch()
     {
          var touch = Input.GetTouch(0);
          Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touch.position, 0);
          Vector3 touchPos = new Vector3(worldPosition.x, worldPosition.y, 0);
          Collider2D cellCol = Physics2D.OverlapPoint(touchPos);
          var selectedCell = cellCol != null ? cellCol.gameObject.GetComponent<Hexagon>() : null;

          if (touch.phase == TouchPhase.Ended)
          {
               if (!Helpers.CheckNull(selectedCell))
               {

                    OutlineManager.Instance.RemoveOutlines();

                    gridManager.MarkSelectedHexagonGroup(selectedCell, touchPos);
               }
               else
               {
                    OutlineManager.Instance.RemoveOutlines();
               }
          }


     }
     void RaycastMouse()
     {
          OutlineManager.Instance.RemoveOutlines();
          Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
          RaycastHit2D info = Physics2D.Raycast(mousePos, Vector2.zero);
          if (info.collider != null)
          {
               Vector2 hitPoint = info.point;
               var selectedHexagon = info.collider.gameObject.GetComponent<Hexagon>();
               //Mark new group
               gridManager.MarkSelectedHexagonGroup(selectedHexagon, hitPoint);

          }
          else
          {
               OutlineManager.Instance.RemoveOutlines();
          }

     }

     void CheckTouchRotate()
     {
          if (Input.GetTouch(0).phase == TouchPhase.Began)
          {
               touchPos = Input.GetTouch(0).position;

          }
          if (Input.GetTouch(0).phase == TouchPhase.Moved && !Helpers.CheckNull(gridManager.SelectedGroup.A))
          {
               print(touchPos);
               Vector2 touchCurrentPosition = Input.GetTouch(0).position;
               print(touchCurrentPosition);

               //Check finger movement amount is higher than treshold
               if (Mathf.Abs(touchCurrentPosition.x - touchPos.x) > HexMetrics.TOUCH_TRESHOLD || Mathf.Abs(touchCurrentPosition.y - touchPos.y) > HexMetrics.TOUCH_TRESHOLD)
               {
                    if (touchPos.x < touchCurrentPosition.x)
                    {
                         StartCoroutine(gridManager.CheckRotate(clokwise));
                    }
                    else if (touchPos.x > touchCurrentPosition.x)
                    {
                         StartCoroutine(gridManager.CheckRotate(!clokwise));
                    }
               }
          }
     }

     void CheckMouseRotation()
     {


          if (!Helpers.CheckNull(gridManager.SelectedGroup.A))
          {
               if (Input.GetKeyDown(KeyCode.A))
               {
                    StartCoroutine(gridManager.CheckRotate(false));
               }
               if (Input.GetKeyDown(KeyCode.D))
               {
                    StartCoroutine(gridManager.CheckRotate(true));
               }
          }

     }
}


