using System;

namespace DialogueEditor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SearchEntryAttribute : Attribute
    {
        public readonly string[] titles;

        public int order { get; set; }

        public SearchEntryAttribute(params string[] titles)
        {
            this.titles = titles;

            this.order = 999;
        }

        public SearchEntryAttribute(string categoryName, float r, float g, float b, params string[] titles)
        {
            this.titles = titles;

            this.order = 999;

            ColorSpaceManager.AddCategoryColor(categoryName, new UnityEngine.Color(r, g, b));
        }  
    }
}