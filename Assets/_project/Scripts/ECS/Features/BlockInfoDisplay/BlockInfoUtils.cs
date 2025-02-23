using System.Collections.Generic;
using _project.Scripts.ECS.Features.Blocks;
using _project.Scripts.ECS.Features.EnergyFeature.EnergyReserving;
using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.BlockInfoDisplay
{
    public static class BlockInfoUtils
    {
        public static Dictionary<string, string> GetBlockStats(Entity entity)
        {
            var stats = new Dictionary<string, string>();
            stats = GetName(entity, stats);
            stats = GetEnergy(entity, stats);
            stats = GetCost(entity, stats);
            stats = GetGenerationRate(entity, stats);
            stats = GetDamage(entity, stats);
            stats = GetAttackRate(entity, stats);
            stats = GetAimingRadius(entity, stats);
            return stats;
        }

        private static Dictionary<string, string> GetName(Entity entity, Dictionary<string, string> stats)
        {
            ref var name = ref entity.GetComponent<BlockName>(out var exist);
            
            if (!exist) return stats;
            
            stats.Add("Name", name.Name);
            
            return stats;
        }

        private static Dictionary<string, string> GetEnergy(Entity entity, Dictionary<string, string> stats)
        {
            ref var energy = ref entity.GetComponent<EnergyContainer>(out var exist);

            if (!exist) return stats;

            var maxEnergy = energy.MaximumAmount;
            var currentEnergy = energy.CurrentAmount;

            stats.Add("Energy", currentEnergy + " / " + maxEnergy);

            return stats;
        }

        private static Dictionary<string, string> GetCost(Entity entity, Dictionary<string, string> stats)
        {
            ref var chitinCost = ref entity.GetComponent<BlockChitinCost>(out var exist);
            
            if (!exist) return stats;
            
            stats.Add("Chitin Cost", chitinCost.Cost.ToString());
            
            return stats;
        }

        private static Dictionary<string, string> GetGenerationRate(Entity entity, Dictionary<string, string> stats)
        {
            return stats;
        }

        private static Dictionary<string, string> GetDamage(Entity entity, Dictionary<string, string> stats)
        {
            return stats;
        }

        private static Dictionary<string, string> GetAttackRate(Entity entity, Dictionary<string, string> stats)
        {
            return stats;
        }
        
        private static Dictionary<string, string> GetAimingRadius(Entity entity, Dictionary<string, string> stats)
        {
            return stats;
        }
    }
}