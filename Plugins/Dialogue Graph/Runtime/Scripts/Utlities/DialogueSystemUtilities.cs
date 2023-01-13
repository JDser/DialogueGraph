namespace DialogueSystem
{
    public static class ArrayExtensions
    {
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array == null || array.Length == 0;
        }
    }

    public static class NodeExtensions
    {
        public static bool IsConditionsMet(this Node node)
        {
            for (int i = 0; i < node.Properties.Length; i++)
            {
                if (node.Properties[i] is IConditionProperty prop && prop.CheckCondition() == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}