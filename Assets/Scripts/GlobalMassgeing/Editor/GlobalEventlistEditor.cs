using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEditor.Timeline;
using UnityEngine;
using System.Reflection;
using System.Linq;

[CustomEditor(typeof(GlobalEventData))]
public class GlobalEventlistEditor : Editor
{

    string LibaryLocation;
    GlobalEventData data;
    public void OnValidate()
    {
        
    }

    public void OnEnable()
    {
        data = (GlobalEventData)target;
    }

    public override void OnInspectorGUI()
    {
        
        data = (GlobalEventData)target;
        var Text = GUILayout.TextField(LibaryLocation);
        LibaryLocation = Text;
        Debug.Log(LibaryLocation);
        Header();

        DisplayData(data);
        AddData(data);
        ClearData(data);
        DrawDefaultInspector();

    }

    void Header()
    {
        var HeaderStyle = new GUIStyle();
        HeaderStyle.fontSize = 16;
        HeaderStyle.alignment = TextAnchor.MiddleCenter;
        HeaderStyle.normal.textColor = Color.white;
        HeaderStyle.fontStyle = FontStyle.Bold;

        GUILayout.Label("List of Events", HeaderStyle);
    }
    public void DisplayData(GlobalEventData data)
    {
        if (data.testData != null)
        {

            for (int i = 0; i < data.testData.Count; i++)
            {
                DisplayElement(i, data.testData[i]);
            }

        }
    }
    public void DisplayElement(int i, MessageData Data)
    {
        GUILayout.BeginHorizontal() ;

        #region Name
        var TextStyle = GUI.skin.textArea;
        TextStyle.fixedHeight = 20;
        TextStyle.fixedWidth = 300;
        TextStyle.normal.textColor = Color.white;
        
        var text = GUILayout.TextField(Data.EventName,50,TextStyle);
        Data.EventName = text;

        GUILayout.Space(10);
        #endregion

        #region ID
        var IntStyle = GUI.skin.textField;
        IntStyle.fixedHeight = 20;
        IntStyle.fixedWidth = 40;
        TextStyle.normal.textColor = Color.white;

        var ID = EditorGUILayout.IntField("", Data.EventID,IntStyle);
        Data.EventID = ID;

        GUILayout.Space(10);
        #endregion

        #region Type





        var Content = new GUIContent(Data.Type.ToString());
        GenericMenu menu = new GenericMenu();

        //Debug.Log(Application.dataPath);

        //Assembly myAssembly = Assembly.ReflectionOnlyLoadFrom(string.Format("{0}\\Assembly-CSharp.dll", LibaryLocation));

        var ValueType = from T in System.AppDomain.CurrentDomain.GetAssemblies()
                        select T.GetTypes();



        ////

        foreach (var valueType in ValueType)
        {
            menu.AddItem(new GUIContent(valueType.ToString()), false, dostuff, valueType);
        }


        void dostuff(object parameter)
        {
            Content.text = parameter as string;
            Data.Type = parameter as Type;
            Debug.Log(parameter);
        }


        if (EditorGUILayout.DropdownButton(Content, FocusType.Keyboard))
        {
            menu.ShowAsContext();
        };
        
        #endregion

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

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

    public void AddData(GlobalEventData data)
    {
        if (GUILayout.Button("Add Event"))
        {
            TestPopulate(data);
        }
    }

    public void TestPopulate(GlobalEventData Data)
    {



        if (Data.testData == null)
        {
            Data.testData = new List<MessageData>();
        }

        int C1 = Data.testData.Count;
        int c2 = C1 + 5;
        for (int i = C1; i < c2 + 5; i++)
        {
            //Data.Events.Add(new MessageData(i,Time.frameCount.ToString()));
           Data.testData.Add(new MessageData(i, "OnEventNamedSomethingLongforTesting",typeof(object)));
           
        }        
        EditorUtility.SetDirty(Data);
        AssetDatabase.SaveAssets();
    }


}
