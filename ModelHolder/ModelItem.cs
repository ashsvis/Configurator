using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelHolder
{
    [Serializable]
    public class ModelItem
    {
        public ModelItem Parent { get; set; }
        public List<ModelItem> Childs { get; set; } = new List<ModelItem>();
        public List<ModelProperty> Properies { get; set; } = new List<ModelProperty>();

        public string Name
        {
            get
            {
                var nameProp = this.Properies.FirstOrDefault(x => x.Name == "Name");
                var name = nameProp?.Value ?? this.Id;
                return $"{name}";
            }
        }

        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
