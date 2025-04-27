using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSpawner : MonoBehaviour
{
    public Transform playerSpawnPoint;
    public JoystickMovement movementJoystick;
    public JoystickMovement actionJoystick;
    public CameraController cameraController;
    public EndlessMapping2 endlessMapController;
    public EnemySpawner enemySpawner;
    public UpgradeDisplay effectSpawnPoint;

    [Header("Level UI")]
    public Image expBarFill;
    public TMP_Text levelText;
    public RectTransform expBarContainer;

    [Header("EXP Bar Effects")]
    public ParticleSystem expFillEffect;
    public RectTransform expBarTransform;

    [Header("Heart UI")]
    public Image heartContainerPrefab;
    public Transform heartContainer;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Ammo UI")]
    public RectTransform ammoBarContainer;
    public Image ammoBarFill;
    public ParticleSystem ammoEffect;

    public RectTransform DeathScreen;
    public GameObject GameplayCanvas;

    void Start()
    {
        if (CharacterSelectionData.SelectedCharacterPrefab != null)
        {
            GameObject player = Instantiate(CharacterSelectionData.SelectedCharacterPrefab, playerSpawnPoint.position, Quaternion.identity);
            player.tag = "Player";

            var character = player.GetComponent<PlayerCharacter>();
            var weapon = player.GetComponentInChildren<Weapon>();
            var control = player.GetComponent<PlayerControl>();

            Transform effectPoint = player.transform.Find("EffectSpawnPoint");

            if (effectPoint != null && UpgradeDisplay.Instance != null)
            {
                UpgradeDisplay.Instance.effectSpawnPoint = effectPoint;
            }
            else
            {
                Debug.LogWarning("EffectSpawnPoint not found on player prefab!");
            }

            if (control != null)
            {
                control.enabled = true;
                control.isInCharacterSelect = false;
                control.movementJoystick = movementJoystick;
                control.actionJoystick = actionJoystick;
            }

            if (weapon != null)
            {
                weapon.weaponJoystick = actionJoystick;
                weapon.ammoBarContainer = ammoBarContainer;
                weapon.ammoBarFill = ammoBarFill;
                weapon.ammoEffect = ammoEffect;
            }

            if (cameraController != null)
            {
                cameraController.player = player.transform;
                cameraController.actionJoystick = actionJoystick;
            }

            if (endlessMapController != null)
            {
                endlessMapController.player = player.transform;
            }

            if (enemySpawner != null)
            {
                enemySpawner.player = player.transform;
            }

            if (character != null)
            {
                character.expBarFill = expBarFill;
                character.levelText = levelText;
                character.expBarContainer = expBarContainer;
                character.expFillEffect = expFillEffect;
                character.expBarTransform = expBarTransform;

                character.heartContainerPrefab = heartContainerPrefab;
                character.heartContainer = heartContainer;
                character.fullHeart = fullHeart;
                character.emptyHeart = emptyHeart;

                character.gameplayCanvas = GameplayCanvas;
                character.deathScreen = DeathScreen;
            }
        }
        else
        {
            Debug.LogError("No character was selected!");
        }
    }
}
