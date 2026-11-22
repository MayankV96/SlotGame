using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main game controller: picks random results (RNG), spins the reels,
/// checks for a win and updates the balance.
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
        bool allSame = true;
        for (int r = 1; r < reels.Length; r++)
        {
            if (results[r][1] != results[0][1]) allSame = false;
        }

        int win = allSame ? bet * results[0][1].payoutMultiplier : 0;
        balance += win;
        UpdateUI(win > 0 ? "WIN " + win : "Try again");

        isSpinning = false;
        spinButton.interactable = true;
    }

    private IEnumerator RunReel(Reel reel, SymbolData[] result, float time, System.Action onDone)
    {
        yield return reel.Spin(result, time, GetRandomSymbol);
        onDone();
    }

    private void UpdateUI(string message)
    {
        balanceText.text = "Balance: " + balance;
        winText.text = message;
    }
}