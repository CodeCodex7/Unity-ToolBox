using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(GlobalEventData))]
public class GlobalEventlistEditor : Editor
{
    public override void OnInspectorGUI()
    {
        
        GlobalEventData data = (GlobalEventData)target;
        GUILayout.Label("List of Events");

        if (data.testData != null)
        {

            for (int i = 0; i < data.testData.Count; i++)
            {
                DisplayElement(i, data.testData[i]);
            }

        }


        if (GUILayout.Button("Add Event"))
        {
            TestPopulate(data);
        }

        ClearData(data);


        DrawDefaultInspector();

    }


    public void DisplayElement(int i, MessageData Data)
    {
        GUILayout.Label(Data.EventName);
    }

    public void ClearData(GlobalEventData Data)
    {
        if (GUILayout.Button("Clear All Data in List"))
        {
            Data.testData.Clear();
            EditorUtility.SetDirty(Data);
            AssetDatabase.SaveAssets();
        }
    }

    public void CreateData(GlobalEventData Data)
    {
        if (GUILayout.Button("Clear All Data"))
        {
            if (Data.testData != null)
            {

            }
            else
            {
                Data.testData = new List<MessageData>();
            }
        }
    }

    public void TestPopulate(GlobalEventData Data)
    {

        if (Data.testData == null)
        {
            Data.testData = new List<MessageData>();
        }

        for (int i = Data.testData.Count; i < Data.testData.Count + 5; i++)
        {
            //Data.Events.Add(new MessageData(i,Time.frameCount.ToString()));
            Data.testData.Add(new MessageData(i, "Name"));
        }        
        EditorUtility.SetDirty(Data);
        AssetDatabase.SaveAssets();
    }


}
