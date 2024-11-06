using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.Shooter
{
    public struct ShootRequest : IComponent
    {
        public Shooter shooter;
        public float delay;
    }
}