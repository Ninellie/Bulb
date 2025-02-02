using _project.Scripts.Core.Variables;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.ChitinPayment
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/Update/" + nameof(ChitinPaymentSystem))]
    public sealed class ChitinPaymentSystem : UpdateSystem
    {
        [SerializeField] private FloatVariable currentCurrency;

        private Filter _paymentRequestsFilter;
        private Stash<PaymentSuccess> _successStash;
        private Stash<PaymentFail> _failStash;
        private Stash<PaymentRequest> _requestStash;
        
        public override void OnAwake()
        {
            _paymentRequestsFilter = World.Filter.With<PaymentRequest>().Build();

            _successStash = World.GetStash<PaymentSuccess>();
            _failStash = World.GetStash<PaymentFail>();
        }

        public override void OnUpdate(float deltaTime)
        {
            _successStash.RemoveAll();
            _failStash.RemoveAll();
            
            foreach (var entity in _paymentRequestsFilter)
            {
                ref var request = ref entity.GetComponent<PaymentRequest>();
                
                if ((int)currentCurrency.value >= request.Cost)
                {
                    currentCurrency.value -= request.Cost;
                    _successStash.Add(entity);
                }
                else
                {
                    _failStash.Add(entity);
                }
            }
            
            _requestStash.RemoveAll();
        }
    }
}