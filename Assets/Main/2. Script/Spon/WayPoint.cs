using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorType
{
    None=-1,
    Red,
    Yellow,
    Green,
    Blue,
    Max
}
public class WayPoint : MonoBehaviour
{
    [SerializeField]
    private ColorType m_colorType;
    private Color m_color;
    public Color GetColorType()
    {
        switch(m_colorType)
        {
            case ColorType.Red:
                m_color = Color.red;
                break;
            case ColorType.Yellow:
                m_color = Color.yellow;
                break;
            case ColorType.Green:
                m_color = Color.green;
                break;
            case ColorType.Blue:
                m_color = Color.blue;
                break;
        }
        return m_color;
    }
    public void OnDrawGizmos()
    {
        m_color = GetColorType();
        Gizmos.color = m_color;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }

}
