using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Giacomo
{
    public class StunTower : AttackingTower
    {
        public Stat StunDuration;
        public Stat StunChance;

        public Transform impactEffect;
        [SerializeField] Animator attackAnim;

        public override Stats GetStats()
        {
            if (stats != null) 
                return stats;

            var tempStats = base.GetStats();
            tempStats.AddStat("stunDuration", StunDuration);
            tempStats.AddStat("stunChance", StunChance);
            return tempStats;
        }

        protected override void Attack()
        {
            foreach (Enemy e in GameManager.Enemies)
            {
                var dist = Vector2.Distance(transform.position, e.transform.position);
                if (dist < MinRange || dist > MaxRange) continue;

                e.Damage(Damage);

                if (Random.Range(0, 1f) > StunChance) continue;

                StatModifierEffect stunEffect = new StatModifierEffect("stun", e.stats);
                stunEffect.AddModifier("moveSpeed", multiply: 0);
                e.EffectHandler.AddEffect("stun", stunEffect, StunDuration);
            }
            StartCoroutine(AttackEffect());
        }

        //does not work with high fire rates. to fix I'd need to increase the animation speed (but this is fine for now)
        protected IEnumerator AttackEffect()
        {
            attackAnim.SetTrigger("attack");
            attackAnim.speed = AttackSpeed;
            impactEffect.gameObject.SetActive(true);
            yield return Helpers.GetWait(0.2f);
            impactEffect.gameObject.SetActive(false);
        }
    }
}
