using UnityEngine;
using UnityEngine.UI;

public class HelpPointsManager : MonoBehaviour
{
    public static HelpPointsManager instance;

    public Slider helpSlider;

    public int currentPoints = 0;

    public int maxPoints = 100;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        helpSlider.maxValue = maxPoints;

        helpSlider.value = currentPoints;
    }

    public void AddPoints(int points)
    {
        currentPoints += points;

        if (currentPoints > maxPoints)
        {
            currentPoints = maxPoints;
        }

        helpSlider.value = currentPoints;

        if (currentPoints >= maxPoints)
        {
            UnlockNextLevel();
        }
    }

    void UnlockNextLevel()
    {
        Debug.Log("Siguiente nivel desbloqueado");
    }
}