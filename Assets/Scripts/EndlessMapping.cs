using System.Collections.Generic;
using UnityEngine;

public class EndlessMap : MonoBehaviour
{
    public Transform player;
    public GameObject[] mapTiles; // Prefabs of different map tiles
    public int mapRadius = 3; // How many tiles around the player in a grid

    private Vector2 lastPlayerPosition;
    private Dictionary<Vector2, GameObject> activeTiles = new Dictionary<Vector2, GameObject>();

    void Start()
    {
        lastPlayerPosition = new Vector2(Mathf.Floor(player.position.x), Mathf.Floor(player.position.y));
        UpdateMap();
    }

    void Update()
    {
        Vector2 currentPlayerPosition = new Vector2(Mathf.Floor(player.position.x), Mathf.Floor(player.position.y));
        if (currentPlayerPosition != lastPlayerPosition)
        {
            lastPlayerPosition = currentPlayerPosition;
            UpdateMap();
        }
    }

    void UpdateMap()
    {
        // Get the player's position in tile space
        Vector2 playerTilePos = new Vector2(Mathf.Floor(player.position.x), Mathf.Floor(player.position.y));

        // Create new tiles around the player
        for (int x = -mapRadius; x <= mapRadius; x++)
        {
            for (int y = -mapRadius; y <= mapRadius; y++)
            {
                Vector2 tilePos = new Vector2(playerTilePos.x + x, playerTilePos.y + y);

                if (!activeTiles.ContainsKey(tilePos))
                {
                    GameObject newTile = Instantiate(mapTiles[Random.Range(0, mapTiles.Length)], tilePos, Quaternion.identity);
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