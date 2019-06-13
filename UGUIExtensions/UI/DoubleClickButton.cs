using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[AddComponentMenu("UI/DoubleClickButton")]
public class DoubleClickButton : Button {

    [Serializable]
    public class DoubleClickEvent : UnityEvent { }

    [SerializeField]
    private DoubleClickEvent _onDoubleClick = new DoubleClickEvent();

    public DoubleClickEvent OnDoubleClick
    {
        get
        {
            return _onDoubleClick;
        }

        set
        {
            _onDoubleClick = value;
        }
    }

    private DateTime _firstTime;
    private DateTime _secondTime;

    private void Press()
    {
        if (!IsActive() || !IsInteractable())
            return;

        if (null != OnDoubleClick)
        {
            OnDoubleClick.Invoke();
        }

        ResetTime();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);       
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("00000="+ eventData.clickCount);              
        if (_firstTime.Equals(default(DateTime)))
        {
            _firstTime = DateTime.Now;          
            StartCoroutine("Click", eventData);
        }
        else
        {
            _secondTime = DateTime.Now;
        }
                
        if (!_firstTime.Equals(default(DateTime)) && !_secondTime.Equals(default(DateTime)))
        {
            var intervalTime = _secondTime - _firstTime;
            float milliSeconds = intervalTime.Seconds * 1000 + intervalTime.Milliseconds;
            if (milliSeconds < 400)//时差小于400ms触发双击
            {
               // Debug.Log("double click");
                Press();
            }
            else
            {
                Debug.LogError("事实上由于时间超过了0.405s就已经走Click了，所以这里永远都不会运行");             
                ResetTime();
            }
        }
        
    }

    private IEnumerator Click(PointerEventData eventData)
    {
        yield return new WaitForSecondsRealtime(0.405f);
        if (!_firstTime.Equals(default(DateTime)))
        {
           // Debug.Log("Click");
            base.OnPointerClick(eventData);
            ResetTime();
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);      
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        ResetTime();
    }

    private void ResetTime()
    {
        _firstTime = default(DateTime);
        _secondTime = default(DateTime);
    }
}
