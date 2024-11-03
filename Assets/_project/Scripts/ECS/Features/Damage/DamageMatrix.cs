using System;
using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Damage
{
    [Serializable]
    [DeclareHorizontalGroup("Dealers")]
    public class Taker
    {
        [Group("Dealers")] [HideLabel] public string tag;
        [Group("Dealers")] [HideLabel] public string damage0;
        [Group("Dealers")] [HideLabel] public string damage1;
        [Group("Dealers")] [HideLabel] public string damage2;
        [Group("Dealers")] [HideLabel] public string damage3;
        [Group("Dealers")] [HideLabel] public string damage4;
        [Group("Dealers")] [HideLabel] public string damage5;
        [Group("Dealers")] [HideLabel] public string damage6;
        [Group("Dealers")] [HideLabel] public string damage7;
        [Group("Dealers")] [HideLabel] public string damage8;
        [Group("Dealers")] [HideLabel] public string damage9;
    }
    
    [Serializable]
    [CreateAssetMenu(menuName = "ECS/DataResources/" + nameof(DamageMatrix))]
    public class DamageMatrix : ScriptableObject
    {
        [HideLabel]
        [TableList(Draggable = false, HideAddButton = false, HideRemoveButton = false, AlwaysExpanded = false)]
        public List<DamageTakerData> damageTakers;
        
        [HideLabel]
        [TableList(Draggable = false, HideAddButton = false, HideRemoveButton = false, AlwaysExpanded = true)]
        public List<Taker> takers;

        private void OnValidate()
        {
            if (takers.Count < 10)
            {
                takers = new List<Taker>(10);
                takers.Clear();
                var title = new Taker
                {
                    tag = "Name",
                    damage0 = "Tag 0",
                    damage1 = "Tag 1",
                    damage2 = "Tag 2",
                    damage3 = "Tag 3",
                    damage4 = "Tag 4",
                    damage5 = "Tag 5",
                    damage6 = "Tag 6",
                    damage7 = "Tag 7",
                    damage8 = "Tag 8",
                    damage9 = "Tag 9"
                };
                
                takers.Add(title);
                
                for (var i = 0; i < 10; i++)
                {
                    takers.Add(new Taker());
                }
            
            }
            ValidateTags();
            ValidateMatrix();
        }
        
        public int GetDamage(string damageTakerTag, string damageDealerTag)
        {
            var damageTaker = damageTakers.FirstOrDefault(t => t.tag == damageTakerTag);
            
            if (damageTaker == null)
            {
                return 0;
            }
            
            var damageDealer = damageTaker.damageDealers.FirstOrDefault(d => d.tag == damageDealerTag);
            
            if (damageDealer == null)
            {
                return 0;
            }

            if (!damageDealer.interact)
            {
                return 0;
            }
            
            return damageDealer.damage;
        }

        private void ValidateTags()
        {
            // Все пустые теги превращаем в "NewTag"
            for (var i = 0; i < damageTakers.Count; i++)
            {
                if (string.IsNullOrEmpty(damageTakers[i].tag))
                {
                    damageTakers[i].tag = "New tag";
                }
                
                if (string.IsNullOrWhiteSpace(damageTakers[i].tag))
                {
                    damageTakers[i].tag = "New tag";
                }
            }

            // Повторяющиеся теги превращаем в "NewTag"
            for (var i = 0; i < damageTakers.Count; i++)
            {
                var equalsCount = 0;
                
                for (var j = 0; j < damageTakers.Count; j++)
                {
                    if (damageTakers[i].tag.Equals(damageTakers[j].tag))
                    { 
                        equalsCount++;
                    }
                    
                    if (equalsCount <= 1) continue;
                    
                    damageTakers[j].tag = "New tag";
                }
            }
            
            // Удаляем повторяющиеся "New tag", добавляя им порядковую цифру
            for (var i = 0; i < damageTakers.Count; i++)
            {
                
                for (var j = 0; j < damageTakers.Count; j++)
                {
                    var newTagsCount = 0;
                    for (var k = 0; k <= j; k++)
                    {
                        if (!damageTakers[k].tag.StartsWith("New tag")) continue;
                        damageTakers[j].tag = newTagsCount == 0 ? "New tag" : "New tag " + newTagsCount;
                        newTagsCount++;
                    }
                }
            }
        }

        private void ValidateMatrix()
        {
            ValidateTags();
            
            for (var i = 0; i < damageTakers.Count; i++)
            {
                var damageDealers = damageTakers[i].damageDealers;
                
                for (var j = 0; j < damageDealers.Count; j++)
                {
                    while (damageDealers.Count < damageTakers.Count)
                    {
                        damageDealers.Add(new DamageDealerData());
                    }
                    while (damageDealers.Count > damageTakers.Count)
                    {
                        damageDealers.RemoveAt(damageTakers.Count - 1);
                    }
                }
            }

            foreach (var damageTakerData in damageTakers)
            {
                for (var j = 0; j < damageTakerData.damageDealers.Count; j++)
                {
                    damageTakerData.damageDealers[j].tag = damageTakers[j].tag;
                }
            }
        }
    }
    
    [Serializable]
    public class DamageTakerData
    {
        public string tag;
        
        [HideLabel]
        [TableList(Draggable = false, HideAddButton = true, HideRemoveButton = true, AlwaysExpanded = true)]
        
        public List<DamageDealerData> damageDealers;
    }

    [Serializable]
    [DeclareHorizontalGroup("Damage dealers")]
    public class DamageDealerData
    { 
        [Group("Damage dealers")] public bool interact;
        [Group("Damage dealers")] [ReadOnly] public string tag; 
        [Group("Damage dealers")] [ShowIf("interact")] public int damage;
    }
}