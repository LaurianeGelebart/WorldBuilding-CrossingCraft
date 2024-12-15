using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WFCTileRenderer : MonoBehaviour
{
    public float textDistance = 3;
    private WFCTile tile;
    private bool updated = false;

    private GameObject tileObject = null;

    public WFCTile Tile
    {
        get => tile;
        set
        {
            tile = value;
            updated = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Find("XPos").transform.localPosition = new Vector3(textDistance, 0, 0);
        transform.Find("XNeg").transform.localPosition = new Vector3(-textDistance, 0, 0);
        transform.Find("YPos").transform.localPosition = new Vector3(0, textDistance, 0);
        transform.Find("YNeg").transform.localPosition = new Vector3(0, -textDistance, 0);
        transform.Find("ZPos").transform.localPosition = new Vector3(0, 0, textDistance);
        transform.Find("ZNeg").transform.localPosition = new Vector3(0, 0, -textDistance);
        transform.Find("RotateFlip").transform.localPosition = new Vector3(0, textDistance * 2 / 3, -textDistance);
        if (updated)
        {
            updated = false;
            Destroy(tileObject);
            tileObject = tile.ToGameObject(transform);

            // if (tile.rotationY != 0 || tile.flipX != false)
            // {
            //     transform.Find("EmptyCube").GetComponentInChildren<Renderer>().material.color = Color.magenta;
            // }

            TextMeshPro xPos = transform.Find("XPos").GetComponent<TextMeshPro>();
            TextMeshPro xNeg = transform.Find("XNeg").GetComponent<TextMeshPro>();
            TextMeshPro yPos = transform.Find("YPos").GetComponent<TextMeshPro>();
            TextMeshPro yNeg = transform.Find("YNeg").GetComponent<TextMeshPro>();
            TextMeshPro zPos = transform.Find("ZPos").GetComponent<TextMeshPro>();
            TextMeshPro zNeg = transform.Find("ZNeg").GetComponent<TextMeshPro>();
            TextMeshPro rotateFlip = transform.Find("RotateFlip").GetComponent<TextMeshPro>();

            xPos.text = tile.sockets.xPos.ToString();
            xNeg.text = tile.sockets.xNeg.ToString();
            yPos.text = tile.sockets.yPos.ToString();
            yNeg.text = tile.sockets.yNeg.ToString();
            zPos.text = tile.sockets.zPos.ToString();
            zNeg.text = tile.sockets.zNeg.ToString();
            rotateFlip.text = $"{tile.rotationY} {tile.flipX}";
        }
    }
}
