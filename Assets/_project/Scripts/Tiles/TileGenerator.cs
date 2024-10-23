using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class GenerationRule
{
    [SerializeField] private TileBase tile;
    [SerializeField] private Vector2Int generateFrom;
    [SerializeField] private Vector2Int notGenerateFrom;
    [SerializeField] private int priority;

    public TileBase Tile => tile;

    public Vector2Int GenerateFrom => generateFrom;

    public Vector2Int NotGenerateFrom => notGenerateFrom;

    public int Priority => priority;
}

public class TileGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private GenerationRule[] rules;
    
    [SerializeField] private int groundHeight;

    private static readonly Vector2Int HalfScreenReferenceSize = new(240 , 135);

    private void Start()
    {
        var leftWorld = new Vector3(-HalfScreenReferenceSize.x, -HalfScreenReferenceSize.y);
        var rightWorld = new Vector3(HalfScreenReferenceSize.x, -HalfScreenReferenceSize.y);
        
        var leftCell = tilemap.WorldToCell(leftWorld);
        var rightCell = tilemap.WorldToCell(rightWorld);

        var dif = rightCell.x - leftCell.x;

        for (int i = 0; i < dif; i++)
        {
            for (int j = 0; j < groundHeight; j++)
            {
                var tile = GetTile((Vector2Int)leftCell);
                
                tilemap.SetTile(leftCell, tile);

                leftCell.Set(leftCell.x, leftCell.y + 1, leftCell.z);
            }
            leftCell.Set(leftCell.x + 1, leftCell.y - groundHeight, leftCell.z);
        }
        
        var bulbCell = new Vector3Int(leftCell.x - dif / 2, leftCell.y + groundHeight + 2, leftCell.z); // Почему 2 а не мильён
        
        tilemap.SetTile(bulbCell, GetTile((Vector2Int)bulbCell));
    }

    private TileBase GetTile(Vector2Int vector2Int)
    {
        var correctRules = new List<GenerationRule>();

        foreach (var rule in rules)
        {
            var y = vector2Int.y;
            
            if (y >= rule.NotGenerateFrom.y)
            {
                continue;
            }
            
            if (y < rule.GenerateFrom.y)
            {
                continue;
            }
            
            // todo доделать x
            correctRules.Add(rule);
        }

        if (correctRules.Count == 0)
        {
            return null;
        }
        var highestPriority = correctRules.Max(r => r.Priority);
        
        return correctRules.First(r => r.Priority == highestPriority).Tile;
    }
}