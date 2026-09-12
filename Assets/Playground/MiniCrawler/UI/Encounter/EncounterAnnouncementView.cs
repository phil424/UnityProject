using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class EncounterAnnouncementView : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text subtitleText;

        private CanvasGroup canvasGroup;
        private Coroutine playRoutine;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void Play(
            string title,
            string subtitle,
            float fadeInSeconds,
            float holdSeconds,
            float fadeOutSeconds,
            Action<EncounterAnnouncementView> finished)
        {
            if (playRoutine != null)
                StopCoroutine(playRoutine);

            titleText.text = title;

            bool hasSubtitle = !string.IsNullOrWhiteSpace(subtitle);

            subtitleText.gameObject.SetActive(hasSubtitle);

            if (hasSubtitle)
                subtitleText.text = subtitle;

            gameObject.SetActive(true);
            playRoutine = StartCoroutine(
                PlayRoutine(fadeInSeconds, holdSeconds, fadeOutSeconds, finished)
            );
        }

        private IEnumerator PlayRoutine(
            float fadeInSeconds,
            float holdSeconds,
            float fadeOutSeconds,
            Action<EncounterAnnouncementView> finished)
        {
            yield return Fade(0f, 1f, fadeInSeconds);

            if (holdSeconds > 0f)
                yield return new WaitForSecondsRealtime(holdSeconds);

            yield return Fade(1f, 0f, fadeOutSeconds);

            playRoutine = null;
            finished?.Invoke(this);
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            if (duration <= 0f)
            {
                canvasGroup.alpha = to;
                yield break;
            }

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            canvasGroup.alpha = to;
        }
    }
}