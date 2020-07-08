using System;

namespace ModelHolder
{
    [Serializable]
    public class ModelRoot : ModelItem
    {
        public ModelRoot()
        {
            Name = "Root";
        }
    }
}
