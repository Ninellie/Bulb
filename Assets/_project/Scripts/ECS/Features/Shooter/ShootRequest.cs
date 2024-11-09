namespace _project.Scripts.ECS.Features.Shooter
{
    public class ShootRequest
    {
        public Shooter Shooter { get; }
        public float Delay { get; set; }

        public ShootRequest(Shooter shooter, float delay)
        {
            Shooter = shooter;
            Delay = delay;
        }
    }
}