namespace Core.CogsV2 {

    /// <summary>
    /// If you don't need any custom action functionality you can use this enumeration
    /// with your CogV2 Source/Target triggers. It should cover most base scenarios as well
    /// as some breathing room for vague actions if you don't need anything fancy. However,
    /// feel free to define your own as needed.
    /// The sources and targets defined in Core.CogsV2 will use these action values.
    /// </summary>
    public enum CommonTargetAction {
        DefaultActivation,              // Unassigned actions will use this. Targets should respond to at least this value.
        Show,                           // Target will appear
        Hide,                           // Target will disappear
        Open,                           // Target will "open" (contextually)
        Close,                          // Target will "close" (contextually)        
        Destroy,                        // Target will destroy itself
        Activate,                       // Target is turned "on"
        Deactivate,                     // Target is turned "off"
        Toggle,                         // Target's state will toggle to the other state relative to current state. Can also use as state iterator

        ReceiveData,                    // Used for internal cog communications where a CogTriggerSource may not necessarily be available

        // TODO: More stuff here when we think of it

        CustomA,                        // These are custom actions for future expandability
        CustomB,
        CustomC,
        CustomD,
        CustomE
    }

    /// <summary>
    /// Holds DataId types for TriggerData.DataId values. If specifying your own custom data Ids it is recommended
    /// that you start at ID = 1000 and higher in the event we add new built-in Ids here.
    /// </summary>
    public static class DataTypes {
        public const int Default = 0;            // The default typing for TriggerData.DataTypeId
    }
}
