using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private float parallaxFactor = 0.5f;
    private Transform cameraTranform;
    private Vector3 previousCameraPosition;
    private float spriteWidth, startPosition;

    void Start()
    {
        cameraTranform = Camera.main.transform;
        previousCameraPosition = cameraTranform.position;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        startPosition = transform.position.x;
    }

    void Update()
    {
        float deltaX = (cameraTranform.position.x - previousCameraPosition.x) * parallaxFactor;
        float moveAmount = cameraTranform.position.x * (1 - parallaxFactor);
        transform.Translate(new Vector3(deltaX, 0, 0));
        previousCameraPosition = cameraTranform.position;

        if (moveAmount > startPosition + spriteWidth)
        {
            transform.Translate(new Vector3(spriteWidth, 0, 0));
            startPosition += spriteWidth;
        }
        else if (moveAmount < startPosition - spriteWidth)
        {
            transform.Translate(new Vector3(-spriteWidth, 0, 0));
            startPosition -= spriteWidth;
        }
    }
}
