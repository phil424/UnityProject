using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteFlipbookPlayer))]
    public class ActorDeathSpritePresenter : MonoBehaviour
    {
        [Header("Death")]
        [SerializeField] private SpriteFlipbookClip deathClip;
        [SerializeField, Min(0f)] private float finalFrameHold = 0.35f;

        private Health health;
        private SpriteFlipbookPlayer flipbookPlayer;
        private ActorLocomotionSpritePresenter locomotionPresenter;
        private ActorCombatSpritePresenter combatPresenter;

        private bool deathPlaying;
        private float cleanupRemaining;

        private void Awake()
        {
            health = GetComponentInParent<Health>();
            flipbookPlayer = GetComponent<SpriteFlipbookPlayer>();
            locomotionPresenter = GetComponent<ActorLocomotionSpritePresenter>();
            combatPresenter = GetComponent<ActorCombatSpritePresenter>();

            if (health == null)
            {
                Debug.LogError($"{nameof(ActorDeathSpritePresenter)} requires Health in its parent hierarchy.", this);
                enabled = false;
                return;
            }

            if (health.DestroyWhenDead)
            {
                Debug.LogWarning(
                    $"{nameof(ActorDeathSpritePresenter)} on {name} requires Health.Destroy When Dead to be disabled.",
                    this
                );
            }
        }

        private void OnEnable()
        {
            if (health != null)
                health.Died += HandleDied;
        }

        private void OnDisable()
        {
            if (health != null)
                health.Died -= HandleDied;
        }

        private void Update()
        {
            if (!deathPlaying || Time.deltaTime <= 0f)
                return;

            cleanupRemaining -= Time.deltaTime;

            if (cleanupRemaining > 0f)
                return;

            if (health != null)
            {
                Destroy(health.gameObject);
                return;
            }

            Destroy(gameObject);
        }

        private void HandleDied(Health deadHealth)
        {
            if (deathPlaying || deadHealth != health)
                return;

            deathPlaying = true;

            if (combatPresenter != null)
                combatPresenter.enabled = false;

            if (locomotionPresenter != null)
                locomotionPresenter.enabled = false;

            float deathDuration = deathClip != null ? deathClip.Duration : 0f;
            cleanupRemaining = deathDuration + finalFrameHold;

            if (deathClip != null)
                flipbookPlayer.Play(deathClip, true);

            if (cleanupRemaining <= 0f)
                Destroy(health.gameObject);
        }
    }
}