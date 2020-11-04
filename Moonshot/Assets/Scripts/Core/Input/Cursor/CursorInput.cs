using UnityEngine;

namespace Core.Input.Cursor {
    public static class CursorInput {
        public static CursorResult CursorDown(int buttonOrFingerId = 0) {
#if UNITY_STANDALONE_WIN
            var mouseDown = UnityEngine.Input.GetMouseButtonDown(buttonOrFingerId);
            if (mouseDown) {
                return new CursorResult() {
                    Position = UnityEngine.Input.mousePosition,
                    ScreenPosition = new Vector2(UnityEngine.Input.mousePosition.x, Screen.height - UnityEngine.Input.mousePosition.y),
                    InputId = buttonOrFingerId,
                    Result = mouseDown
                };
            }
#else
                        
            if (UnityEngine.Input.touchCount > 0) {
                // Lol. Touch down.
                var touchDown = UnityEngine.Input.GetTouch(0);
                if (touchDown.phase == TouchPhase.Began) {
                    return new CursorResult() {
                        Position = touchDown.position,
                        ScreenPosition = new Vector2(touchDown.position.x, Screen.height - touchDown.position.y),
                        InputId = buttonOrFingerId,
                        Result = true
                    };
                }
            }
#endif
            return new CursorResult() {
                Result = false,
                InputId = buttonOrFingerId,
                Position = Vector3.zero
            };
        }

        public static CursorResult CursorUp(int buttonOrFingerId = 0) {
#if UNITY_STANDALONE_WIN
            var mouseDown = UnityEngine.Input.GetMouseButtonUp(buttonOrFingerId);
            if (mouseDown) {
                return new CursorResult() {
                    Position = UnityEngine.Input.mousePosition,
                    ScreenPosition = new Vector2(UnityEngine.Input.mousePosition.x, Screen.height - UnityEngine.Input.mousePosition.y),
                    InputId = buttonOrFingerId,
                    Result = mouseDown
                };
            }
#else
            if (UnityEngine.Input.touchCount > 0) {
                // Lol. Touch down.
                var touchDown = UnityEngine.Input.GetTouch(0);
                if (touchDown.phase == TouchPhase.Ended) {
                    return new CursorResult() {
                        Position = touchDown.position,
                        ScreenPosition = new Vector2(touchDown.position.x, Screen.height - touchDown.position.y),
                        InputId = buttonOrFingerId,
                        Result = true
                    };
                }
            }
#endif
            return new CursorResult() {
                Result = false,
                InputId = buttonOrFingerId,
                Position = Vector3.zero
            };
        }

        public static CursorResult CursorPressed(int buttonOrFingerId = 0) {
#if UNITY_STANDALONE_WIN
            var mouseDown = UnityEngine.Input.GetMouseButton(buttonOrFingerId);
            if (mouseDown) {
                return new CursorResult() {
                    Position = UnityEngine.Input.mousePosition,
                    ScreenPosition = new Vector2(UnityEngine.Input.mousePosition.x, Screen.height - UnityEngine.Input.mousePosition.y),
                    InputId = buttonOrFingerId,
                    Result = mouseDown
                };
            }
#else
            if (UnityEngine.Input.touchCount > 0) {
                // Lol. Touch down.
                var touchDown = UnityEngine.Input.GetTouch(0);
                if (touchDown.phase == TouchPhase.Stationary || touchDown.phase == TouchPhase.Moved) {
                    return new CursorResult() {
                        Position = touchDown.position,
                        ScreenPosition = new Vector2(touchDown.position.x, Screen.height - touchDown.position.y),
                        InputId = buttonOrFingerId,
                        Result = true
                    };
                }
            }
#endif
            return new CursorResult() {
                Result = false,
                InputId = buttonOrFingerId,
                Position = Vector3.zero
            };
        }
    }
}
