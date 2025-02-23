using System;
using Scellecs.Morpeh;
using TMPro;

namespace _project.Scripts.ECS.Features.BlockInfoDisplay
{
    [Serializable]
    public struct BlockInfoDisplay : IComponent
    {
        public TextMeshProUGUI Text;
    }
}