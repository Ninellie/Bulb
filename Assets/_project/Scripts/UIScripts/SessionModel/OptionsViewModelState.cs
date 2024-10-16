using UnityEngine;

namespace UIScripts.SessionModel
{
    public class OptionsViewModelState : IViewModelState
    {
        public ViewModelStateType Name => ViewModelStateType.Options;
        private readonly ViewModel _viewModel;
        private readonly IViewController _viewController;
        
        public OptionsViewModelState(ViewModel viewModel, IViewController viewController)
        {
            _viewModel = viewModel;
            _viewController = viewController;
        }
        
        public void PauseScreen()
        {
            _viewModel.ChangeState(ViewModelStateType.Pause);
            _viewController.CloseOptions();
            _viewController.OpenPauseScreen();
        }
        
        public void Resume() => PauseScreen();
        public void Options() => Debug.LogWarning($"Attempt to {nameof(Options)} from {Name} state");
        public void LevelUp() => Debug.LogWarning($"Attempt to {nameof(LevelUp)} from {Name} state");
        public void Mutate()=> Debug.LogWarning($"Attempt to {nameof(Mutate)} from {Name} state");
        public void Win() => Debug.LogWarning($"Attempt to {nameof(Win)} from {Name} state");
        public void Lose() => Debug.LogWarning($"Attempt to {nameof(Lose)} from {Name} state");
    }
}