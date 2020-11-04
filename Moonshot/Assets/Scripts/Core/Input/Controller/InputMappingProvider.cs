using UnityEngine;

namespace Core.Input.Controller {
    public class InputMappingProvider<TAction> where TAction : struct {
        public KeyMapping<TAction, KeyCode> KeyboardMapping { get; set; }
    }
}
