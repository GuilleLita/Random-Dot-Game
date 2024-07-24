using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class DotsLogic : MonoBehaviour
{
    public class Dot
    {
        public GameObject dot;
        public int index;
        public Dot()
        {
        }
    }

    [HideInInspector]
    public List<Dot> dots;
    [HideInInspector]
    public bool isWinner = false;

    public GameObject dot;

    private float timeSinceLastDot = 0.0f;
    private Dot winner;
    private int DotsCount = 0;

    private Dictionary<int, Dot> fingerToIndex = new Dictionary<int, Dot>();
    // Start is called before the first frame update
    void Start()
    {
        dots = new List<Dot>();
        isWinner = false;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastDot += Time.deltaTime;
        if (timeSinceLastDot > 2.5f && DotsCount >1 && !isWinner)
        {
            StartCoroutine(SelectWinner());
        }   
    }

    public bool isWinnnerFinger(Touch touch)
    {
        Dot dot = fingerToIndex[touch.fingerId];
        return dot == winner;
    }


    public void MoveDot(Touch touch)
    {
        Dot index = fingerToIndex[touch.fingerId];  
        index.dot.transform.position = touch.position;
    }

    public void AddDot(Touch touch)
    {
        timeSinceLastDot = 0.0f;
        GameObject _dot = Instantiate(dot, transform);
        _dot.transform.position = touch.position;
        _dot.transform.localPosition = new Vector3(_dot.transform.localPosition.x, _dot.transform.localPosition.y, -2.0f);
        _dot.GetComponent<DotScript>().SetRandomColor();
        //_dot.GetComponent<Image>().color = new Color(Random.Range(0.0f,1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f),1.0f);
        Dot newDot = new Dot();
        newDot.dot = _dot;
        newDot.index = dots.Count;
        dots.Add(newDot);
        fingerToIndex.Add(touch.fingerId, newDot);
        DotsCount++;

    }

    public void RemoveDot(int fingerindex)
    {
        if(fingerindex.Equals(null) || !fingerToIndex.ContainsKey(fingerindex)) return;
        Dot index = fingerToIndex[fingerindex];
        Destroy(index.dot);
        dots[index.index] = null;
        fingerToIndex.Remove(fingerindex);
        DotsCount--;
    }

    private Dot GetRandomDot()
    {
        
        int index = Random.Range(0, dots.Count);

        return dots[index];
    }

    IEnumerator SelectWinner()
    {
        do
        {
            winner = GetRandomDot();
        }while(winner == null);
        isWinner = true;
        winner.dot.GetComponent<DotScript>().Uwin();
        for (int i = 0; i < dots.Count; ++i)
        {

            if (dots[i] != null && dots[i] != winner)
            {
                Destroy(dots[i].dot);
                dots[i] = null;
            }
        }
        Handheld.Vibrate();
        yield return new WaitForSeconds(1.0f);
        
    }

    public void _Reset()
    {
        Destroy(winner.dot);
        winner = null;
        fingerToIndex.Clear();
        dots.Clear();
        DotsCount = 0;
        isWinner = false;
    }
}
