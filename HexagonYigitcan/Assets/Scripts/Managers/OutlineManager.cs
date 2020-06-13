using System.Collections;
using System.Collections.Generic;
using Toolbox;
using UnityEngine;

public class OutlineManager : Singleton<OutlineManager>
{
     [SerializeField]
     private GameObject outlineObject;

     List<GameObject> outlines = new List<GameObject>();
   
     /// <summary>
     /// Construct outline for selected group
     /// </summary>
     /// <param name="group"></param>
     public void AddOutline(CellGroup group)
     {
          foreach (var item in group.NeighbourGroup)
          {
               var go = Instantiate(outlineObject, item.transform);
               go.name = $"Outline {item.GridX},{item.GridY}";
               outlines.Add(go);
          }
     }

     /// <summary>
     /// deconstruct all outlines
     /// </summary>
     public void RemoveOutlines()
     {
          outlines.ForEach(x => Destroy(x));
          GridManager.Instance.SelectedGroup = new CellGroup(null, null, null);
     }
}
