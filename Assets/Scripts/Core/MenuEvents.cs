using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuEvents : MonoBehaviour
{
    public void LoadLevel(int levelIndex)
    {
        string sceneName = "";

        // Mapea tanto 0-based como 1-based para los niveles definidos por el usuario:
        // Tutorial (0 o 1) -> Level 1 (1 o 2) -> Level 2 (2 o 3) -> Level 3/nivel 4 (3 o 4) -> Final (4 o 5)
        switch (levelIndex)
        {
            case 0:
                sceneName = "Menu";
                break;
            case 1:
                sceneName = "Tutorial";
                break;
            case 2:
                sceneName = "Level1";
                break;
            case 3:
                sceneName = "Level2";
                break;
            case 4:
                sceneName = "Level3";
                break;
            case 5:
                sceneName = "Final";
                break;
            default:
                SceneManager.LoadScene(levelIndex);
                return;
        }

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(levelIndex);
        }
    }
}
