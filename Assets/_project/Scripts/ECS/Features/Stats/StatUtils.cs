using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.Stats
{
    public static class StatUtils
    {
        public static void CreateStatModificationRequest(World world, Entity entity, StatMod mod, string statId)
        {
            var requestEntity = world.CreateEntity();

            var statIdToLower = statId.ToLower();


            switch (statIdToLower)
            {
                case "damage":
                    
                    break;
                case "speed":
                    
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
                    
                    break;
            }
        }
    }
}