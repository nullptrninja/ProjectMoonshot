using UnityEngine;

namespace Core.Behaviors.Functional.Character {
    public interface IIsometricCharacterController {
        void Move(Vector3 normalizedMoveVect);

        void ReleaseMovement();

        void StopMovement();

        void MoveImpulse(Vector3 normalizedMoveVect, float maxVelocity);

        bool InterpolateToPoint(Vector3 targetPoint, float moveScalar, float tolerance);

        void FaceObject(Transform obj);
    }
}
