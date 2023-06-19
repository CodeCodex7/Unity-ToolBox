using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

using Random = UnityEngine.Random;

public class ObjectPool : MonoService<ObjectPool>
{
    private Dictionary<string, Object_Pool> PoolManager = new Dictionary<string, Object_Pool>();

    public GameObject PoolRoot;

    private void Awake()
    {
        RegisterService();
    }

    private void Start()
    {
        GameObject A = new GameObject("TestObject");
        CreatePool(A, 100, "TestObject1");
        PoolCount("TestObject1");
        PoolCount("TestObject");

      
    }


    public void Update()
    {
        if(Input.GetKeyUp(KeyCode.O))
        {
            for (int i = 0; i < 100; i++)
            {
                InstantiateFromPool("TestObject1", new Vector3(Random.Range(0, 100), Random.Range(0, 100), Random.Range(0, 100)), Quaternion.identity);
            }
        }
    }

    private void OnDestroy()
    {
        UnregisterService();
    }

    void CreatePool(GameObject PoolObject, int Ammount, string PoolIdentity)
    {

        if(PoolRoot == null)
        {
            PoolRoot = new GameObject();
            PoolRoot.name = "Object Pool Root";
        }

        Object_Pool NewPool = new Object_Pool(PoolIdentity, 0, PoolObject, PoolRoot);
        NewPool.GeneratePool(50);
        PoolManager.Add(PoolIdentity, NewPool);

        Debug.Log(string.Format("Created pool {0} of Object {1}",PoolIdentity,PoolObject.name));
    }

    void ModifyPool(string key,int Amount)
    {
        if(Amount <= -1)
        {

        }
        else if(Amount >= 1)
        {
            PoolManager[key].GeneratePool(Amount);
        }
    }

    public GameObject InstantiateFromPool(string PoolKey, Vector3 position, Quaternion rotation, Transform parent)
    {
        var Object = Retrive(PoolKey);
        Object.transform.position = position;
        Object.transform.rotation = rotation;
       
        Object.SetActive(true);
        return Object;
    }

    public GameObject InstantiateFromPool(string PoolKey, Vector3 position, Quaternion rotation)
    {
        var Object = Retrive(PoolKey);
        Object.transform.position = position;
        Object.transform.rotation = rotation;
        Object.transform.SetParent(null);
        Object.SetActive(true);
        return Object;
    }

    public GameObject InstantiateFromPool(string PoolKey, Transform parent)
    {
        var Object = Retrive(PoolKey);
        Object.transform.SetParent(parent);
        Object.SetActive(true);
        return Object;
    }

    public GameObject InstantiateFromPool(string PoolKey)
    {
        var Object = Retrive(PoolKey);
        Object.SetActive(true);
        Object.transform.SetParent(null);
        return Object;
    }

    private GameObject Retrive(string PoolKey)
    {

        if(!PoolManager.ContainsKey(PoolKey))
        {
            Debug.LogError(string.Format("No Pool with that key"));
        }
        else
        {
            for (int i = 0; i < PoolManager[PoolKey].Pool.Count; i++)
            {
                if (PoolManager[PoolKey].Pool[i].GetComponent<Poolable>().InPool)
                {
                    PoolManager[PoolKey].Pool[i].GetComponent<Poolable>().InPool = false;
                    return PoolManager[PoolKey].Pool[i];
                }

                Debug.Log(string.Format("Pool Empty!"));
            }            
        }
        return null;
    }

    /// <summary>
    /// Returns object to object pool, checks to see if it is a poolable object
    /// </summary>
    /// <param name="PoolableObject"></param>
    public void ReturnToPool(GameObject PoolableObject)
    {
        if(PoolableObject.GetComponent<Poolable>())
        {
            var PoolObject = PoolableObject.GetComponent<Poolable>();
            PoolObject.InPool = true;
            PoolObject.transform.position = Vector3.zero;
            PoolableObject.SetActive(false);
        }
    }

    /// <summary>
    /// Returns object to object pool
    /// </summary>
    /// <param name="PoolanleObject">Poolable componet of the gameobject</param>
    public void ReturnToPool(Poolable PoolanleObject)
    {

        PoolanleObject.InPool = true;
        PoolanleObject.transform.position = Vector3.zero;
        PoolanleObject.gameObject.SetActive(false);

    }


    public int PoolCount(string PoolKey)
    {

        int PoolCount = 0;

        if (!PoolManager.ContainsKey(PoolKey))
        {
            Debug.LogError(string.Format("No Pool with that key"));
            return 0;
        }
        else
        {
            for (int i = 0; i < PoolManager[PoolKey].Pool.Count; i++)
            {
                if (PoolManager[PoolKey].Pool[i].GetComponent<Poolable>().InPool)
                {
                    PoolCount++;
                }               
            }
        }

        return PoolCount;
    }

    private class Object_Pool
    {
        public string PoolName;
        public int KEY;
        public Action<GameObject> OnAllocate;
        public Action<GameObject> OnDeallocate;
        public Action<GameObject> OnCreate;

        public GameObject PoolItem;
        public GameObject PoolRoot;
        public List<GameObject> Pool;

        public Object_Pool(string m_poolName,int key,GameObject poolItem,GameObject poolRoot)
        {
            PoolName = m_poolName;
            KEY = key;
            PoolItem = poolItem;
            PoolRoot = poolRoot;

            Pool = new List<GameObject>();
        }

        public void GeneratePool(int ammount)
        {

            var Root = new GameObject(string.Format("{0} Pool Root", PoolItem.name));
            Root.transform.SetParent(PoolRoot.transform);

            for (int i = Pool.Count; i < ammount; i++)
            {
                var Item = Instantiate(PoolItem,Vector3.zero,Quaternion.identity);
                Item.SetActive(false);
                Item.name = string.Format("{0} {1}", PoolItem.name, i);

                Item.transform.SetParent(Root.transform);

                Poolable P = Item.AddComponent<Poolable>();
                P.PoolPostion = i;
                P.PoolIdentity = PoolName;
                P.InPool = true;
                Pool.Add(Item);

            }
        }

        public void RemoveFromPool(int ammount)
        {
            for (int i = 0; i < ammount; i++)
            {
                Pool[i].GetComponent<Poolable>().MarkedforDestruction = true;
            }
        }

        public void PurgePool()
        {

            Debug.Log(string.Format("Pool at {0} Items", Pool.Count));

            List<GameObject> Remove = new List<GameObject>();

            for (int i = 0; i < Pool.Count; i++)
            {
                if (Pool[i].GetComponent<Poolable>().MarkedforDestruction)
                {
                    Remove.Add(Pool[i]);
                }
            }

            foreach (var PoolItem in Remove)
            {
                Destroy(PoolItem.gameObject);
            }

            Pool.TrimExcess();
            Debug.Log(string.Format("Pool Purged now at {0} Items", Pool.Count));
        }

    }

}





