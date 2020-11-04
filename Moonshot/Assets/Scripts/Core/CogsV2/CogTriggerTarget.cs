using UnityEngine;

namespace Core.CogsV2 {
    /// <summary>
    /// Cog V2 features a more precise way to activate targets by allowing you to specify the action to take when triggered. Additionally
    /// this base abstract type allows any source that is connecting to a target to retrieve what the default action should be if needed.
    /// If a trigger target receives an activation with an unsupported action you should either reject the action or use a default action
    /// which is less recommended.
    /// 
    /// DefaultActionOnLink: TBD, RESERVED FOR FUTURE FEATURE
    /// 
    /// To use:    
    /// - Implement your own bevvy of trigger targets.
    /// - Your trigger target needs to implement the Activate() method and process the action(s) they need. It is recommended you also 
    ///   handle the defined DefaultActionOnLink in your Activate() method in addition to the specific action needed.    
    /// </summary>    
    public abstract class CogTriggerTarget : MonoBehaviour {
        public CommonTargetAction DefaultActionOnLink;           // Which action we should take when dynamically linking a source to this target.

        /// <summary>
        /// Activates the trigger with the specified action.
        /// </summary>
        /// <returns>True if the activation was successful</returns>
        public abstract bool Activate(CogTriggerSource source, CommonTargetAction action, TriggerData triggerData);

        /// <summary>
        /// Allows sending a generic command (as integer ID) and a trigger data packet without the
        /// need for the heavier-weight parameters of Activate. Use this when there isn't always a CogTriggerSource
        /// available. If there still needs to be a source reference of a different type you can specify it
        /// within the TriggerData packet.
        /// </summary>
        /// <param name="commandId">A generic command ID to be understood by the trigger target implementor</param>
        /// <param name="triggerData">Any data that needs to accompany the command</param>
        /// <returns>True if the command was accepted</returns>
        public virtual bool Command(int commandId, TriggerData triggerData) { return false; }
    }
}
