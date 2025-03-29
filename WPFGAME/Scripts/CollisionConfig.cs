namespace WPFGAME
{
    public class CollisionConfig
    {
        public HashSet<int> CollidableBlocks { get; set; }

        public CollisionConfig()
        {
            CollidableBlocks = new HashSet<int> { 2, 3, 4, 6 }; // Default collidable blocks (grass, sand, stone, dirt)
        }

        public bool IsCollidable(int blockType)
        {
            return CollidableBlocks.Contains(blockType);
        }
    }
}
