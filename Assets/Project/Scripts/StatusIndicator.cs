using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour
{
    [SerializeField]
    RectTransform healthBarRect;

    // Start is called before the first frame update
    void Start()
    {
        if(healthBarRect == null)
        {
            Debug.LogError("STATUS INDICATOR: no health bar obect reference!");
        }
    }

    public void SetHealth(int _cur, int _max) //the _ is to show that the variable is only valid inside the given method
    {
        float _value = (float) _cur / _max;

        healthBarRect.localScale = new Vector3(_value, healthBarRect.localScale.y, healthBarRect.localScale.z);
    }
}
