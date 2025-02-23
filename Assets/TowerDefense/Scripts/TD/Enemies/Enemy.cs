using System;
using UnityEngine;

namespace Giacomo
{
    public class Enemy : Targetable
    {
        public float maxPositionOffset = 0.25f;

        public Stat MoveSpeed;
        public Stat MoneyReward;
        public Stat DamageToTower;


        public FollowPathMovement movement;

        [HideInInspector]
        public EffectHandler EffectHandler;

        protected override void ManagedInitialize()
        {
            base.ManagedInitialize();

            if(!TryGetComponent(out EffectHandler))
                EffectHandler = gameObject.AddComponent<EffectHandler>();
            
            movement = GetComponent<FollowPathMovement>();
            Vector2 positionOffset = new Vector2(
                UnityEngine.Random.Range(-maxPositionOffset, maxPositionOffset),
                UnityEngine.Random.Range(-maxPositionOffset, maxPositionOffset));
            movement.SetPositionOffset(positionOffset);

            UpdateDestination();

            GameManager.AddEnemy(this);
        }

        public void UpdateDestination()
        {
            movement.SetDestinationCriteria((Tile t) => t.isHome);
            movement.OnArrive += ReachedHomeTile;
            if (movement.path?.Count <= 1)
                Debug.LogError($"Path not found ({name})", gameObject);
        }


        public override Stats GetStats()
        {
            if(stats != null) return stats;
            
            var tempStats = base.GetStats();
            tempStats.AddStat("moveSpeed", MoveSpeed);
            tempStats.AddStat("moneyReward", MoneyReward);
            tempStats.AddStat("damageToTower", DamageToTower);
            return tempStats;
        }

        public override void ManagedUpdate()
        {
            float moveAmount = MoveSpeed * Time.deltaTime;
            movement.Move(moveAmount);
        }

        protected void ReachedHomeTile()
        {
            GameStats.Instance?.LoseHP((int)DamageToTower);
            Kill();
        }

        protected override void Kill()
        {
            isAlive = false;
        }

        protected void Die()
        {
            GameStats.Instance.ModifyCoins((int)MoneyReward);

            var pitch = new AudioParams.Pitch(AudioParams.Pitch.Variation.Small);
            var repetition = new AudioParams.Repetition(.05f);
            AudioController.Instance.PlaySound2D("enemy_death", .2f, pitch: pitch, repetition: repetition);

            GameManager.RemoveEnemy(this);
            Destroy(gameObject);
        }

        public override void ManagedLateUpdate()
        {
            if (!isAlive)
            {
                Die();
            }
        }
    }
}