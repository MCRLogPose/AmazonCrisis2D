using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthManager : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] public static int health = 3;
    [SerializeField] private Image[] healthIcons; // Array de imágenes para representar la salud
    [SerializeField] private Sprite fullHeart; // Imagen para corazón lleno
    [SerializeField] private Sprite emptyHeart; // Imagen para corazón vacío

    void Update()
    {
        for (int i = 0; i < healthIcons.Length; i++)
        {
            if (i < health)
                healthIcons[i].sprite = fullHeart;  // Establece las imágenes correspondientes a la salud actual a corazón lleno
            else
                healthIcons[i].sprite = emptyHeart; // Establece las imágenes restantes a corazón vacío
        }
    }

}
