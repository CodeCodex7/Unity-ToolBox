using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic Service Locator Pattern
/// </summary>
public static class Services
{

    #region Public functions

    /// <summary>
    /// Dictionary of services.
    /// </summary>
    private static Dictionary<Type, object> services = new Dictionary<Type, object>();

    /// <summary>
    /// Stored actions to take when service is available.
    /// </summary>
    private static Dictionary<Type, Action> StoredActions = new Dictionary<Type, Action>();

    /// <summary>
    /// Try to get Service 
    /// </summary>
    /// <typeparam name="T"> Generic type </typeparam>
    /// <returns>Service of type T </returns>
    public static T Resolve<T>() where T : class
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
    /// Registar Service.
    /// </summary>
    /// <typeparam name="T"> Type of service to registar. </typeparam>
    /// <param name="o"> Object to registar. </param>
    public static bool Registar<T>(object o)
    {
        Type typeParameterType = typeof(T);

        if (services.ContainsKey(typeParameterType))
        {
            Debug.LogError(string.Format("Service {0} all ready registated, Cannot have duplicated services", typeParameterType));
            return false;
        }


        services.Add(typeParameterType, o);

        try
        {
            if (StoredActions.ContainsKey(typeParameterType))
            {
                CallAction(typeParameterType);
            }
        }
        catch //(Exception e)
        {

        }

        Debug.Log(string.Format("Service {0} registered successfully", typeParameterType));
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ObjectType"></param>
    public static void UnRegistar<T>()
    {

        Type typeParameterType = typeof(T);

        if (services.ContainsKey(typeParameterType))
        {
            services.Remove(typeParameterType);
            Debug.Log(string.Format("Removed {0} from Service List", typeParameterType));
        }
    }

    /// <summary>
    /// Resovle Action once service becomes available.
    /// </summary>
    /// <typeparam name="T">Type of service.</typeparam>
    /// <param name="action">Action to call when sevice available. use lamba or method. </param>
    public static void ResolveWhenValid<T>(Action action)
    {
        Type typeParameterType = typeof(T);

        if (services.ContainsKey(typeParameterType))
        {
            action();
        }
        else
        {
            StoredActions.Add(typeParameterType, action);
        }

    }

    #endregion

    #region Internal Functions

    private static void CallAction(Type typeParameterType)
    {
        StoredActions[typeParameterType].Invoke();
        StoredActions.Remove(typeParameterType);

    }
    #endregion

}
