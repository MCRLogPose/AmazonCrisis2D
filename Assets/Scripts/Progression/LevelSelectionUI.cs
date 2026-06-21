using UnityEngine;

public class LevelSelectionUI : MonoBehaviour
{
    public LevelButtonUI[] levelButtons;

    private void OnEnable()
    {
        RefreshLevels();
    }

    public void RefreshLevels()
    {
        for (int i = 0;
             i < levelButtons.Length;
             i++)
        {
            levelButtons[i].Refresh(
                LevelManager.Instance.levels[i]
            );
        }
    }
}