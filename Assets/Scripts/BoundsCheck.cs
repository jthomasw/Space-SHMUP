using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

public class BoundsCheck : MonoBehaviour
{
    [System.Flags]
    public enum eScreenlocs {
    onScreen = 0,
    offRight = 1,
    offLeft = 2,
    offUp = 3,
    offDown = 4,
    }
    public enum eType { center, inset, outset };

    [Header("Inscribed")]
    public eType boundsType = eType.center;
    public float radius = 1f;
    public bool keepOnScreen = true;

    [Header("Dynamic")]
    public eScreenlocs screenLocs = eScreenlocs.onScreen;
    
    public float camWidth;
    public float camHeight;

    void Awake()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    void LateUpdate()
    {
        float checkRadius = 0;
        if (boundsType == eType.inset) checkRadius = -radius;
        if (boundsType == eType.outset) checkRadius = radius;

        Vector3 pos = transform.position;
        screenLocs = eScreenlocs.onScreen;

        if (pos.x > camWidth + checkRadius)
        {
            pos.x = camWidth + checkRadius;
            screenLocs |= eScreenlocs.offRight;
        }
        if (pos.x < -camWidth - checkRadius)
        {
            pos.x = -camWidth - checkRadius;
            screenLocs |= eScreenlocs.offLeft;
        }

        if (pos.y > camHeight + checkRadius)
        {
            pos.y = camHeight + checkRadius;
            screenLocs |= eScreenlocs.offUp;
        }
        if (pos.y < -camHeight - checkRadius)
        {
            pos.y = -camHeight - checkRadius;
            screenLocs |= eScreenlocs.offDown;
        }

        if (keepOnScreen && !isOnScreen)
        {
            transform.position = pos;
            screenLocs |= eScreenlocs.onScreen;
        }
    }
        public bool isOnScreen{
        get{ return (screenLocs == eScreenlocs.onScreen);}
        }

    public bool LocIs(eScreenlocs checkLoc)
    {
        if (checkLoc == eScreenlocs.onScreen) return isOnScreen;
        return ((screenLocs & checkLoc) == checkLoc);
    }
}
