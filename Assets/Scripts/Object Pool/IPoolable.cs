using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    public int PoolIdentity { get; set; }
    public bool InPool { get; set; }

}
