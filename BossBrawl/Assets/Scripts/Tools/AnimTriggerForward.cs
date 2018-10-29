using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimTriggerForward : MonoBehaviour
{

    public UnityEvent[] events;

    public void CallEvent(int index)
    {
        events[index].Invoke();
    }
}
