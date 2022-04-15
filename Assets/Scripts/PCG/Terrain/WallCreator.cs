using UnityEngine;

namespace ViralVial.PCG.Terrain
{
    public class WallCreator : MonoBehaviour
    {
        void Start()
        {
            var mapTraversal = gameObject.GetComponent<MapTraversalMono>();
            var baseObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            baseObject.isStatic = true;
            baseObject.AddComponent<TriggerMapWall>();
            baseObject.GetComponent<BoxCollider>().isTrigger = true;
            baseObject.GetComponent<MeshRenderer>().enabled = false;
            baseObject.transform.localScale = new Vector3(1, MapConfig.TileSize.y + MapConfig.AdditionalWallHeight, 1);
            baseObject.transform.position += Vector3.up * (baseObject.transform.localScale.y / 2);

            // North Wall
            var northWall = Instantiate(baseObject, transform);
            northWall.transform.position += Vector3.forward * (2 * MapConfig.TileSize.z - MapConfig.WallDistanceFromEdge);
            northWall.GetComponent<TriggerMapWall>().SetMapTraversal(mapTraversal, WallDirection.North);
            northWall.transform.localScale += Vector3.right * (MapConfig.TileSize.x * 4 - 2 * MapConfig.WallDistanceFromEdge);
            northWall.name = "North Wall";

            // South Wall
            var southWall = Instantiate(baseObject, transform);
            southWall.transform.position += Vector3.back * (2 * MapConfig.TileSize.z - MapConfig.WallDistanceFromEdge);
            southWall.GetComponent<TriggerMapWall>().SetMapTraversal(mapTraversal, WallDirection.South);
            southWall.transform.localScale += Vector3.right * (MapConfig.TileSize.x * 4 - 2 * MapConfig.WallDistanceFromEdge);
            southWall.name = "South Wall";

            // East Wall
            var eastWall = Instantiate(baseObject, transform);
            eastWall.transform.position += Vector3.right * (2 * MapConfig.TileSize.x - MapConfig.WallDistanceFromEdge);
            eastWall.GetComponent<TriggerMapWall>().SetMapTraversal(mapTraversal, WallDirection.East);
            eastWall.transform.localScale += Vector3.forward * (MapConfig.TileSize.z * 4 - 2 * MapConfig.WallDistanceFromEdge);
            eastWall.name = "East Wall";

            // West Wall
            var westWall = Instantiate(baseObject, transform);
            westWall.transform.position += Vector3.left * (2 * MapConfig.TileSize.x - MapConfig.WallDistanceFromEdge);
            westWall.GetComponent<TriggerMapWall>().SetMapTraversal(mapTraversal, WallDirection.West);
            westWall.transform.localScale += Vector3.forward * (MapConfig.TileSize.z * 4 - 2 * MapConfig.WallDistanceFromEdge);
            westWall.name = "West Wall";

            Destroy(baseObject);
        }
    }
}
