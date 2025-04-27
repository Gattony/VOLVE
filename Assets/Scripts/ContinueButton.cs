using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    public void ButtonSelection()
    {
        Time.timeScale = 1f; // Reset time scale to normal
        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScreen");
    }

}
