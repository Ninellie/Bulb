using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.EntitySelect
{
    public struct OnDeselectEvent : IComponent
    {
        public Entity DeselectedEntity;
    }
}