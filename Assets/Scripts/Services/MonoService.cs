using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoService<T> : MonoBehaviour
{
    /// <summary>
    /// Is this a duplicate of a service all ready registered
    /// </summary>
    private bool m_Duplicate;

    /// <summary>
    /// Register service
    /// </summary>
    public void RegisterService()
    {   
        if(!Services.Registar<T>(this))
        {   
            m_Duplicate = true;
            DestroyImmediate(this);
        }
    }

    /// <summary>
    /// Un-Register Service
    /// </summary>
    public void UnregisterService()
    {
        if (!m_Duplicate)
        {
            Services.UnRegistar<T>();
        }
    }

}
