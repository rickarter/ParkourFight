using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorMappings", menuName = "LevelGenerator/ColorMappings")]
public class ColorMappings : ScriptableObject
{
    public List<ColorToPrefab> mappings;
}

[System.Serializable]
public class ColorToPrefab
{
    public Color color;
    public GameObject prefab;
}
