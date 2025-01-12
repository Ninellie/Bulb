using _project.Scripts.Core.Variables.References;

namespace _project.Scripts.ECS.Features.Shooter
{
    internal interface IAbility
    {
        public FloatReference Cooldown { get; set; }
        public FloatReference Cost { get; set; }
    }
}