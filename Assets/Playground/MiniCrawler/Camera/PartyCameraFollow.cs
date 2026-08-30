using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Presentation
{
    [DisallowMultipleComponent]
    public class PartyCameraFollow :
        MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private StageDirector stageDirector;

        [SerializeField]
        private Transform followOrigin;

        [Header("Follow")]
        [SerializeField]
        [Min(0f)]
        private float followSharpness = 8f;

        private readonly List<GameObject>
            trackedPartyActors = new();

        private Vector3 followOffset;

        private bool hasFollowOffset;

        private void Awake()
        {
            CaptureFollowOffset();
        }

        private void Start()
        {
            if (stageDirector == null)
            {
                stageDirector =
                    StageDirector.Instance;
            }

            if (stageDirector == null)
            {
                Debug.LogError(
                    "PartyCameraFollow requires " +
                    "a StageDirector."
                );

                enabled = false;

                return;
            }

            if (followOrigin == null)
            {
                Debug.LogError(
                    "PartyCameraFollow requires " +
                    "a Follow Origin."
                );

                enabled = false;

                return;
            }

            if (!hasFollowOffset)
            {
                CaptureFollowOffset();
            }

            stageDirector.PartyMemberSpawned +=
                HandlePartyMemberSpawned;

            stageDirector.LevelCleared +=
                HandleLevelCleared;

            ResetToFollowOrigin();
        }

        private void OnDestroy()
        {
            if (stageDirector == null)
                return;

            stageDirector.PartyMemberSpawned -=
                HandlePartyMemberSpawned;

            stageDirector.LevelCleared -=
                HandleLevelCleared;
        }

        private void LateUpdate()
        {
            RemoveMissingActors();

            if (
                !TryGetPartyCentre(
                    out Vector3 centre
                )
            )
            {
                return;
            }

            Vector3 desiredPosition =
                centre +
                followOffset;

            if (
                followSharpness <= 0f ||
                Time.deltaTime <= 0f
            )
            {
                transform.position =
                    desiredPosition;

                return;
            }

            float interpolation =
                1f -
                Mathf.Exp(
                    -followSharpness *
                    Time.deltaTime
                );

            transform.position =
                Vector3.Lerp(
                    transform.position,
                    desiredPosition,
                    interpolation
                );
        }

        private void HandlePartyMemberSpawned(
            PartyMemberDefinition definition,
            GameObject actor
        )
        {
            if (
                actor == null ||
                trackedPartyActors.Contains(
                    actor
                )
            )
            {
                return;
            }

            trackedPartyActors.Add(
                actor
            );

            if (
                TryGetPartyCentre(
                    out Vector3 centre
                )
            )
            {
                transform.position =
                    centre +
                    followOffset;
            }
        }

        private void HandleLevelCleared()
        {
            trackedPartyActors.Clear();

            ResetToFollowOrigin();
        }

        private bool TryGetPartyCentre(
            out Vector3 centre
        )
        {
            centre = Vector3.zero;

            int livingCount = 0;

            foreach (
                GameObject actor
                in trackedPartyActors
            )
            {
                if (actor == null)
                    continue;

                Health health =
                    actor.GetComponent<Health>();

                if (
                    health != null &&
                    health.IsDead
                )
                {
                    continue;
                }

                centre +=
                    actor.transform.position;

                livingCount++;
            }

            if (livingCount <= 0)
                return false;

            centre /= livingCount;

            return true;
        }

        private void RemoveMissingActors()
        {
            for (
                int index =
                    trackedPartyActors.Count - 1;
                index >= 0;
                index--
            )
            {
                if (
                    trackedPartyActors[index] ==
                    null
                )
                {
                    trackedPartyActors.RemoveAt(
                        index
                    );
                }
            }
        }

        private void CaptureFollowOffset()
        {
            if (followOrigin == null)
                return;

            followOffset =
                transform.position -
                followOrigin.position;

            hasFollowOffset = true;
        }

        private void ResetToFollowOrigin()
        {
            if (
                followOrigin == null ||
                !hasFollowOffset
            )
            {
                return;
            }

            transform.position =
                followOrigin.position +
                followOffset;
        }

        private void OnValidate()
        {
            followSharpness =
                Mathf.Max(
                    0f,
                    followSharpness
                );
        }
    }
}