using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObject/Item_ScriptableObj")]
public class Item_ScriptableObj : ScriptableObject
{
    // Start is called before the first frame update
    [SerializeField] private Color color = Color.red;

    [SerializeField] private Sprite sprite;

    [SerializeField] private int value;

    [Header("Object Name")]
    [SerializeField] private string _name = string.Empty;
    [SerializeField] private UInt16 ID = 0; //0 signifies empty

    public GameObject equipPrefab;

    public UInt16 getID()
    {
        return ID;
    }

    public Sprite getSprite()
    {
        return sprite;
    }

    public Color getColor() { return color; }

    public string getName() { return _name; }

    public int getValue() { return value; }
    public GameObject getPrefab() { return equipPrefab;}
}
