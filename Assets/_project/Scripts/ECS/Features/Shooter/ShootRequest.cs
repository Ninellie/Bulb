using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.Shooter
{
    public class ShootRequest
    {
        public Entity ShooterEntity { get; }
        public float Delay { get; set; }

        public ShootRequest(Entity shooterEntity, float delay)
        {
            ShooterEntity = shooterEntity;
            Delay = delay;
        }
    }
}