using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpin : MonoBehaviour
{
    public TerrainController twfc;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 center = new(0, 0, 0);
        Vector3 min = new(0, 0, 0);
        Vector3 max = new(0, 0, 0);
        if (twfc.transform.childCount > 0)
        {
            Vector3 sumVector = new(0f, 0f, 0f);
            foreach (Transform child in twfc.transform)
            {
                sumVector += child.position;
                min = Vector3.Min(min, child.position);
                max = Vector3.Max(max, child.position);
            }
            center = sumVector / twfc.transform.childCount;
        }

        float speed = 0.125f;
        float angle = Time.time;
        float angleOmega = angle * Mathf.PI;
        float radius = Vector3.Distance(min, max);
        transform.position = center + new Vector3(
            Mathf.Sin(angleOmega * speed) * radius,
            radius * 2 / 3,
            Mathf.Cos(angleOmega * speed) * radius
        );
        transform.LookAt(center);
    }
}
