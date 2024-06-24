using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml.Linq;
using OpenCover.Framework.Model;

public class TextReplacment : MonoService<TextReplacment>
{
    public TextAsset TextFile;

    Dictionary<string,string> TextDict = new Dictionary<string,string>();

    private void Awake()
    {
        RegisterService();
        ProcessTextFile();
    }


    private void OnDestroy()
    {
        UnregisterService();
    }

    public string GetText(string Key)
    {
        if (TextDict.ContainsKey(Key))
        {
            return TextDict[Key];
        }
        else
        {
            Debug.Log(string.Format("Searched for key {0} -> not Found",Key));
            return "TEXT KEY NOT FOUND";
        }
    }


    void ProcessTextFile()
    {

        XDocument xdoc = XDocument.Parse(TextFile.text);
        var Words = from T in xdoc.Root.Elements()
                    select new TextData(T.Attribute("key").Value,T.Value);

        foreach (var W in Words)
        {
            TextDict.Add(W.m_key, W.m_Text);
            Debug.Log(string.Format("Value = {0} , Key = {1}",W.m_Text,W.m_key));
        }




    }
    private class TextData
    {
        public string m_key;
        public string m_Text;

        public TextData(string key,string Text)
        {
            m_key = key;
            m_Text = Text;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //ProcessTextFile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
