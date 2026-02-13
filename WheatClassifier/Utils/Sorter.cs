namespace WheatClassifier.Utils
{
    public static class Sorter
    {
        public static List<T> MergeSortByKey<T>(List<T> items, Func<T, double> key)
        {
            if (items.Count <= 1) return items;

            int mid = items.Count / 2;
            var left = MergeSortByKey(items.GetRange(0, mid), key);
            var right = MergeSortByKey(items.GetRange(mid, items.Count - mid), key);

            return Merge(left, right, key);
        }

        private static List<T> Merge<T>(List<T> left, List<T> right, Func<T, double> key)
        {
            var result = new List<T>(left.Count + right.Count);
            int i = 0, j = 0;

            while (i < left.Count && j < right.Count)
            {
                if (key(left[i]) <= key(right[j]))
                    result.Add(left[i++]);
                else
                    result.Add(right[j++]);
            }

            while (i < left.Count) result.Add(left[i++]);
            while (j < right.Count) result.Add(right[j++]);

            return result;
        }
    }
}
