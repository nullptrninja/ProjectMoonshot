using UnityEngine;

namespace Core.Input.Controller {
    public class VirtualController<TAction> where TAction : struct {private IInputReader<TAction, KeyCode> mCurrentReader;
        private IInputReader<TAction, KeyCode> mKeyboardReader;

        public int Id { get; set; }

        public VirtualController(InputMappingProvider<TAction> inputMappingProvider) {
            mKeyboardReader = new KeyboardInputReader<TAction>(inputMappingProvider);
            mCurrentReader = mKeyboardReader;
        }

        public bool GetAction(TAction action) {
            return mCurrentReader.ReadInput(action);
        }

        public bool GetActionDown(TAction action) {
            return mCurrentReader.ReadInputDown(action);
        }

        public bool GetActionUp(TAction action) {
            return mCurrentReader.ReadInputUp(action);
        }

        public float GetDirectionalAction(TAction axialAction) {
            return mCurrentReader.ReadAxis(axialAction);
        }
    }
}
