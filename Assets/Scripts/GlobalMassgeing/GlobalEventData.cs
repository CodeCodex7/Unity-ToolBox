using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventList", menuName = "Oliver's Toolbox/GlobalEventList")]
public class GlobalEventData : ScriptableObject
{

    public List<MessageData> Events
    {
        get { return m_EventData; }
        set { m_EventData = value; }
    }

    [SerializeField]
    private List<MessageData> m_EventData;


    public List<MessageData> testData
    {
        get { return m_testData; }
        set { m_testData = value; }
    }

    [SerializeField]
    private List<MessageData> m_testData;



}
