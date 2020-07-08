using System;
using System.Collections.Generic;

namespace ModelHolder
{
    [Serializable]
    public class ModelItem
    {
        public ModelItem Parent { get; set; }
        public List<ModelItem> Childs { get; set; } = new List<ModelItem>();
        public List<ModelProperty> Properies { get; set; } = new List<ModelProperty>();

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Descriptor { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
