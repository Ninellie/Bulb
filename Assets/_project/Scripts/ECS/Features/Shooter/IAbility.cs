namespace _project.Scripts.ECS.Features.Shooter
{
    internal interface IAbility
    {
        public float Cooldown { get; set; }
        public float Cost { get; set; }
    }
}