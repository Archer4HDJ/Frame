using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// UGUI滑动接触到UI物体触发事件
/// </summary>
public class EventTriggerSlideEvent : MonoBehaviour {

    public CallBack<PointerEventData> OnBeginSlide;
    public CallBack<int, GameObject, PointerEventData> OnSlide;
    public CallBack OnEndSlide;
    /// <summary>
    /// 是否在触发物体上滑动也能触发
    /// </summary>
    public bool isTriggerOnChild = false;
    /// <summary>
    /// 需要滑动触发的物体
    /// </summary>
    public List<GameObject> triggerObject = new List<GameObject>();
	// Use this for initialization
	void Start () {

        EventTrigger trigger = null;
        trigger = GetComponent<EventTrigger>();
        if (trigger == null)
            trigger= gameObject.AddComponent<EventTrigger>();

        trigger.triggers.Add(AddEvent(EventTriggerType.BeginDrag, OnBeginDrag));
        trigger.triggers.Add(AddEvent(EventTriggerType.EndDrag, OnEndDrag));

        for (int i = 0; i < triggerObject.Count; i++)
        {
            GameObject obj = triggerObject[i];
            EventTrigger t = obj.AddComponent<EventTrigger>();
            t.triggers.Add(AddEvent(EventTriggerType.PointerEnter, OnSelectObjectEnter));

            if (!isTriggerOnChild)
            {
                EventTriggerSlideEvent sE = obj.AddComponent<EventTriggerSlideEvent>();
                sE.isTriggerOnChild = true;
                sE.OnBeginSlide = (p) =>
                {
                    OnBeginDrag(p);
                    p.pointerEnter = p.pointerDrag;
                    OnSelectObjectEnter(p);
                };
                sE.OnEndSlide = () =>
                {
                    OnEndDrag(null);
                };

                sE.OnSlide = (index, o, p) =>
                {
                    OnSelectObjectEnter(p);
                };
                sE.triggerObject.AddRange(triggerObject);
            }
        }
    }

    private void OnSelectObjectEnter(BaseEventData arg0)
    {
        if (beginDrag)
        {
            if (OnSlide != null)
            {
                PointerEventData pe = (PointerEventData)arg0;
                OnSlide(indexSlide, pe.pointerEnter,pe);
            }
            indexSlide++;
        }
    }

    private bool beginDrag = false;
    private int indexSlide = 0;
    private void OnBeginDrag(BaseEventData arg0)
    {
        beginDrag = true;
        indexSlide = 0;
        if (OnBeginSlide != null)
            OnBeginSlide((PointerEventData)arg0);
    }
    private void OnEndDrag(BaseEventData arg0)
    {
        beginDrag = false;
        if (OnEndSlide != null)
            OnEndSlide();
    }

  

    private EventTrigger.Entry AddEvent(EventTriggerType tType, UnityAction<BaseEventData> eventFun)
    {
        UnityAction<BaseEventData> click = new UnityAction<BaseEventData>(eventFun);
        EventTrigger.Entry myclick = new EventTrigger.Entry();
        myclick.eventID = tType;
        myclick.callback.AddListener(click);
        return myclick;
    }
       
}
