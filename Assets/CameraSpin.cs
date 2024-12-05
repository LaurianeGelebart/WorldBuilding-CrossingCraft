using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float speed = 0.125f;
        float angle = Time.time;
        float angleOmega = angle * Mathf.PI;
        transform.position = new Vector3(
            Mathf.Sin(angleOmega * speed) * 30,
            20,
            Mathf.Cos(angleOmega * speed) * 30
        );
        transform.LookAt(Vector3.zero);
    }
}
