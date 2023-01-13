using System.Collections.Generic;

namespace DialogueEditor
{
    public interface IHasObjectsToAdd
    {
        public IEnumerable<UnityEngine.Object> GetObjects();
    }
}