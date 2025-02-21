using Sirenix.Utilities;
using UnityEngine;

namespace Giacomo
{
    public class ScaleWithStat : MonoBehaviour
    {
        [SerializeField] string statName;
        [SerializeField] GameObject statObject;
        public float offset = 0;
        public float multiply = 1;
        Stat stat;

        void OnEnable()
        {
            if(stat != null)
                stat.OnValueChanged += StatChanged;

            if (stat == null && statObject && !statName.IsNullOrWhitespace())
            {
                var s = statObject.GetComponent<IStatObject>().GetStats()[statName];
                SetStat(s);
            }

            UpdateScale();
        }
        private void OnDisable()
        {
            if (stat != null)
                stat.OnValueChanged -= StatChanged;
        }

        public void SetStat(Stat stat)
        {
            if (this.stat != null)
                this.stat.OnValueChanged -= StatChanged;

            this.stat = stat;
            stat.OnValueChanged += StatChanged;
            UpdateScale();
        }

        void StatChanged(Stat.StatValueChangedEventArgs args)
        {
            UpdateScale();
        }

        void UpdateScale()
        {
            if(stat == null)
            {
                transform.localScale = Vector3.zero;
                return;
            }

            transform.localScale = Vector3.one * (stat + offset) * multiply;
        }

    }
}