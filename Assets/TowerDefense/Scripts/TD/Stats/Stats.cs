using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

namespace Giacomo
{
    public class Stats
    {
        [ShowInInspector]
        public Dictionary<string, Stat> stats {  get; private set; }

        public Stat this[string s] => stats[s];
        public Stats()
        {
            stats = new Dictionary<string, Stat>();
        }

        public void AddStat(string name, Stat stat)
        {
            if (stats.ContainsKey(name))
                Debug.LogError("duplicate stat: " + name + ": " + stat);

            stats.Add(name, stat);
        }
        public void AddStat(string name, float baseValue, float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            if (stats.ContainsKey(name))
                Debug.LogError("duplicate stat: " + name + ": " + baseValue);

            AddStat(name, new Stat(name, baseValue, minValue, maxValue));
        }

        public void AddModifier(string modifierName, string statName, float add = 0, float multiply = 1, bool keepHighestIfDuplicate = true)
            => stats[statName].AddModifier(modifierName, add, multiply, keepHighestIfDuplicate);

        public bool HasModifier(string modifierName, string statName)
            => stats[statName].HasModifier(modifierName);

        public void RemoveModifier(string modifierName, string statName)
            => stats[statName].RemoveModifier(modifierName);

        public Stat GetStat(string name) => stats[name];
    }

    [Serializable, InlineProperty, BoxGroup("Stats"), LabelWidth(100)]
    public class Stat
    {
        [SerializeField, HideInInspector]
        private string m_name;
        [HideInInspector]
        public string Name { get; protected set; }


        [SerializeField, HideLabel, HorizontalGroup("Values")]
        [OnValueChanged("@UpdateValue()")]
        private float m_baseValue;
        public float BaseValue { get => m_baseValue; protected set => m_baseValue = value; }


        private float m_value;
        [ShowInInspector, ReadOnly, HorizontalGroup("Values"), LabelText("Current"), LabelWidth(48)]
        public float Value { 
            get { if (!isInitialized) Initialize(); 
                return m_value; } 
            private set => m_value = value; 
        }

        #region RangeEditor

        [HorizontalGroup("Range")]
        [HorizontalGroup("Range/Min", width: 60, LabelWidth = 30)]
        [Tooltip("@min?\"Disable minimum value\":\"Enable minimum value\"")]
        [Button("@min?\"Min On\":\"Min Off\"")]
        public void ToggleMin()
        {
            min = !min;
            if (min) MinValue = tempMin;
            else { tempMin = MinValue; MinValue = float.MinValue; }
            UpdateValue();
        }
        [HorizontalGroup("Range")]
        [HorizontalGroup("Range/Max", width: 60, LabelWidth = 30)]
        [Tooltip("@max?\"Disable maximum value\":\"Enable maximum value\"")]
        [Button("@max?\"Max On\":\"Max Off\"")]
        protected void ToggleMax()
        {
            max = !max;
            if (max) MaxValue = tempMax;
            else { tempMax = MaxValue; MaxValue = float.MaxValue; }
            UpdateValue();
        }
        [SerializeField, HideInInspector]
        protected bool min;
        [SerializeField, HideInInspector]
        protected bool max;
        [SerializeField, HideInInspector]
        protected float tempMin;
        [SerializeField, HideInInspector]
        protected float tempMax;

        #endregion 

        [SerializeField]
        [HorizontalGroup("Range"), EnableIf("@min"), HideLabel]
        [HorizontalGroup("Range/Min")]
        [OnValueChanged("@UpdateValue()")]
        protected float m_minValue;
        public float MinValue { get => m_minValue; private set => m_minValue = value; }

        [SerializeField]
        [HorizontalGroup("Range"), EnableIf("@max"), HideLabel]
        [HorizontalGroup("Range/Max")]
        [OnValueChanged("@UpdateValue()")]
        private float m_maxValue;
        public float MaxValue { get => m_maxValue; private set => m_maxValue = value; }


        [ShowInInspector]
        [OnValueChanged("@UpdateValue()")]
        public Dictionary<string, StatModifier> modifiers { get; private set; }

        public event Action<StatValueChangedEventArgs> OnValueChanged;

        public static implicit operator float(Stat s) => s.Value;

        private bool isInitialized;
        
        public Stat()
        {
            modifiers = new Dictionary<string, StatModifier>();
            MinValue = float.MinValue;
            MaxValue = float.MaxValue;
        }

        public Stat(string name, float baseValue, float minValue, float maxValue) : base()
        {
            this.Name = name;
            this.BaseValue = baseValue;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            
            modifiers = new Dictionary<string, StatModifier>();
            UpdateValue();
        }

        protected void Initialize()
        {
            isInitialized = true;
            min = MinValue != float.MinValue;
            max = MaxValue != float.MaxValue;
            UpdateValue();
        }

        public void SetBaseValue(float newValue)
        {
            BaseValue = newValue;
            UpdateValue();
        }

        public void AddModifier(string id, float add = 0, float multiply = 1, bool keepHighestIfDuplicate = true)
        {
            if (modifiers.ContainsKey(id))
            {
                if (!keepHighestIfDuplicate)
                {
                    modifiers[id].multiply = Mathf.Max(modifiers[id].multiply, multiply);
                    modifiers[id].add = Mathf.Max(modifiers[id].add, add);
                }
                else
                {
                    modifiers[id].multiply = Mathf.Min(modifiers[id].multiply, multiply);
                    modifiers[id].add = Mathf.Min(modifiers[id].add, add);
                }
            }
            else
            {
                modifiers[id] = new StatModifier(add, multiply);
            }
            UpdateValue();
        }

        public bool HasModifier(string id)
        {
            return modifiers.ContainsKey(id);
        }
        public void RemoveModifier(string id)
        {
            if (modifiers.ContainsKey(id))
                modifiers.Remove(id);
            UpdateValue();
        }

        protected void UpdateValue()
        {
            float add = modifiers.Sum(x => x.Value.add);
            float multiply = 1 + modifiers.Sum(x => x.Value.multiply - 1);

            float res = BaseValue * multiply + add;
            float newVal = Mathf.Clamp(res, MinValue, MaxValue);

            float previousValue = Value;
            Value = newVal;

            if (previousValue != newVal)
            {
                OnValueChanged?.Invoke(new StatValueChangedEventArgs(previousValue, newVal));
            }
        }

        public class StatValueChangedEventArgs
        {
            public float previousValue;
            public float newValue;
            public StatValueChangedEventArgs(float previousValue, float newValue)
            {
                this.previousValue = previousValue;
                this.newValue = newValue;
            }
        }

        [Serializable]
        public class StatModifier
        {
            public float add;
            public float multiply;
            public StatModifier() { }
            public StatModifier(float add, float multiply)
            {
                this.add = add;
                this.multiply = multiply;
            }
        }
    }
}