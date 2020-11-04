namespace Core.Utility {
    public static class StringExtensions {
        public static bool NPArrayContains(this string[] array, string target) {
            for (int i = 0; i < array.Length; i++) {
                if (array[i].Equals(target)) {
                    return true;
                }
            }
            return false;
        }
    }
}
