using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace O_UnityToolbox
{
    /// <summary>
    /// Basic Service Locator Pattern
    /// </summary>
    public static class Services
    {

        public static Dictionary<Type, object> services = new Dictionary<Type, object>();

        /// <summary>
        /// Try to get Service 
        /// </summary>
        /// <typeparam name="T"> Generic type </typeparam>
        /// <returns>Service of type T </returns>
        public static T Resovle<T>() where T : class
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
        public static void Registar(object o)
        {
            services.Add(o.GetType(), o);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ObjectType"></param>
        public static void DeRegistar(Type ObjectType)
        {
            if (services.ContainsKey(ObjectType))
            {
                services.Remove(ObjectType);
            }
        }
    }
}
