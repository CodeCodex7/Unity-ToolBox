using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMessanger : MonoService<GlobalMessanger>
{
    public GlobalEventData eventData;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Broadcast()
    {

    }

}

public class MessageData
{
    public string EventName; //Human Readable
    public int EventID;
    public object Data;
    public Type Type;
    public string TestData;

    public MessageData(int Id, string Name,Type type)
    {
        EventID = Id;
        EventName = Name;
        Type = type;
    }
}