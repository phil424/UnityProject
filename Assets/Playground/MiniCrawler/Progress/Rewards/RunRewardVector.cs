namespace MiniCrawler.Progress
{
    public enum RunRewardVector
    {
        Any,
        CharacterGrowth,
        Ability
    }

    public static class RunRewardVectorPresentation
    {
        public static string GetDisplayName(RunRewardVector vector)
        {
            return vector switch
            {
                RunRewardVector.CharacterGrowth => "GROWTH",
                RunRewardVector.Ability => "ABILITY",
                _ => "ANY"
            };
        }
    }
}