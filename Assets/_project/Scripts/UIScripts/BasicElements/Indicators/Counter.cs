using System;
using _project.Scripts.Core.Variables.References;
using TMPro;
using UnityEngine;

namespace UIScripts.BasicElements.Indicators
{
    public enum CounterFormat
    {
        LabelCurrentSlashMax,
        CurrentSlashMax,
        LabelCurrentOnly,
        CurrentOnly,
        LabelOnly,
        DoNotShow
    }

    public enum ValueFormat
    {
        D,
        D1,
        D2,
        D3,
        F,
        F1,
        F2,
        F3
    }

    public enum PercentFormat
    {
        Flat,
        Sigh
    }

    [AddComponentMenu("UI/Indicator/Сounter")]
    public class Counter : MonoBehaviour
    {
        [Header("Reserve settings")]
        [Header("Label")]
        [SerializeField] private string _counterName = "Simple counter";
        [Space]
        [Header("Value settings")]
        [SerializeField] protected FloatReference _value;
        [SerializeField] protected FloatReference _maximumValue;
        [Space]
        [Header("Counter text settings")]
        [SerializeField] private CounterFormat _counterFormat;
        [SerializeField] private ValueFormat _valueFormat;
        [SerializeField] private ValueFormat _maxValueFormat;
        [SerializeField] private bool _inPercent;
        [SerializeField] private PercentFormat _percentFormat;
        [Space]
        [Header("Counter text")]
        [SerializeField] private TMP_Text _text;

        private void Start()
        {
            UpdateCounter();
        }
        
        public virtual void UpdateCounter()
        {
            UpdateLabel();
        }

        private void OnValidate()
        {
            UpdateCounter();
        }

        public void Update()
        {
            UpdateLabel();
        }
        
        private void UpdateLabel()
        {
            var currentValue = GetValue();

            var textToSet = _counterFormat switch
            {
                CounterFormat.LabelCurrentSlashMax => $"{_counterName} {currentValue}/{GetMaxValue()}",
                CounterFormat.CurrentSlashMax => $"{currentValue}/{GetMaxValue()}",
                CounterFormat.LabelCurrentOnly => $"{_counterName} {currentValue}",
                CounterFormat.CurrentOnly => $"{currentValue}",
                CounterFormat.LabelOnly => $"{_counterName}",
                CounterFormat.DoNotShow => $"",
                _ => throw new ArgumentOutOfRangeException()
            };
            _text.SetText(textToSet);
        }

        protected int GetPercentCurrentValue()
        {
            if (!_maximumValue.useConstant && _maximumValue.variable == null)
            {
                Debug.unityLogger.LogWarning($"UI", $"You are using the maximum value as a variable in the counter, but you have not set the variable in inspector.", this);
                return 0;
            }
            if (!_value.useConstant && _value.variable == null)
            {
                Debug.unityLogger.LogWarning($"UI", $"You are using the value as a variable in the counter, but you have not set the variable in inspector.", this);
                return 0;
            }
            if (_value.Value == 0) return 0;
            if (_maximumValue.Value == 0) return 0;
            var percent = _maximumValue.Value / 100;
            var currentPercent = _value.Value / percent;
            return (int)currentPercent;
        }

        private string GetMaxValue()
        {
            if (_maximumValue == null) return "";
            if (!_maximumValue.useConstant && _maximumValue.variable == null)
            {
                Debug.unityLogger.LogWarning($"UI", $"You are using the maximum value as a variable in the counter, but you have not set the variable in inspector.", this);
                return "";
            }
            if (_inPercent)
            {
                var maxPercentValue = "100";
                if (_percentFormat == PercentFormat.Sigh)
                {
                    maxPercentValue += "%";
                }
                return maxPercentValue;
            }

            var value = _maximumValue.Value;

            var maxValue = _maxValueFormat switch
            {
                ValueFormat.D => $"{(int)value:D}",
                ValueFormat.D1 => $"{(int)value:D1}",
                ValueFormat.D2 => $"{(int)value:D2}",
                ValueFormat.D3 => $"{(int)value:D3}",
                ValueFormat.F => $"{value:F}",
                ValueFormat.F1 => $"{value:F1}",
                ValueFormat.F2 => $"{value:F2}",
                ValueFormat.F3 => $"{value:F3}",
                _ => throw new ArgumentOutOfRangeException()
            };

            return maxValue;
        }

        private string GetValue()
        {
            if (!_value.useConstant && _value.variable == null)
            {
                Debug.unityLogger.LogWarning($"UI", $"You are using the value as a variable in the counter, but you have not set the variable in inspector.", this);
                return "";
            }

            var value = 0f;

            if (_value != null)
            {
                if (_inPercent)
                { 
                    value = GetPercentCurrentValue();
                }
                else
                {
                    value = _value.Value;
                }
            }
            
            var currentValue = _valueFormat switch
            {
                ValueFormat.D => $"{(int)value:D}",
                ValueFormat.D1 => $"{(int)value:D1}",
                ValueFormat.D2 => $"{(int)value:D2}",
                ValueFormat.D3 => $"{(int)value:D3}",
                ValueFormat.F => $"{value:F}",
                ValueFormat.F1 => $"{value:F1}",
                ValueFormat.F2 => $"{value:F2}",
                ValueFormat.F3 => $"{value:F3}",
                _ => throw new ArgumentOutOfRangeException()
            };

            if (_inPercent && _percentFormat == PercentFormat.Sigh)
            {
                currentValue += "%";
            }

            return currentValue;
        }
    }
}