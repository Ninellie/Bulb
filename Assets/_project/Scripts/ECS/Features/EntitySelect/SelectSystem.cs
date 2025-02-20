using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;

namespace _project.Scripts.ECS.Features.EntitySelect
{
    /// <summary>
    ///  Обрабатывает Select и Deselect запросы и создаёт соответствующие события.
    ///  Время жизни событий - 1 кадр.
    /// </summary>
    public class SelectSystem : UpdateSystem
    {
        private Filter _selfSelectRequestFilter;
        private Filter _selfDeselectRequestFilter;
        
        private Stash<SelfSelectRequest> _selfSelectRequestStash;
        private Stash<SelfDeselectRequest> _selfDeselectRequestStash;
        
        private Stash<OnSelfSelectEvent> _onSelfSelectStash;
        private Stash<OnSelfDeselectEvent> _onSelfDeselectStash;
        
        public override void OnAwake()
        {
            _selfSelectRequestFilter = World.Filter.With<SelfSelectRequest>().Build();
            _selfDeselectRequestFilter = World.Filter.With<SelfDeselectRequest>().Build();
            
            _selfSelectRequestStash = World.GetStash<SelfSelectRequest>();
            _selfDeselectRequestStash = World.GetStash<SelfDeselectRequest>();
            
            _onSelfSelectStash = World.GetStash<OnSelfSelectEvent>();
            _onSelfDeselectStash = World.GetStash<OnSelfDeselectEvent>();
        }

        public override void OnUpdate(float deltaTime)
        {
            _onSelfSelectStash.RemoveAll();
            _onSelfDeselectStash.RemoveAll();
            
            foreach (var entity in _selfSelectRequestFilter)
            {
                _onSelfSelectStash.Add(entity);
            }
            
            _selfSelectRequestStash.RemoveAll();
            
            foreach (var entity in _selfDeselectRequestFilter)
            { 
                _onSelfDeselectStash.Add(entity);
            }
            
            _selfDeselectRequestStash.RemoveAll();
        }
    }
}