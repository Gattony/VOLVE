using System.Collections.Generic;
using UnityEngine;

public class EndlessMapping2 : MonoBehaviour
{
    public Transform player;
    public GameObject[] mapTiles; // Prefabs of different map tiles
    public int mapRadius = 10; // How many tiles around the player in a grid
    public float tileSize = 3f; // Size of each tile

    private Vector2 lastPlayerTilePosition;
    private Dictionary<Vector2, GameObject> activeTiles = new Dictionary<Vector2, GameObject>();

    void Start()
    {
        lastPlayerTilePosition = GetPlayerTilePosition();
        UpdateMap();
    }

    void Update()
    {
        // Calculate the player's current position in tile space
        Vector2 currentPlayerTilePosition = GetPlayerTilePosition();

        // Check if the player has moved to a new tile
        if (currentPlayerTilePosition != lastPlayerTilePosition)
        {
            lastPlayerTilePosition = currentPlayerTilePosition;
            UpdateMap();
        }
    }

    Vector2 GetPlayerTilePosition()
    {
        // Calculate the player's position in tile space, snapping to tileSize
        return new Vector2(
            Mathf.Floor(player.position.x / tileSize),
            Mathf.Floor(player.position.y / tileSize)
        );
    }

    void UpdateMap()
    {
        // Get the player's position in tile space and render's the opposing tilemaps
        Vector2 playerTilePos = GetPlayerTilePosition();

        // Create new tiles around the player
        for (int x = -mapRadius; x <= mapRadius; x++)
        {
            for (int y = -mapRadius; y <= mapRadius; y++)
            {
                Vector2 tilePos = new Vector2(playerTilePos.x + x, playerTilePos.y + y);

                if (!activeTiles.ContainsKey(tilePos))
                {
                    // Calculate world position for the tile
                    Vector3 worldPos = new Vector3(tilePos.x * tileSize, tilePos.y * tileSize, 0);
                    GameObject newTile = Instantiate(mapTiles[Random.Range(0, mapTiles.Length)], worldPos, Quaternion.identity);
                    activeTiles.Add(tilePos, newTile);
                }
            }
        }

        // Remove tiles that are too far from the player
        List<Vector2> tilesToRemove = new List<Vector2>();
        foreach (var tilePos in activeTiles.Keys)
        {
            if (Vector2.Distance(tilePos, playerTilePos) > mapRadius)
            {
                tilesToRemove.Add(tilePos);
            }
        }

        foreach (var tilePos in tilesToRemove)
        {
            Destroy(activeTiles[tilePos]);
            activeTiles.Remove(tilePos);
        }
    }
}
