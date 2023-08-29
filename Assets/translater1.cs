using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class translater : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;

    private Vector3 camCurr;
    private Vector3 offsetCurr;
    
    private Vector3 maxPos = new Vector3(0f, 8.15f, 0f);

    // Translater 1 Vectors
    private Vector3 direction1 = new Vector3(-0.50042f, 0.81568f, 0.290243f);
    private Vector3 startPos1 = new Vector3(5f, 0f, -2.9f);
    private Vector3 addedOffsetPos1 = new Vector3(-1.7f, 3f, 1f);
    
    // Translater 2 Vectors
    private Vector3 direction2 = new Vector3(0f, -0.8171f, 0.57648f);
    private Vector3 startPos2 = new Vector3(0f, 0f, 5.75f);
    private Vector3 addedOffsetPos2 = new Vector3(0f, 3.05f, -2.15f);

    // Translater 3 Vectors
    private Vector3 direction3 = new Vector3(-0.5004f, -0.8156f, -0.2902f);
    private Vector3 startPos3 = new Vector3(-5f, 0f, -2.9f);
    private Vector3 addedOffsetPos3 = new Vector3(1.8f, 3.1f, 1.1f);

    // Generic Translater Vectors        0 - 0, 0.25 - 3,3, 5,65-3,5
    private Vector3 direction = new Vector3(0f, 0f, 0f);
    private Vector3 startPos = new Vector3(0f, 0f, 0f);
    private Vector3 addedOffsetPos = new Vector3(0f, 0f, 0f);

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

        // Identify selected translater
        if (gameObject.name == "translater-1")
        {
            direction = direction1;
            startPos = startPos1;
            addedOffsetPos = addedOffsetPos1;
        }
        else if (gameObject.name == "translater-2")
        {
            direction = direction2;
            startPos = startPos2;
            addedOffsetPos = addedOffsetPos2;
        }
        else if (gameObject.name == "translater-3")
        {
            direction = direction3;
            startPos = startPos3;
            addedOffsetPos = addedOffsetPos3;
        }

        offset = Vector3.Dot(offset, direction) / Vector3.Dot(direction, direction) * direction;
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        camCurr = Camera.main.ScreenToWorldPoint(curScreenPoint);
        offsetCurr = startPos + Vector3.Dot(camCurr, direction) / Vector3.Dot(direction, direction) * direction;

        Vector3 curPosition = offsetCurr + addedOffsetPos;
        float posToStart = Vector3.Distance(curPosition, startPos);
        float edgeDistance = Vector3.Distance(maxPos, startPos);

        // Check if the Translater is moving inside the limits
        if ((posToStart + Vector3.Distance(curPosition, maxPos)) < edgeDistance + 0.2)
        {
            transform.position = curPosition;
        }
        else
        {
            // If the mouse moves out of the upper limit, block the translater at the max position
            if (posToStart > edgeDistance)
            {
                transform.position = maxPos;
            }
            else
            {
                // If the mouse moves out of the lower limit, block the translater at the min position
                transform.position = startPos;
            }
        }
    }
}
