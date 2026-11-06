namespace Segra.Backend.App
{
    /// <summary>
    /// Fork-specific settings (fayo build). Kept in its own file so upstream merges stay conflict-free.
    /// The auto-updater only pulls releases from this repository, never from the official Segra releases.
    /// </summary>
    public static class ForkConfig
    {
        public const string UpdateRepositoryUrl = "https://github.com/fayo11111/fayo-Segra";
    }
}
