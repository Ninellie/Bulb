using System.Collections.Generic;
using _project.Scripts.ECS.Features.Aiming;
using _project.Scripts.ECS.Features.Blocks;
using _project.Scripts.ECS.Features.Damage;
using _project.Scripts.ECS.Features.EnergyFeature.EnergyProduction;
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
            stats = GetShooterStats(entity, stats);
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
            ref var container = ref entity.GetComponent<EnergyContainer>(out var exist);
            if (!exist) return stats;
            var energy = $"{container.CurrentAmount:F2}/{container.MaximumAmount}";
            stats.Add("Energy", energy);
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
            ref var generator = ref entity.GetComponent<Generator>(out var exist);
            if (!exist) return stats;
            var generationRate = generator.EnergyProductionAmount.Value / generator.BaseCooldown.Value;
            stats.Add("Generation rate", $"{generationRate:F2}");
            return stats;
        }

        private static Dictionary<string, string> GetDamage(Entity entity, Dictionary<string, string> stats)
        {
            ref var damageDealer = ref entity.GetComponent<DamageDealer>(out var exist);
            if (!exist) return stats;
            stats.Add("Damage", damageDealer.Amount.ToString());
            return stats;
        }

        private static Dictionary<string, string> GetShooterStats(Entity entity, Dictionary<string, string> stats)
        {
            ref var aiming = ref entity.GetComponent<Shooter.Shooter>(out var exist);
            if (!exist) return stats;
            var attackRate = 1 / aiming.Cooldown; 
            stats.Add("Attack rate", $"{attackRate:F2}");
            stats.Add("Energy cost per shoot", aiming.Cost.ToString());
            return stats;
        }
        
        private static Dictionary<string, string> GetAimingRadius(Entity entity, Dictionary<string, string> stats)
        {
            ref var aiming = ref entity.GetComponent<Aiming.Aiming>(out var exist);
            if (!exist) return stats;
            stats.Add("Aiming radius", $"{aiming.AimingRadius:F1}");
            return GetAimedState(entity, stats);
        }
        
        private static Dictionary<string, string> GetAimedState(Entity entity, Dictionary<string, string> stats)
        {
            entity.GetComponent<Aimed>(out var exist);
            stats.Add("Aimed", exist ? "True" : "False");
            return stats;
        }
    }
}