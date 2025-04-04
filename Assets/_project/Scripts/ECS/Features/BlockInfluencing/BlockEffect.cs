using System;
using System.Collections.Generic;
using _project.Scripts.ECS.Features.Stats;

namespace _project.Scripts.ECS.Features.BlockInfluencing
{
    [Serializable]
    public class BlockEffect
    {
        public string Id; // Название эффекта
        public List<string> AffectingBlockID; // На какой блок действует данный эффект
        public List<SingleStatEffect> StatMods; // Модификаторы, которые будут применены к блокам
    }
}