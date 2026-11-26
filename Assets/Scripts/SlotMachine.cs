using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main game controller: picks random results (RNG), spins the reels,
/// checks for a win, updates the balance, and plays feedback effects.
/// </summary>
public class SlotMachine : MonoBehaviour
{
    [SerializeField] private List<SymbolData> symbols;  // all possible symbols
    [SerializeField] private Reel[] reels;               // the 3 reels
    [SerializeField] private Button spinButton;
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text winText;
    [SerializeField] private int balance = 1000;
    [SerializeField] private int bet = 10;

    [Header("Audio (optional - leave empty if you have no sounds)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip spinClip;
    [SerializeField] private AudioClip stopClip;
    [SerializeField] private AudioClip winClip;

    private bool isSpinning;

    private void Start()
    {
        foreach (var reel in reels) reel.Init(GetRandomSymbol);
        UpdateUI("");
    }

    // RNG: weighted random pick. A symbol with weight 20 appears
    // twice as often as one with weight 10.
    private SymbolData GetRandomSymbol()
    {
        int total = 0;
        foreach (var s in symbols) total += s.weight;

        int roll = Random.Range(0, total);
        foreach (var s in symbols)
        {
            if (roll < s.weight) return s;
            roll -= s.weight;
        }
        return symbols[0];
    }

    // Connected to the SPIN button's OnClick event.
    public void OnSpin()
    {
        if (isSpinning || balance < bet) return;
        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        isSpinning = true;
        spinButton.interactable = false;
        balance -= bet;
        UpdateUI("");
        PlaySound(spinClip);

        // 1) Decide the final result FIRST (3 reels x 3 rows).
        var results = new SymbolData[reels.Length][];
        for (int r = 0; r < reels.Length; r++)
        {
            results[r] = new SymbolData[3];
            for (int row = 0; row < 3; row++)
                results[r][row] = GetRandomSymbol();
        }

        // 2) Spin all reels. Later reels spin longer, so they stop one by one.
        int stillSpinning = reels.Length;
        for (int r = 0; r < reels.Length; r++)
        {
            StartCoroutine(RunReel(reels[r], results[r], 1.2f + r * 0.5f, () => stillSpinning--));
        }
        yield return new WaitUntil(() => stillSpinning == 0);

        // 3) Check the middle row (row index 1). All the same = win.
               // 3) Check the middle row (row index 1). Wilds count as any symbol.
        var payline = new SymbolData[reels.Length];
        for (int r = 0; r < reels.Length; r++) payline[r] = results[r][1];
        int win = CalculateWin(payline);
        balance += win;
        UpdateUI(win > 0 ? "WIN " + win : "Try again");

        if (win > 0)
        {
            PlaySound(winClip);
            StartCoroutine(PulseWinText());
        }

        isSpinning = false;
        spinButton.interactable = true;
    }

    private IEnumerator RunReel(Reel reel, SymbolData[] result, float time, System.Action onDone)
    {
        yield return reel.Spin(result, time, GetRandomSymbol);
        PlaySound(stopClip);
        onDone();
    }

    // Makes the win text grow and shrink once, so a win feels rewarding.
    private IEnumerator PulseWinText()
    {
        const float duration = 0.6f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float scale = 1f + 0.3f * Mathf.Sin(t / duration * Mathf.PI);
            winText.rectTransform.localScale = Vector3.one * scale;
            yield return null;
        }
        winText.rectTransform.localScale = Vector3.one;
    }


        // Returns the payout for one payline. A wild matches any symbol.
    private int CalculateWin(SymbolData[] payline)
    {
        // The first non-wild symbol decides what we need to match.
        SymbolData target = null;
        foreach (var s in payline)
        {
            if (!s.isWild) { target = s; break; }
        }
        if (target == null) target = payline[0];   // every symbol is a wild

        // Any symbol that is neither the target nor a wild means no win.
        foreach (var s in payline)
        {
            if (s != target && !s.isWild) return 0;
        }
        return bet * target.payoutMultiplier;
    }
    // Safe to call even if no sound is assigned.
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
    }

    private void UpdateUI(string message)
    {
        balanceText.text = "Balance: " + balance;
        winText.text = message;
    }
}