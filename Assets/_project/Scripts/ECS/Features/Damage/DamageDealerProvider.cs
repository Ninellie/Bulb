using Scellecs.Morpeh.Providers;
using Unity.IL2CPP.CompilerServices;

namespace _project.Scripts.ECS.Features.Damage
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public sealed class DamageDealerProvider : MonoProvider<DamageDealer>
    {
    }
}