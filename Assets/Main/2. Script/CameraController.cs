using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform m_target;
    [Range(0f, 10f)]
    [SerializeField]
    private float m_distance;
    [Range(0f, 20f)]
    [SerializeField]
    private float m_height;
    [Range(0f, 180f)]
    [SerializeField]
    private float m_angle;
    [Range(0f, 10f)]
    [SerializeField]
    private float m_speed;

    private Vector3 m_prevPos;
    private float m_prevAngle;

    // Start is called before the first frame update
    void Start()
    {
        m_prevPos = transform.position = m_target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(Mathf.Lerp(m_prevAngle, m_angle, m_speed * Time.deltaTime), 0f, 0f);
        transform.position = new Vector3(
            Mathf.Lerp(m_prevPos.x, m_target.transform.position.x, m_speed * Time.deltaTime),
            Mathf.Lerp(m_prevPos.y, m_target.transform.position.y + m_height, m_speed * Time.deltaTime),
            Mathf.Lerp(m_prevPos.z, m_target.transform.position.z - m_distance, m_speed * Time.deltaTime)
            );
    }
    private void LateUpdate()
    {
        m_prevAngle = transform.eulerAngles.x;
        m_prevPos = transform.position;
    }
}
