using UnityEngine;

namespace CannonMonke
{
    [CreateAssetMenu(fileName = "CollectibleData", menuName = "CannonMonke/Collectible Data")]

    public class CollectibleData : EntityData 
    {
        public int score;
        // Additional properties specific to collectibles
    }
}
