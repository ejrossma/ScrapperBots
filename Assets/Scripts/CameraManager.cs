using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float dragSpeed = 1;
    private Vector3 dragOrigin;
    private Vector3 oldPos;
    private bool dragging;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            oldPos = transform.position;
            dragOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition) - dragOrigin;
            transform.position = oldPos + -pos * dragSpeed;
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, 4, 12), Mathf.Clamp(transform.position.y, 6, 31), 0);
        }
    }
}
