namespace WPFGAME
{
    public class CollisionConfig
    {
        public HashSet<int> CollidableBlocks { get; set; }

        public CollisionConfig()
        {
            CollidableBlocks = new HashSet<int> { 2, 3, 4, 6, 7, 8, 13, 14, 15, 16, 17, 18, 19, 20}; // Default collidable blocks (grass, sand, stone, dirt)
        }

        public bool IsCollidable(int blockType)
        {
            return CollidableBlocks.Contains(blockType);
        }
    }
}
