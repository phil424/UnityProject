using System.Collections.Generic;
using MiniCrawler.Movement;

namespace MiniCrawler.Encounters
{
    public sealed class EncounterRouteCompatibilityResult
    {
        private readonly List<RouteTagDefinition> effectiveSiteTags;
        private readonly List<RouteTagDefinition> missingRequiredTags;
        private readonly List<RouteTagDefinition> matchedPreferredTags;
        private readonly List<RouteTagDefinition> presentForbiddenTags;

        internal EncounterRouteCompatibilityResult(
            EncounterDefinition definition,
            EncounterRouteSite site,
            IReadOnlyList<RouteTagDefinition> effectiveSiteTags,
            IReadOnlyList<RouteTagDefinition> missingRequiredTags,
            IReadOnlyList<RouteTagDefinition> matchedPreferredTags,
            IReadOnlyList<RouteTagDefinition> presentForbiddenTags,
            int preferredTagCount)
        {
            Definition = definition;
            Site = site;
            PreferredTagCount = preferredTagCount;

            this.effectiveSiteTags = new List<RouteTagDefinition>(effectiveSiteTags);
            this.missingRequiredTags = new List<RouteTagDefinition>(missingRequiredTags);
            this.matchedPreferredTags = new List<RouteTagDefinition>(matchedPreferredTags);
            this.presentForbiddenTags = new List<RouteTagDefinition>(presentForbiddenTags);
        }

        public EncounterDefinition Definition { get; }
        public EncounterRouteSite Site { get; }

        public IReadOnlyList<RouteTagDefinition> EffectiveSiteTags => effectiveSiteTags;
        public IReadOnlyList<RouteTagDefinition> MissingRequiredTags => missingRequiredTags;
        public IReadOnlyList<RouteTagDefinition> MatchedPreferredTags => matchedPreferredTags;
        public IReadOnlyList<RouteTagDefinition> PresentForbiddenTags => presentForbiddenTags;

        public bool HasValidInputs => Definition != null && Site != null;
        public bool AutomaticPlacementEnabled => Definition != null && Definition.AllowAutomaticPlacement;
        public bool MatchesTagRequirements => HasValidInputs && missingRequiredTags.Count == 0 && presentForbiddenTags.Count == 0;
        public bool IsCompatible => AutomaticPlacementEnabled && MatchesTagRequirements;

        public int PreferredTagCount { get; }
        public int PreferredMatchCount => matchedPreferredTags.Count;
        public float PreferenceScore => PreferredTagCount > 0 ? (float)PreferredMatchCount / PreferredTagCount : 0f;
    }

    public static class EncounterRouteCompatibility
    {
        public static EncounterRouteCompatibilityResult Evaluate(EncounterDefinition definition, EncounterRouteSite site)
        {
            List<RouteTagDefinition> effectiveTags = new();
            List<RouteTagDefinition> missingRequired = new();
            List<RouteTagDefinition> matchedPreferred = new();
            List<RouteTagDefinition> presentForbidden = new();

            if (site != null)
                site.CollectEffectiveTags(effectiveTags);

            int preferredTagCount = 0;

            if (definition != null)
            {
                CollectMissingTags(definition.RequiredRouteTags, effectiveTags, missingRequired);
                CollectMatchedTags(definition.PreferredRouteTags, effectiveTags, matchedPreferred);
                CollectMatchedTags(definition.ForbiddenRouteTags, effectiveTags, presentForbidden);
                preferredTagCount = CountUniqueTags(definition.PreferredRouteTags);
            }

            return new EncounterRouteCompatibilityResult(
                definition,
                site,
                effectiveTags,
                missingRequired,
                matchedPreferred,
                presentForbidden,
                preferredTagCount
            );
        }

        public static void CollectCompatibleSites(
            EncounterDefinition definition,
            IReadOnlyList<EncounterRouteSite> sites,
            List<EncounterRouteCompatibilityResult> results)
        {
            results.Clear();

            if (definition == null || sites == null)
                return;

            foreach (EncounterRouteSite site in sites)
            {
                if (site == null)
                    continue;

                EncounterRouteCompatibilityResult result = Evaluate(definition, site);

                if (result.IsCompatible)
                    results.Add(result);
            }

            results.Sort(CompareCandidates);
        }

