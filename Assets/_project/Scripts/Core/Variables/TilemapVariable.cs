﻿using UnityEngine;
using UnityEngine.Tilemaps;

namespace _project.Scripts.Core.Variables
{
    [CreateAssetMenu(fileName = "New Tilemap Variable", menuName = "Variables/Tilemap", order = 51)]
    public class TilemapVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string developerDescription = "";
#endif
        public Tilemap value;

        public void SetValue(Tilemap value)
        {
            this.value = value;
        }

        public void SetValue(TilemapVariable value)
        {
            this.value = value.value;
        }
        
        public static implicit operator Tilemap(TilemapVariable variable)
        {
            return variable.value;
        }
    }
}