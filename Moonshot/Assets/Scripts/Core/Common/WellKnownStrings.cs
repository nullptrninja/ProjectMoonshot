namespace Core.Common {
    /// <summary>
    /// Maintains a list of well-known strings that CORE and GAME code can
    /// both rely on existing.
    /// </summary>
    public static class WellKnownStrings {
        // These are tags, not an object names
        public const string StateHookTag = "StateHook";
        public const string PlayerTag = "Player";
        public const string MainCameraProxy = "MainCameraProxy";        
    }
}
