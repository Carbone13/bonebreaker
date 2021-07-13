namespace Bonebreaker.Physics
{
    public struct AABB
    {
        public int2 Min;
        public int2 Max;

        public AABB (int2 position, int2 size)
        {
            Min = position;
            Max = position + size;
        }
    }
}