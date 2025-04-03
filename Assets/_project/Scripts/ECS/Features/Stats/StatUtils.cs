using _project.Scripts.ECS.Features.Stats.EnergyGenRate;
using _project.Scripts.ECS.Features.Stats.MovementSpeed;
using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.Stats
{
    public static class StatUtils
    {
        public static void CreateStatModAddRequest(World world, Entity entity, StatMod mod, string statId)
        {
            var requestEntity = world.CreateEntity();

            var statIdToLower = statId.ToLower();

            switch (statIdToLower)
            {
                case "damage":
                    
                    break;
                case "speed":
                    ref var movementSpeedAddModRequest = ref requestEntity.AddComponent<MovementSpeedStatAddModRequest>();
                    movementSpeedAddModRequest.StatMod = mod;
                    movementSpeedAddModRequest.Target = entity;
                    break;
                case "aim_radius":
                    
                    break;
                case "max_health":
                    
                    break;
                case "attack_speed":
                    
                    break;
                case "shoot_power":
                    
                    break;
                case "energy_capacity":
                    
                    break;
                case "damage_decrease":
                    
                    break;
                case "energy_generation_rate":
                    ref var energyGenRateAddModRequest = ref requestEntity.AddComponent<EnergyGenRateStatAddModRequest>();
                    energyGenRateAddModRequest.StatMod = mod;
                    energyGenRateAddModRequest.Target = entity;
                    break;
            }
        }

        public static void CreateStatModRemoveRequest(World world, Entity entity, StatMod mod, string statId)
        {
            var requestEntity = world.CreateEntity();

            var statIdToLower = statId.ToLower();

            switch (statIdToLower)
            {
                case "damage":
                    
                    break;
                case "speed":
                    ref var movementSpeedRemoveModRequest = ref requestEntity.AddComponent<MovementSpeedStatRemoveModRequest>();
                    movementSpeedRemoveModRequest.StatMod = mod;
                    movementSpeedRemoveModRequest.Target = entity;
                    break;
                case "aim_radius":
                    
                    break;
                case "max_health":
                    
                    break;
                case "health":
                    
                    break;
                case "attack_speed":
                    
                    break;
                case "energy":
                    
                    break;
                case "shoot_power":
                    
                    break;
                case "energy_capacity":
                    
                    break;
                case "damage_decrease":
                    
                    break;
                case "energy_generation_rate":
                    ref var energyGenRateRemoveModRequest = ref requestEntity.AddComponent<EnergyGenRateStatRemoveModRequest>();
                    energyGenRateRemoveModRequest.StatMod = mod;
                    energyGenRateRemoveModRequest.Target = entity;
                    break;
            }
        }
    }
}