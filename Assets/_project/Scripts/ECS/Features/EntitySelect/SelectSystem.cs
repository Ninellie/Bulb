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
        private Stash<SelectRequest> _selectRequestStash;
        private Stash<DeselectRequest> _deselectRequestStash;
        
        private Stash<OnSelectEvent> _onSelectStash;
        private Stash<OnDeselectEvent> _onDeselectStash;
        
        public override void OnAwake()
        {
            _selectRequestStash = World.GetStash<SelectRequest>();
            _deselectRequestStash = World.GetStash<DeselectRequest>();
            
            _onSelectStash = World.GetStash<OnSelectEvent>();
            _onDeselectStash = World.GetStash<OnDeselectEvent>();
        }

        public override void OnUpdate(float deltaTime)
        {
            _onSelectStash.RemoveAll();
            _onDeselectStash.RemoveAll();
            
            foreach (ref var request in _selectRequestStash)
            {
                var entity = World.CreateEntity();
                
                ref var onSelectEvent = ref _onSelectStash.Add(entity);
                onSelectEvent.SelectedEntity = request.SelectedEntity;
            }
            
            _selectRequestStash.RemoveAll();
            
            foreach (ref var request in _deselectRequestStash)
            {
                var entity = World.CreateEntity();
                ref var onDeselectEvent = ref _onDeselectStash.Add(entity);
                onDeselectEvent.DeselectedEntity = request.DeselectedEntity;
            }
            
            _deselectRequestStash.RemoveAll();
        }
    }
}