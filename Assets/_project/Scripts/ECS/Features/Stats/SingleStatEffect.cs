using System;

namespace _project.Scripts.ECS.Features.Stats
{
    [Serializable]
    public struct SingleStatEffect
    {
        public string StatId;
        public StatMod Mod;
    }
}