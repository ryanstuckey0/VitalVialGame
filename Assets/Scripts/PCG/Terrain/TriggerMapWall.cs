using UnityEngine;

namespace ViralVial.PCG.Terrain
{
    public class TriggerMapWall : MonoBehaviour
    {
        private MapTraversalMono mapTraversal;
        private WallDirection direction;

        public void SetMapTraversal(MapTraversalMono mapTraversal, WallDirection direction)
        {
            this.mapTraversal = mapTraversal;
            this.direction = direction;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && mapTraversal != null)
            {
                mapTraversal.GoDirection(direction);
            }
        }
    }
}
