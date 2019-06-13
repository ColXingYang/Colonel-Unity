using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("UI/LongClickButton")]
public class LongClickButton : Button {

    [Serializable]
    public class LongClickEvent : UnityEvent { }

    [SerializeField]
    private LongClickEvent _onLongClick = null;
    public LongClickEvent OnLongClick
    {
        get
        {
            return _onLongClick;
        }

        set
        {
            _onLongClick = value;
        }
    }

    private DateTime _firstTime = default(DateTime);
    private DateTime _secondTime = default(DateTime);

    private void Press()
    {
        if (!IsActive() || !IsInteractable())
            return;

        if (null != OnLongClick)
        {
            OnLongClick.Invoke();
        }

        ResetTime();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (_firstTime.Equals(default(DateTime)))
            _firstTime = DateTime.Now;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        // 在鼠标抬起的时候进行事件触发，时差大于600ms触发
        if (!_firstTime.Equals(default(DateTime)))
        {
            _secondTime = DateTime.Now;
        }

        if (!_firstTime.Equals(default(DateTime)) && !_secondTime.Equals(default(DateTime)))
        {
            var intervalTime = _secondTime - _firstTime;
            float milliSeconds = intervalTime.Seconds * 1000 + intervalTime.Milliseconds;
            if (milliSeconds > 600)
            {
                Press();
            }
            else
            {
                ResetTime();
            }
        }
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
