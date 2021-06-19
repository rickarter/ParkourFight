using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public Texture2D[] maps;
    public ColorMappings mappings;
    public GameObject levelController;

    private GameObject levelGroup;

    void Awake()
    {
        GenerateLevel(Random.Range(0, maps.Length));
    }

    public void GenerateLevel(int mapID)
    {
        if(levelGroup != null) Destroy(levelGroup);

        levelGroup = new GameObject();
        levelGroup.name = "Generated level";
        
        Instantiate(levelController, Vector3.zero, Quaternion.identity, levelGroup.transform);

        for(int x = 0; x < maps[mapID].width; x++)
        {
            for(int y = 0; y < maps[mapID].height; y++)
            {
                GenerateTile(x, y, mapID);
            }
        }
    }

    void GenerateTile(int x, int y, int mapID)
    {
        Color pixelColor = maps[mapID].GetPixel(x, y);

        if(pixelColor.a == 0) return;

        foreach(ColorToPrefab colorMapping in mappings.mappings)
        {
            if(colorMapping.color.Equals(pixelColor))
            {
                Vector2 position = new Vector2(x * 1.59f, y * 1.59f);
                if(colorMapping.prefab.CompareTag("Player") || colorMapping.prefab.CompareTag("LootBox"))
                {
                    GameObject gameObject = Instantiate(colorMapping.prefab, position, Quaternion.identity, levelGroup.transform);
                    gameObject.name = colorMapping.prefab.name;
                } 
                else
                    Instantiate(colorMapping.prefab, position, Quaternion.identity, levelGroup.transform);
            }
        }


    }
}
