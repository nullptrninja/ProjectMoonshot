using UnityEngine;

namespace Core.Input.Cursor {
    public struct CursorResult {
        public bool Result;
        public int InputId;             // Either the mouse or the finger id
        public Vector3 Position;
        public Vector3 ScreenPosition;
    }
}
