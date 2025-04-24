using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform playerSpawnPoint;
    public JoystickMovement movementJoystick;
    public JoystickMovement actionJoystick;

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

                control.movementJoystick = movementJoystick;
                control.actionJoystick = actionJoystick;
            }
            else
            {
                Debug.LogWarning("No PlayerControl found on spawned prefab!");
            }

            // Assign the joystick to the Weapon script
            var weapon = player.GetComponentInChildren<Weapon>();
            if (weapon != null)
            {
                weapon.weaponJoystick = actionJoystick;
            }
            else
            {
                Debug.LogWarning("No Weapon script found on spawned prefab!");
            }
        }
        else
        {
            Debug.LogError("No character was selected!");
        }
    }
}
