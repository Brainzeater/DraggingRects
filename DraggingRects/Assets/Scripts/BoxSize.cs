using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Stores the data about width and height of the box
[CreateAssetMenu(fileName = "BoxSize", menuName = "BoxSize")]
public class BoxSize : ScriptableObject
{
    public float width;
    public float height;
}