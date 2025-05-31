using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(JapanAbstractDungeonGenerator),true)]

public class JapanCityDungeonGeneratorEditor : Editor
{
    JapanAbstractDungeonGenerator generator;

    private void Awake()
    {
        generator = (JapanAbstractDungeonGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Create Dungeon"))
        {
            generator.generateDungeon();
        }
    }
}
