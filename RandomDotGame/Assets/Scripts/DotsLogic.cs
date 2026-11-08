using System.Collections.Generic;
using UnityEngine;
using RDG;

public class DotsLogic : MonoBehaviour
{
    public class Dot
    {
        public GameObject dot;
        public int fingerId;
        public int colorIndex;
    }

    [HideInInspector]
    public bool isWinner = false;

    public GameObject dot;

    [Tooltip("Seconds without fingers being added/removed before a winner is chosen")]
    [SerializeField] float timeToSelectWinner = 2.5f;

    private float timeSinceLastChange = 0.0f;
    private Dot winner;

    // Active dots indexed by the finger that owns them
    private readonly Dictionary<int, Dot> fingerToDot = new Dictionary<int, Dot>();
    // Reusable buffers to avoid allocations every frame
    private readonly HashSet<int> activeFingers = new HashSet<int>();
    private readonly List<int> fingersToRemove = new List<int>();
    private readonly List<int> freeColors = new List<int>();

    public int DotsCount => fingerToDot.Count;

    void Update()
    {
        timeSinceLastChange += Time.deltaTime;
        if (timeSinceLastChange > timeToSelectWinner && fingerToDot.Count > 1 && !isWinner)
        {
            SelectWinner();
        }
    }

    public bool HasDot(int fingerId)
    {
        return fingerToDot.ContainsKey(fingerId);
    }

    public bool IsWinnerFinger(int fingerId)
    {
        return winner != null && winner.fingerId == fingerId;
    }

    public void MoveDot(Touch touch)
    {
        if (!fingerToDot.TryGetValue(touch.fingerId, out Dot d) || d.dot == null) return;
        Vector3 pos = d.dot.transform.position;
        d.dot.transform.position = new Vector3(touch.position.x, touch.position.y, pos.z);
    }

    public void AddDot(Touch touch)
    {
        if (isWinner) return;

        // If, for any reason, this finger still has a dot (lost Ended/Canceled event), recycle it
        if (fingerToDot.ContainsKey(touch.fingerId)) RemoveDot(touch.fingerId);

        timeSinceLastChange = 0.0f;
        GameObject _dot = Instantiate(dot, transform);
        _dot.transform.position = touch.position;
        _dot.transform.localPosition = new Vector3(_dot.transform.localPosition.x, _dot.transform.localPosition.y, -2.0f);

        int colorIndex = PickUnusedColor();
        _dot.GetComponent<DotScript>().SetColor(colorIndex);

        fingerToDot[touch.fingerId] = new Dot { dot = _dot, fingerId = touch.fingerId, colorIndex = colorIndex };
    }

    public void RemoveDot(int fingerId)
    {
        if (!fingerToDot.TryGetValue(fingerId, out Dot d)) return;
        if (d.dot != null) Destroy(d.dot);
        fingerToDot.Remove(fingerId);

        // Someone lifted a finger before the draw: restart the countdown
        if (!isWinner) timeSinceLastChange = 0.0f;
    }

    /// <summary>
    /// Removes every dot whose finger is no longer touching the screen.
    /// Safety net for touches whose Ended/Canceled phase was never received
    /// (very common when several fingers are placed/lifted quickly).
    /// </summary>
    public void SyncWithTouches()
    {
        if (fingerToDot.Count == 0) return;

        activeFingers.Clear();
        for (int i = 0; i < Input.touchCount; ++i)
        {
            Touch t = Input.GetTouch(i);
            if (t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled)
                activeFingers.Add(t.fingerId);
        }

        fingersToRemove.Clear();
        foreach (var pair in fingerToDot)
        {
            if (!activeFingers.Contains(pair.Key)) fingersToRemove.Add(pair.Key);
        }
        for (int i = 0; i < fingersToRemove.Count; ++i) RemoveDot(fingersToRemove[i]);
    }

    private int PickUnusedColor()
    {
        freeColors.Clear();
        for (int i = 0; i < DotScript.ColorCount; ++i)
        {
            bool used = false;
            foreach (var d in fingerToDot.Values)
            {
                if (d.colorIndex == i) { used = true; break; }
            }
            if (!used) freeColors.Add(i);
        }
        if (freeColors.Count == 0) return Random.Range(0, DotScript.ColorCount);
        return freeColors[Random.Range(0, freeColors.Count)];
    }

    private void SelectWinner()
    {
        int target = Random.Range(0, fingerToDot.Count);
        int i = 0;
        foreach (var d in fingerToDot.Values)
        {
            if (i++ == target) { winner = d; break; }
        }
        if (winner == null) return;

        isWinner = true;
        winner.dot.GetComponent<DotScript>().Uwin();

        // Remove every loser dot but keep the winner tracked so it can still be dragged
        fingersToRemove.Clear();
        foreach (var pair in fingerToDot)
        {
            if (pair.Value != winner) fingersToRemove.Add(pair.Key);
        }
        for (int k = 0; k < fingersToRemove.Count; ++k) RemoveDot(fingersToRemove[k]);

        if (PlayerPrefs.GetInt("Vibracion", 1) == 1) Vibration.Vibrate(600, 10);
    }

    public void _Reset()
    {
        foreach (var d in fingerToDot.Values)
        {
            if (d.dot != null) Destroy(d.dot);
        }
        if (winner != null && winner.dot != null) Destroy(winner.dot);
        winner = null;
        fingerToDot.Clear();
        isWinner = false;
        timeSinceLastChange = 0.0f;
    }
}
