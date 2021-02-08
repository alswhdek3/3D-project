using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointController : MonoBehaviour
{
    private WayPoint[] m_wayPoint;
    public WayPoint[] WayPoint { get { return m_wayPoint; } }
    private void Start()
    {
        m_wayPoint = gameObject.GetComponentsInChildren<WayPoint>();
    }
}
