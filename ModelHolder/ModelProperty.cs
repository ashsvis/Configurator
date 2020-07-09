using System;

namespace ModelHolder
{
    [Serializable]
    public class ModelProperty
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public Type Type { get; set; }
        public object Value { get; set; }
        public int Size { get; set; }
        public int Decimal { get; set; }
    }
}
