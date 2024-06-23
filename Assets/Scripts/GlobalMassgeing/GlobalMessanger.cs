using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMessanger : MonoService<GlobalMessanger>
{
    public GlobalEventData eventData;
    delegate void EventCallback(MessageData Data);

    Dictionary<string, int> NametoID = new Dictionary<string, int>();
    //Dictionary<int, List<Ilistner>> Events;
    Dictionary<int, List<Action>> EventActions;
    Dictionary<int, List<EventCallback>> DelegateEventActions;

    // Start is called before the first frame update
    void Start()
    {
        RegisterService();
        Initialistion();
    }


    private void OnDestroy()
    {
        UnregisterService();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Subscribe(int ID,Ilistner Listner)
    {
      
    }

    void Broadcast(int ID,MessageData data)
    {
        foreach (var item in DelegateEventActions[ID])
        {
            item.Invoke(data);
        }
    }

    void Initialistion()
    {

    }

    void CreateEvents()
    {

    }

}

public interface Ilistner
{
    me

    public void Event();
}

public class MessageData
{
    public string EventName; //Human Readable
    public int EventID;
    public object Data;
    public Type Type;

    public MessageData(int Id, string Name,Type type)
    {
        EventID = Id;
        EventName = Name;
        Type = type;
    }
}