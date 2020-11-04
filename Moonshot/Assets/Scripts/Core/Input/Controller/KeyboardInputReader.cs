using UnityEngine;

namespace Core.Input.Controller {
    public class KeyboardInputReader<TAction> : IInputReader<TAction, KeyCode> where TAction : struct {
        private KeyMapping<TAction, KeyCode> mMapping;

        public KeyboardInputReader(InputMappingProvider<TAction> mappingsProvider) {
            mMapping = mappingsProvider.KeyboardMapping;
        }

        public void SetInputMap(KeyMapping<TAction, KeyCode> mapping) {
            mMapping = mapping;
        }

        public bool ReadInput(TAction input) {
            var actionToInput = mMapping[input];
            return UnityEngine.Input.GetKey(actionToInput);
        }

        public bool ReadInputDown(TAction input) {
            var actionToInput = mMapping[input];
            return UnityEngine.Input.GetKeyDown(actionToInput);
        }

        public bool ReadInputUp(TAction input) {
            var actionToInput = mMapping[input];
            return UnityEngine.Input.GetKeyUp(actionToInput);
        }

        public float ReadAxis(TAction axialInput) {            
            var inputs = mMapping.ExpandDirectionalInputs(axialInput);
            
            // SingleInputPerAxis is not support for keyboard input. There must be 2 inputs
            // mapped per axis but we don't enforce this, just don't fuck it up. The first
            // element must be in the negative direction
            if (UnityEngine.Input.GetKey(inputs[0])) {
                return -1f;
            }
            else if (UnityEngine.Input.GetKey(inputs[1])) {
                return 1f;
            }

            return 0f;
        }
    }
}
