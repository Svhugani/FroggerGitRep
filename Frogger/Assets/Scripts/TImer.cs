using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TImer : MonoBehaviour
{
    private RectTransform _timeBar;
    private float _maxTime;
    private float _maxWidth;
    private float _timeLeft;
    public GameObject timesUpText;
    private Frog _frogSc;

    public Frog SetFrogRef
    {
        set {_frogSc = value;}
    }
    public float MaxTime
    {
        set{_maxTime = value;}
    }

    public float TimeLeft
    {
        get{return _timeLeft;}
    }

    public void TimeReset()
    {
        _timeLeft = _maxTime;
    }

    void Start()
    {
        timesUpText.SetActive(false);
        _timeBar = this.GetComponent<RectTransform>();
        _timeLeft = _maxTime;
        _maxWidth= _timeBar.sizeDelta.x;
        StartCoroutine(StartTimer(1f));
    }

    
    IEnumerator StartTimer(float time)
    {
        while (_timeLeft > 0)
        {
            _timeLeft -= time;
            _timeBar.sizeDelta= new Vector2((_timeLeft / _maxTime) * _maxWidth, _timeBar.sizeDelta.y);
            yield return new WaitForSeconds(time);
        }
        

    }


    void Update()
    {
        if (_timeLeft <= 0)
        {
            _frogSc.ResetPosition(true);
            _timeLeft = _maxTime;
        }
    }
}
