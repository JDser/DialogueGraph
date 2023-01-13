using System;
using System.Collections.Generic;

namespace DialogueEditor
{
    public interface IGraphEvents
    {
        public void Validate();
    
        public void Remove();
    
        public void RemapConnections(Dictionary<Guid, Guid> remappedGuids);
    }
}