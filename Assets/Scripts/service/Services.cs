using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class Services : MonoBehaviour
{

    public Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static Services Instance;

    public void Awake()
    {
        Instance = this;
    }
    
    /// <summary>
    /// Try to get Service 
    /// </summary>
    /// <typeparam name="T"> Generic type </typeparam>
    /// <returns>Service of type T </returns>
    public T Resovle<T>() where T : class
    {
        Type typeParameterType = typeof(T);

        if (services.ContainsKey(typeParameterType))
        {
            return (T)services[typeParameterType];
        }

        Debug.LogError(string.Format("Cant Resovle service of type {0}", typeParameterType));
        return null;
    }

    /// <summary>
    /// Registar Service
    /// </summary>
    /// <param name="o"> Object to Registar </param>
    public void Registar(object o)
    {
        services.Add(o.GetType(), o);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ObjectType"></param>
    public void DeRegistar(Type ObjectType)
    {
        if(services.ContainsKey(ObjectType))
        {
            services.Remove(ObjectType);
        }
    }




}
