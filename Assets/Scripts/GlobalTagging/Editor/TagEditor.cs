using UnityEngine;
using System.Collections;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;

[CustomEditor(typeof(Tag))]
public class TagEditor : Editor
{

    SerializedProperty damageProp;
    SerializedProperty armorProp;
    SerializedProperty gunProp;

    void OnEnable()
    {
        // Setup the SerializedProperties.
        damageProp = serializedObject.FindProperty("damage");
        armorProp = serializedObject.FindProperty("armor");
        gunProp = serializedObject.FindProperty("gun");
    }


    public override void OnInspectorGUI()
    {
        Tag myTarget = (Tag)target;


        EditorGUILayout.IntSlider(damageProp, 0, 100, new GUIContent("Damage"));

    }
}
