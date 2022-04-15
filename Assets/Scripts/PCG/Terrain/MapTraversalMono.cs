using UnityEngine;

namespace ViralVial.PCG.Terrain
{
    public class MapTraversalMono : MonoBehaviour
    {

        private MapTraversal mapTraversal;
    
        public void InitializeMap(int seed, int x, int y)
        {
            mapTraversal = new MapTraversal(seed, x, y, transform);
        }

        public void GoDirection(WallDirection direction)
        {
            mapTraversal.Move(direction);
        }
    }
}
