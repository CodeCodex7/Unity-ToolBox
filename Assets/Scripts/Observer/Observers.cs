using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observers : MonoBehaviour
{


}

public class TestEvent : IObserverBase
{

    public int test;
    public int testBackup;

    public List<ISubsciberBase> Observers 
    {
        get { return m_Observers; }
        set { m_Observers.Add(value as ISubsciberBase); }
    }

    public List<ISubsciberBase> m_Observers = new List<ISubsciberBase>();

    public void AddSubscriber(ISubsciberBase subsciber)
    {
        m_Observers.Add(subsciber);
    }

    public void RemoveSubscriber(ISubsciberBase subsciber)
    {
        m_Observers.Remove(subsciber);
    }

    public void Notify()
    {
        if(test == testBackup)
        {
            foreach (var item in m_Observers)
            {
                item.UpdateSubscriber(this);
            }
            testBackup = test;
        }
    }
}

public class TestEventWatch : ISubsciberBase
{
    public void UpdateSubscriber(IObserverBase observerBase)
    {
        Debug.Log("Trigged");
    }
}

public interface IObserverBase 
{

    /// <summary>
    /// List of Subscriber listening to this observer
    /// </summary>
    public  List<ISubsciberBase> Observers { get; set; } 

    /// <summary>
    /// Add Subscriber to this observer
    /// </summary>
    /// <param name="subsciber"></param>
    public void AddSubscriber(ISubsciberBase subsciber);
    
    /// <summary>
    /// Remove Subscriber from this Observer
    /// </summary>
    /// <param name="subsciber"></param>
    public void RemoveSubscriber(ISubsciberBase subsciber);

    /// <summary>
    /// Trigger change, send out notification to all subscribers
    /// </summary>
    public void Notify();
}

public interface ISubsciberBase 
{
    /// <summary>
    /// Act on notfication from observer
    /// </summary>
    /// <param name="observerBase"></param>
    public void UpdateSubscriber(IObserverBase observerBase);
}