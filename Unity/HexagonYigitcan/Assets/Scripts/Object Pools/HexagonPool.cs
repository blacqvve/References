using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolbox;
using UnityEngine;

public class HexagonPool : Singleton<HexagonPool>
{
     [SerializeField]
     private GameObject hPrefab;
     [SerializeField]
     private GameObject bPrefab;

     private int preCreateCount;
     private bool expand = true;

     GridManager gridManager;
     [SerializeField]
     private int preCreateBombCount;

     [SerializeField]
     private List<GameObject> pooledObjects;

     public List<GameObject> PooledObjects
     {
          get { return pooledObjects; }
     }

     // creating our initial objects for pooling
     void Start()
     {
          gridManager = GridManager.Instance;
          preCreateCount =gridManager.gridWidth *gridManager.gridHeight;
          pooledObjects = new List<GameObject>();
          for (int i = 0; i < preCreateCount; i++)
          {
               InstantiateAndAddPool(false);
          }
          for (int i = 0; i < preCreateBombCount; i++)
          {
               InstantiateAndAddPool(true);
          }
     }

     public GameObject GetPooledObject(bool bomb)
     {
          if (bomb)
          {
               var bombGo= pooledObjects.FirstOrDefault(x => x.activeInHierarchy!=true && x.GetComponent<Hexagon>().GetType() == typeof(Bomb));
               if (Helpers.CheckNull(bombGo))
               {
                    if (expand)
                    {
                         return InstantiateAndAddPool(true);
                    } else
                         return null;
               }
               return bombGo;
          }
          for (int i = 0; i < pooledObjects.Count; i++)
          {
               if (!pooledObjects[i].activeInHierarchy)
               {
                         return pooledObjects[i];
               }
          }
          if (expand)
          {

               return InstantiateAndAddPool(false); ;
          }
          else
          {
               return null;
          }
     }

     GameObject InstantiateAndAddPool(bool b)
     {
          if (b)
          {
               GameObject bomb = Instantiate(bPrefab,gameObject.transform);
               bomb.SetActive(false);
               pooledObjects.Add(bomb);
               return bomb;
          }
          else
          {
               GameObject obj = (GameObject)Instantiate(hPrefab,gameObject.transform);
               obj.SetActive(false);
               pooledObjects.Add(obj);
               return obj;
          }
     }
}
