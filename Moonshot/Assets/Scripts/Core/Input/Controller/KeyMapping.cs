using System.Collections.Generic;

namespace Core.Input.Controller {
    public class KeyMapping<TAction, TInput> where TAction : struct {
        /// <summary>
        /// Maps high level actions to specific input keys. Setter is exposed for initialization
        /// but querying should be done via GetInput()
        /// </summary>
        public Dictionary<TAction, TInput> AllActions { get; set; }

        /// <summary>
        /// Maps high level directional actions to specific input keys. Setter is exposed for
        /// initialization but querying should be done via ExpandDirectionalInputs()
        /// </summary>
        public Dictionary<TAction, TInput[]> DirectionalMaps { get; set; }

        /// <summary>
        /// Gets or sets whether this key mapping uses a single input key to represent
        /// an axial query; for example a keyboard would have this set to False since
        /// UP and DOWN need to be checked separately/exclusively from each other.
        /// </summary>
        public bool SingleInputPerAxis { get; set; }

        public TInput this[TAction action] => AllActions[action];

        public KeyMapping() {
            this.AllActions = new Dictionary<TAction, TInput>();
            this.DirectionalMaps = new Dictionary<TAction, TInput[]>();
        }

        public TInput[] ExpandDirectionalInputs(TAction action) {
            return this.DirectionalMaps[action];
        }

        public TInput GetInput(TAction action) {
            return this.AllActions[action];
        }
    }
}
