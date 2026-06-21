using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonUI : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int levelIndex;

    [Header("UI References")]
    [SerializeField] private Image levelImage;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Button levelButton;

    [Header("Sprites")]
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;

    [Header("Optional")]
    [SerializeField] private bool showNumberOnlyIfUnlocked = true;

    public int LevelIndex => levelIndex;

    public void Refresh(LevelData data)
    {
        bool unlocked = data.unlocked;

        if (levelImage != null)
        {
            levelImage.sprite =
                unlocked
                ? unlockedSprite
                : lockedSprite;
        }

        if (levelButton != null)
        {
            levelButton.interactable = unlocked;
        }

        if (levelText != null)
        {
            levelText.text =
                data.levelNumber.ToString();

            levelText.gameObject.SetActive(
                !showNumberOnlyIfUnlocked ||
                unlocked
            );
        }
    }
}