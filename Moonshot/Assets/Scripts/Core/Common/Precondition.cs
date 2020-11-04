using System;

namespace Core.Common {
    public static class Precondition {
        public static void Check(bool expression, string failureMessage) {
            if (!expression) {
                throw new Exception(failureMessage);
            }
        }

        public static T Check<T>(bool expression, T passCheckVal, string failureMessage) {
            if (!expression) {
                throw new Exception(failureMessage);
            }

            return passCheckVal;
        }

        public static T IsNotNull<T>(T checkValue) where T : class {
            if (checkValue == null) {
                throw new Exception("Precondition check: Object was null");
            }

            return checkValue;
        }

        public static string IsNotEmptyOrWhitespace(string value) {
            if (string.IsNullOrWhiteSpace(value)) {
                throw new Exception("Precondition check: IsNotEmptyOrWhitespace failed");
            }

            return value;
        }
    }
}
