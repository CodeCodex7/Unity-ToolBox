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

    public T Resovle<T>()
    {
        Type typeParameterType = typeof(T);

        if (services.ContainsKey(typeParameterType))
        {
            return (T)services[typeParameterType];
        }

        return default;
    }

    public void Registar(object o)
    {
        services.Add(o.GetType(), o);
    }

    public void DeRegistar(Type ObjectType)
    {
        if(services.ContainsKey(ObjectType))
        {
            services.Remove(ObjectType);
        }
    }




}
