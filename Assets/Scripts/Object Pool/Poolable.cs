using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable : MonoBehaviour 
{
    [SerializeField]
    public string PoolIdentity = "";
    [SerializeField]
    public int PoolPostion = 0;
    [SerializeField]
    public bool InPool = true;

    public bool MarkedforDestruction = false;

    private void OnDestroy()
    {
        if(!MarkedforDestruction)
        {
            Debug.Log(string.Format("Poolable Object being permanent Destoyed, it was not marked for destruction"));
        }
    }

    /// <summary>
    /// Used to return object back to it pool
    /// </summary>
    public void ReturnToPool()
    {
        Services.Resolve<ObjectPool>().ReturnToPool(this);
    }


}
