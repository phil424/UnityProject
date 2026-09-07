using System;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Encounters
{
    [DefaultExecutionOrder(-350)]
    public class EncounterDirectionController : MonoBehaviour
    {
        public static EncounterDirectionController Instance { get; private set; }

        [SerializeField, Min(0.1f)] private float arrivalDistance = 2.5f;

        public event Action<LevelEncounter> SelectedEncounterChanged;

        public LevelEncounter SelectedEncounter { get; private set; }
        public bool IsTravelling { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            ClearPartyNavigationIntents();

            if (Instance == this)
                Instance = null;
        }

        private void Update()
        {
            if (!IsTravelling)
            {
                if (SelectedEncounter != null && SelectedEncounter.IsCompleted)
                    ClearSelection();

                return;
            }

            if (SelectedEncounter == null || SelectedEncounter.IsCompleted || SelectedEncounter.IsExpired)
            {
                ClearSelection();
                return;
            }

            ApplyTravelIntentToParty();

            if (!HasAnyPartyMemberArrived())
                return;

            ArriveAtSelectedEncounter();
        }

        public bool SelectEncounter(LevelEncounter encounter)
        {
            if (encounter == null || !encounter.IsSelectable)
                return false;

            if (SelectedEncounter == encounter && !IsTravelling)
                return false;

            SelectedEncounter = encounter;
            IsTravelling = true;

            ClearPartyCombatTargets();
            ApplyTravelIntentToParty();

            SelectedEncounterChanged?.Invoke(SelectedEncounter);

            Debug.Log($"Encounter directive selected: {encounter.DisplayName}", this);
            return true;
        }

        public void ClearSelection()
        {
            ClearPartyNavigationIntents();

            SelectedEncounter = null;
            IsTravelling = false;

            SelectedEncounterChanged?.Invoke(null);
        }

        private void ArriveAtSelectedEncounter()
        {
            if (SelectedEncounter == null)
                return;

            // Prototype default: reaching a selected encounter commits to its gameplay.
            // Existing authored triggers may still begin spawning/activate combat earlier.
            SelectedEncounter.BeginSpawning();
            SelectedEncounter.ActivateCombat();

            ClearPartyNavigationIntents();
            IsTravelling = false;

            Debug.Log($"Encounter directive arrived: {SelectedEncounter.DisplayName}", this);
        }

        private void ApplyTravelIntentToParty()
        {
            if (SelectedEncounter == null)
                return;

            foreach (PartyMember partyMember in FindObjectsByType<PartyMember>(FindObjectsSortMode.None))
            {
                if (!IsLivingPartyMember(partyMember))
                    continue;

                AutoTargetMover mover = partyMember.GetComponent<AutoTargetMover>();

                if (mover == null || !mover.isActiveAndEnabled)
                    continue;

                ActorNavigationIntent navigation = partyMember.GetComponent<ActorNavigationIntent>();

                if (navigation == null)
                    navigation = partyMember.gameObject.AddComponent<ActorNavigationIntent>();

                navigation.SetDestination(this, SelectedEncounter.AnchorPosition, arrivalDistance, suppressCombatTargeting: true);
            }
        }

        private bool HasAnyPartyMemberArrived()
        {
            foreach (PartyMember partyMember in FindObjectsByType<PartyMember>(FindObjectsSortMode.None))
            {
                if (!IsLivingPartyMember(partyMember))
                    continue;

                ActorNavigationIntent navigation = partyMember.GetComponent<ActorNavigationIntent>();

                if (navigation != null && navigation.IsOwnedBy(this) && navigation.HasArrived(partyMember.transform))
                    return true;
            }

            return false;
        }

        private void ClearPartyCombatTargets()
        {
            foreach (PartyMember partyMember in FindObjectsByType<PartyMember>(FindObjectsSortMode.None))
            {
                AutoTargetMover mover = partyMember != null ? partyMember.GetComponent<AutoTargetMover>() : null;

                if (mover != null && mover.CurrentIntent == TargetIntent.Combat)
                    mover.ClearTarget();
            }
        }

        private void ClearPartyNavigationIntents()
        {
            foreach (PartyMember partyMember in FindObjectsByType<PartyMember>(FindObjectsSortMode.None))
            {
                ActorNavigationIntent navigation = partyMember != null
                    ? partyMember.GetComponent<ActorNavigationIntent>()
                    : null;

                navigation?.Clear(this);
            }
        }

        private static bool IsLivingPartyMember(PartyMember partyMember)
        {
            if (partyMember == null)
                return false;

            Health health = partyMember.GetComponent<Health>();
            return health == null || !health.IsDead;
        }
    }
}