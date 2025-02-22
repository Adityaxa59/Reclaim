using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Giacomo
{
    public class Projectile : MonoBehaviour
    {
        [DisableInEditorMode, SerializeField] private float damage;
        [DisableInEditorMode, SerializeField] private float speed;
        [DisableInEditorMode, SerializeField] private float lifetime;
        [DisableInEditorMode, SerializeField] private float splashArea;
        [DisableInEditorMode, SerializeField] private Targetable target;
        [DisableInEditorMode, SerializeField] private bool destroyIfTargetDied;

        [SerializeField] string hitSoundEffect = "";
        [SerializeField] float hitSoundVolume = 1;

        [SerializeField] GameObject impactEffect;
        [SerializeField] private Color impactColor;

        protected Vector3 lastTargetPosition;
        private bool isInitialized;
        public void Initialize(float damage, float speed, float lifetime, float splashArea, Targetable target, bool destroyIfTargetDied, string hitSoundEffect = null, float hitSoundVolume = -1f)
        {
            isInitialized = true;
            this.damage = damage;
            this.speed = speed;
            this.lifetime = lifetime;
            this.splashArea = splashArea;
            this.target = target;
            this.destroyIfTargetDied = destroyIfTargetDied;
            lastTargetPosition = target.transform.position;

            if (!string.IsNullOrEmpty(hitSoundEffect))
                this.hitSoundEffect = hitSoundEffect;
            if(hitSoundVolume != -1)
                this.hitSoundVolume = hitSoundVolume;
        }

        private float despawnTime;
        private void Start()
        {
            if (!isInitialized)
                Destroy(gameObject);
            despawnTime = Time.time + lifetime;
        }

        private void Update()
        {
            if (Time.time >= despawnTime)
            {
                Destroy(gameObject);
                return;
            }

            if (target == null || !target.isAlive)
            {
                if (destroyIfTargetDied)
                {
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                lastTargetPosition = target.transform.position;
            }

            
            if (MoveTowards(lastTargetPosition))
            {
                TargetHit();
                Destroy(gameObject);
            }
        }

        public bool MoveTowards(Vector3 targetPosition)
        {
            bool reached = false;
            Vector3 direction = (targetPosition - transform.position).normalized;
            var moveby = direction * speed * Time.deltaTime;

            if (Vector3.Distance(targetPosition, transform.position) < speed * Time.deltaTime)
                reached = true;

            transform.up = direction;
            transform.position += moveby;
            return reached;
        }


        protected void TargetHit()
        {
            GameObject impactGO = null;
            if (impactEffect)
            {
                impactGO = Instantiate(impactEffect, transform.position, transform.rotation);
                impactGO.GetComponent<ImpactEffect>()?.SetColor(impactColor);
            }
            Destroy(impactGO, .2f);

            var pitch = new AudioParams.Pitch(AudioParams.Pitch.Variation.Medium);
            var repetition = new AudioParams.Repetition(.05f);
            AudioController.Instance.PlaySound2D(hitSoundEffect, hitSoundVolume, pitch: pitch, repetition: repetition);

            if (splashArea <= 0)
            {
                target.Damage(damage);
            }
            else
            {
                var hit = Physics2D.CircleCastAll(transform.position, splashArea, Vector2.zero);
                foreach (var h in hit)
                {
                    var targetable = h.transform.GetComponent<Targetable>();
                    if (targetable)
                        targetable.Damage(damage);
                }

                if(impactGO)
                    impactGO.transform.localScale = Vector3.one * splashArea;
            }
        }

    }
}