using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float dragSpeed = 1;
    public float panSpeed;

    private Vector3 dragOrigin;
    private Vector3 oldPos;
    private bool panning;
    private Vector3 panDestination;
    private Vector3 startPosition;
    private float progress = 0;
    private float distance = 0;

    void Update()
    {
        if (!panning && Input.GetMouseButtonDown(1))
        {
            oldPos = transform.position;
            float temp = oldPos.y;
            oldPos.y = oldPos.z;
            oldPos.z = temp;
            dragOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }

        if (!panning & Input.GetMouseButton(1))
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition) - dragOrigin;
            pos.x *= dragSpeed * 2;
            pos.y *= dragSpeed;
            Vector3 a = oldPos + -pos;
            transform.position = new Vector3(Mathf.Clamp(a.x, 0, 14), 10, Mathf.Clamp(a.y, -5, 13));
        }

        if(panning)
        {
            transform.position = Vector3.Lerp(startPosition, panDestination, progress += Time.deltaTime * panSpeed / distance);
            panning = progress < 1;
        }
    }

    public void PanToDestination(Vector3 destination)
    {
        panning = true;
        startPosition = transform.position;
        panDestination = destination;
        progress = 0;
        distance = Vector3.Distance(startPosition, destination);
        if (distance <= 0)
            distance = 0.1f;
    }
}
