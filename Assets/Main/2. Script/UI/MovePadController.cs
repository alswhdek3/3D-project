using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePadController : SingtonMonoBehaviour<MovePadController>
{
    [SerializeField]
    private Camera m_uiCamera;
    [SerializeField]
    private UISprite m_padBg, m_padBtn;
    private Vector3 m_dir;
    private const float MAX_DIST = 0.37f;
    private float m_maxDistPow;
    private bool m_isDrag;

    public bool IsDrag { get { return m_isDrag; } }
    private Vector3 GetTouchMove()
    {
        Ray ray = m_uiCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray,out hit,100f,1<<LayerMask.NameToLayer("UI")))
        {
            if(hit.collider.transform == m_padBg.transform)
            {
                return hit.point;
            }
        }
        return Vector3.zero;
    }
    public Vector2 GetPos()
    {
        return m_dir;
    }
    void Start()
    {
        m_uiCamera = GameObject.Find("UI Root").GetComponentInChildren<Camera>();
        m_maxDistPow = Mathf.Pow(MAX_DIST, 2f);
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var pos = GetTouchMove();
            if(pos != Vector3.zero)
            {
                m_isDrag = true;
                var distance = pos - m_padBg.transform.position;
                if(Mathf.Approximately(distance.sqrMagnitude,m_maxDistPow) || distance.sqrMagnitude < m_maxDistPow)
                {
                    m_padBtn.transform.position = m_padBg.transform.position + distance;
                }
                else
                {
                    m_padBtn.transform.position = m_padBg.transform.position + distance.normalized * MAX_DIST;
                }
                m_dir = pos;
            }
            else
            {
                m_dir = Vector3.zero;
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            m_isDrag = false;
            m_padBtn.transform.localPosition = m_padBg.transform.localPosition;
            m_dir = Vector3.zero;
        }
        if(m_isDrag)
        {
            var dir = m_uiCamera.ScreenToWorldPoint(Input.mousePosition) - m_padBg.transform.position;
            if(Mathf.Approximately(dir.sqrMagnitude,m_maxDistPow) || dir.sqrMagnitude < m_maxDistPow)
            {
                m_padBtn.transform.position = m_padBg.transform.position + dir;
            }
            else
            {
                m_padBtn.transform.position = m_padBg.transform.position + dir.normalized * MAX_DIST;
            }
            m_dir = dir;
        }
    }
}
