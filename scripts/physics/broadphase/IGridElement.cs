using System.Collections.Generic;

namespace Bonebreaker.Physics.Broadphase
{
    public interface IGridElement
    {
        public Dictionary<int2, IGridElement> NextElement { get; set; }

        public int2 Min { get; }
        public int2 Max { get; }

        public int ID { get; set; }
        
        public string Name { get; set; }
    }
}