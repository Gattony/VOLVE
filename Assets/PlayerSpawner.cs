using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform playerSpawnPoint;

    void Start()
    {
        if (CharacterSelectionData.SelectedCharacterPrefab != null)
        {
            GameObject player = Instantiate(CharacterSelectionData.SelectedCharacterPrefab, playerSpawnPoint.position, Quaternion.identity);

            player.tag = "Player";

            var control = player.GetComponent<PlayerControl>();
            if (control != null)
            {
                control.enabled = true;
                control.isInCharacterSelect = false; 
            }
        }
        else
        {
            Debug.LogError("No character was selected!");
        }
    }
}
