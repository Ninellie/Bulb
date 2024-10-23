using Core.Sets;
using TMPro;
using UnityEngine;

namespace EntityComponents.UnitComponents.EnemyComponents
{
    public class Target : MonoBehaviour
    {
        [SerializeField] private TMP_ColorGradient outlineColor;
        [SerializeField] private TargetRuntimeSet runtimeSet;
        [SerializeField] private bool isCurrent;
        
        public bool IsCurrent => isCurrent;
        public Transform Transform { get; private set; }
        
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            if (Transform == null) Transform = transform;
            if (_spriteRenderer != null) return;
            if (TryGetComponent(out SpriteRenderer sp))
            {
                _spriteRenderer = sp;
            }
        }

        private void OnBecameVisible()
        {
            runtimeSet.Add(this);
        }

        private void OnBecameInvisible()
        {
            runtimeSet.Remove(this);
            if (!isCurrent) return;
            RemoveFromCurrent();
        }

        public void TakeAsCurrent()
        {
            var mpb = new MaterialPropertyBlock();
            _spriteRenderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_Outline", 1f);
            mpb.SetColor("_OutlineColor", outlineColor.topLeft);
            _spriteRenderer.SetPropertyBlock(mpb);
            isCurrent = true;
        }

        public void RemoveFromCurrent()
        {
            var mpb = new MaterialPropertyBlock();
            _spriteRenderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_Outline", 0f);
            mpb.SetColor("_OutlineColor", outlineColor.topLeft);
            _spriteRenderer.SetPropertyBlock(mpb);
            isCurrent = false;
        }
    }
}