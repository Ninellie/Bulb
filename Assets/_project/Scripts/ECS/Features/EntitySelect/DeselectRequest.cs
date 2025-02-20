using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.EntitySelect
{
    public struct DeselectRequest : IComponent
    {
        public Entity DeselectedEntity;
    }
}