        public static void CollectDefinitionValidationIssues(EncounterDefinition definition, List<string> issues)
        {
            if (definition == null || issues == null)
                return;

            CollectTagListIssues("Required", definition.RequiredRouteTags, issues);
            CollectTagListIssues("Preferred", definition.PreferredRouteTags, issues);
            CollectTagListIssues("Forbidden", definition.ForbiddenRouteTags, issues);

            CollectRoleConflictIssues("Required", definition.RequiredRouteTags, "Preferred", definition.PreferredRouteTags, issues);
            CollectRoleConflictIssues("Required", definition.RequiredRouteTags, "Forbidden", definition.ForbiddenRouteTags, issues);
            CollectRoleConflictIssues("Preferred", definition.PreferredRouteTags, "Forbidden", definition.ForbiddenRouteTags, issues);
        }

        private static void CollectMissingTags(
            IReadOnlyList<RouteTagDefinition> expected,
            IReadOnlyList<RouteTagDefinition> available,
            List<RouteTagDefinition> results)
        {
            HashSet<RouteTagDefinition> seen = new();

            foreach (RouteTagDefinition tag in expected)
            {
                if (tag == null || !seen.Add(tag))
                    continue;

                if (!ContainsTag(available, tag))
                    results.Add(tag);
            }
        }

        private static void CollectMatchedTags(
            IReadOnlyList<RouteTagDefinition> expected,
            IReadOnlyList<RouteTagDefinition> available,
            List<RouteTagDefinition> results)
        {
            HashSet<RouteTagDefinition> seen = new();

            foreach (RouteTagDefinition tag in expected)
            {
                if (tag == null || !seen.Add(tag))
                    continue;

                if (ContainsTag(available, tag))
                    results.Add(tag);
            }
        }

        private static int CountUniqueTags(IReadOnlyList<RouteTagDefinition> tags)
        {
            HashSet<RouteTagDefinition> unique = new();

            foreach (RouteTagDefinition tag in tags)
            {
                if (tag != null)
                    unique.Add(tag);
            }

            return unique.Count;
        }

        private static bool ContainsTag(IReadOnlyList<RouteTagDefinition> tags, RouteTagDefinition target)
        {
            if (target == null)
                return false;

            foreach (RouteTagDefinition tag in tags)
            {
                if (tag == target)
                    return true;
            }

            return false;
        }

        private static void CollectTagListIssues(
            string role,
            IReadOnlyList<RouteTagDefinition> tags,
            List<string> issues)
        {
            HashSet<RouteTagDefinition> seen = new();

            foreach (RouteTagDefinition tag in tags)
            {
                if (tag == null)
                {
                    issues.Add($"{role} Route Tags contains an empty reference.");
                    continue;
                }

                if (!seen.Add(tag))
                    issues.Add($"{role} Route Tags contains duplicate Tag '{tag.DisplayName}'.");
            }
        }

        private static void CollectRoleConflictIssues(
            string firstRole,
            IReadOnlyList<RouteTagDefinition> firstTags,
            string secondRole,
            IReadOnlyList<RouteTagDefinition> secondTags,
            List<string> issues)
        {
            HashSet<RouteTagDefinition> reported = new();

            foreach (RouteTagDefinition tag in firstTags)
            {
                if (tag == null || !reported.Add(tag))
                    continue;

                if (ContainsTag(secondTags, tag))
                    issues.Add($"Route Tag '{tag.DisplayName}' appears in both {firstRole} and {secondRole}. Give each Tag one placement role.");
            }
        }

        private static int CompareCandidates(EncounterRouteCompatibilityResult a, EncounterRouteCompatibilityResult b)
        {
            int scoreComparison = b.PreferenceScore.CompareTo(a.PreferenceScore);

            if (scoreComparison != 0)
                return scoreComparison;

            return string.CompareOrdinal(GetSiteName(a.Site), GetSiteName(b.Site));
        }

        private static string GetSiteName(EncounterRouteSite site)
        {
            if (site == null)
                return string.Empty;

            return site.Encounter != null ? site.Encounter.DisplayName : site.name;
        }
    }
}