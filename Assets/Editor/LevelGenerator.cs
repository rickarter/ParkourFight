using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class LevelGenerator : EditorWindow
{
    private Texture2D map;
    private ColorMappings mappings;

    private GameObject levelGroup;

    [MenuItem("Window/Level Generator")]
    public static void ShowWindow()
    {
        GetWindow<LevelGenerator>("Level Generator");
    }

    void OnGUI()
    {
        map = (Texture2D)EditorGUILayout.ObjectField("Map", map, typeof(Texture2D), false);
        mappings = (ColorMappings)EditorGUILayout.ObjectField("ColorMappings", mappings, typeof(ColorMappings), false);

        if(GUILayout.Button("Generate from texture"))
        {
            GenerateLevel(map);
        }
        if(GUILayout.Button("Destroy level"))
        {
            DestroyImmediate(levelGroup);
        }
    }

    void GenerateLevel(Texture2D map)
    {
        levelGroup = new GameObject();
        levelGroup.name = "Generated level";

        for(int x = 0; x < map.width; x++)
        {
            for(int y = 0; y < map.height; y++)
            {
                GenerateTile(x, y);
            }
        }
    }

    void GenerateTile(int x, int y)
    {
        Color pixelColor = map.GetPixel(x, y);

        if(pixelColor.a == 0) return;

        foreach(ColorToPrefab colorMapping in mappings.mappings)
        {
            if(colorMapping.color.Equals(pixelColor))
            {
                Vector2 position = new Vector2(x * 1.59f, y * 1.59f);
                if(colorMapping.prefab.CompareTag("Player") || colorMapping.prefab.CompareTag("LootBox"))
                    Instantiate(colorMapping.prefab, position, Quaternion.identity);
                else
                    Instantiate(colorMapping.prefab, position, Quaternion.identity, levelGroup.transform);
            }
        }


    }
}
