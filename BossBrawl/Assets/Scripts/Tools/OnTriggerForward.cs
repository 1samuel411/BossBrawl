using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerForward : MonoBehaviour
{

    public delegate void EventToForward(Collider collider);
    public EventToForward eventForwardDelegate;

    public EventToForward eventExitForwardDelegate;

    public string targetTag = "Ground";

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == targetTag && eventForwardDelegate != null)
            eventForwardDelegate.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == targetTag && eventExitForwardDelegate != null)
            eventExitForwardDelegate.Invoke(other);
    }
}
