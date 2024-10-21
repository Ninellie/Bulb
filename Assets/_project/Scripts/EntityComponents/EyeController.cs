using UnityEngine;

namespace EntityComponents
{
    public class EyeController : MonoBehaviour
    {
        [SerializeField] private Transform core;
        
        // Максимум сдвига от центра
        [SerializeField] private float deviationMax;
        
        // Скорость реакции глаза
        [SerializeField] private float deviationPower;

        private Transform _eye;
        
        private Vector2 _previousPosition;
        
        private void Awake()
        {
            _eye = transform;
        }

        private void Start()
        {
            _previousPosition = transform.position;
        }

        private void FixedUpdate()
        {
            // Текущая позиция записана
            var currentPosition = (Vector2)core.position;
            
            // Разница между позициями между двумя fixed кадрами, это расстояние пройдённое между кадрами, оно же - скорость объекта
            var posDif = currentPosition - _previousPosition;
            
            posDif.Normalize();
            
            posDif *= deviationPower;
            
            var nextLocalPos = (Vector2)_eye.localPosition + posDif;

            var finalPosition = Vector2.ClampMagnitude(nextLocalPos, deviationMax);
            
            _eye.localPosition = finalPosition;
            
            _previousPosition = currentPosition;
        }
    }
}