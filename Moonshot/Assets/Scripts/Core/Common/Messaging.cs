namespace Core.Common {
    public static class Messaging {
        // Channels
        public const int SystemChannel = 0;         // For systems related messaging such as core application event messages
        public const int GameChannel = 1;           // For game messages such as configuration, mode, and state changes
        public const int UIChannel = 2;             // For UI/sub-UI messages

        // Group IDs
        public const int RemoteProcCallRequestGroupId = 100;
        public const int MouseEventGroupId = 101;               // Generic mouse event occurred

        // Sub IDs
        public const int UIButtonPressedSubId = 100;
        public const int MouseDownEventSubId = 101;             // Mouse down event
        public const int MouseUpEventSubId = 102;               // Mouse up event
    }
}
