using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BlinkController : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Color current;
    Color currentNoAlpha;
    bool isBlinking = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        current = spriteRenderer.color;
        currentNoAlpha = new Color(current.r, current.g, current.b, 0);
        spriteRenderer.color = currentNoAlpha;
    }

    public void StartBlink(int count)
    {
        if (!isBlinking)
        {
            StartCoroutine(DoStartBlink(count));
        }
    }

    IEnumerator DoStartBlink(int count)
    {
        isBlinking = true;

        for (int i = 0; i < count; i++)
        {
            spriteRenderer.color = currentNoAlpha;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = current;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = currentNoAlpha;
        }

        isBlinking = false;
    }
}
