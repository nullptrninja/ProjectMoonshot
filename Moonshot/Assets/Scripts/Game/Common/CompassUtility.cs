namespace Assets.Scripts.Game.Common {
    public static class CompassUtility {
        public static float WindDirToAngle(CompassDirection cdir) {
            switch (cdir) {
                case CompassDirection.N:
                    return 0f;
                case CompassDirection.E:
                    return 90f;
                case CompassDirection.W:
                    return 270f;
                case CompassDirection.S:
                    return 180f;

                case CompassDirection.NE:
                    return 45f;
                case CompassDirection.NW:
                    return 315f;
                case CompassDirection.SE:
                    return 125f;
                case CompassDirection.SW:
                    return 225f;

                default:
                    return 0f;
            }
        }

        public static float WindDirToInverseAngle(CompassDirection cdir) {
            switch (cdir) {
                case CompassDirection.N:
                    return 180f;
                case CompassDirection.E:
                    return 270f;
                case CompassDirection.W:
                    return 90f;
                case CompassDirection.S:
                    return 0f;

                case CompassDirection.NE:
                    return 225f;
                case CompassDirection.NW:
                    return 125f;
                case CompassDirection.SE:
                    return 315f;
                case CompassDirection.SW:
                    return 45f;

                default:
                    return 0f;
            }
        }
    }
}
