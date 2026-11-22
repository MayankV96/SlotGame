using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls ONE reel. It holds a tall strip of 20 pictures. The bottom 3 show
/// what is on screen now and the top 3 are the new result. The strip slides
/// downwards so the new result arrives smoothly.
/// </summary>
public class Reel : MonoBehaviour
{
    [SerializeField] private RectTransform content;   // the moving strip
    [SerializeField] private Image symbolPrefab;      // picture template
    [SerializeField] private float symbolHeight = 150f;

    private const int StripLength = 20;
    private Image[] slots;
    private SymbolData[] visible = new SymbolData[3]; // symbols currently on screen

    // Called once at the start: builds the strip and shows 3 random symbols.
    public void Init(Func<SymbolData> getRandom)
    {
        slots = new Image[StripLength];
        for (int i = 0; i < StripLength; i++)
        {
            slots[i] = Instantiate(symbolPrefab, content);
            slots[i].rectTransform.anchoredPosition = new Vector2(0, -i * symbolHeight);
        }

        for (int i = 0; i < 3; i++)
        {
            visible[i] = getRandom();
            slots[i].sprite = visible[i].sprite;
        }
        content.anchoredPosition = Vector2.zero;
    }

    // Spins, then stops showing 'result' (top row first).
    public IEnumerator Spin(SymbolData[] result, float duration, Func<SymbolData> getRandom)
    {
        float startY = (StripLength - 3) * symbolHeight;

        // 1) Put the CURRENT symbols at the bottom of the strip and jump there.
        //    It looks identical, so the player sees no jump.
        for (int i = 0; i < 3; i++)
            slots[StripLength - 3 + i].sprite = visible[i].sprite;
        content.anchoredPosition = new Vector2(0, startY);

        // 2) Put the NEW result at the top; fill the middle with random symbols.
        for (int i = 0; i < 3; i++)
            slots[i].sprite = result[i].sprite;
        for (int i = 3; i < StripLength - 3; i++)
            slots[i].sprite = getRandom().sprite;

        // 3) Slide down. Ease-out = fast at first, slows down to stop.
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            float eased = 1f - Mathf.Pow(1f - k, 3f);
            content.anchoredPosition = new Vector2(0, Mathf.Lerp(startY, 0f, eased));
            yield return null;
        }

        content.anchoredPosition = Vector2.zero;
        visible = result;
    }
}