namespace Core.CogsV1 {
    /// <summary>
    /// Describes an object that implements Cog interaction
    /// </summary>
    public interface ICog {

        /// <summary>
        /// Returns output data of the meshed cog.
        /// </summary>
        CogData Output { get; }

        /// <summary>
        /// Triggers an Activation event on the cog. Some Cogs may treat repeated calls
        /// to Activate as a toggle instead. This is your primary method that will be called
        /// by COG-users, it's not reasonable to expect a Deactivate call to be made unless
        /// in scenarios you control.
        /// </summary>
        /// <param name="source">Triggering object</param>
        /// <param name="data">Optional data to pass into the Cog</param>
        void Activate(object source, CogData data);

        /// <summary>
        /// Triggers a Deactivation event on the cog. It is suggested that Deactivate
        /// events will always deactivate the cog rather than toggling the state. Not all
        /// callers will use Deactivate naturally, you should expect Activate to be the
        /// primary calling method. Only in specific circumstances that you control should
        /// you be using Deactivate.
        /// </summary>
        /// <param name="source">Triggering object</param>
        /// <param name="data">Optional data to pass into the Cog</param>
        void Deactivate(object source, CogData data);

        /// <summary>
        /// Joins another cog with this one. The parameterized Cog's Output property will
        /// be used as an input data source for this cog. The function call is a one-way mesh, meaning
        /// that the input (parameterized) cog will not know it is meshed. Only one cog may be meshed to this cog at any time.
        /// </summary>
        /// <param name="cog">Cog to mesh with. It is suggested that specifying a null value will demesh the cog from this one.</param>
        void Mesh(ICog cog);
    }
}
