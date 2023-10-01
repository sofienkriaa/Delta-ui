using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using System.IO.Ports;

public class translater : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;

    private Vector3 camCurr;
    private Vector3 offsetCurr;
    
    private Vector3 maxPos = new Vector3(0f, 8.15f, 0f);

    // Translater 1 Vectors
    private static Vector3 direction1 = new Vector3(-0.50042f, 0.81568f, 0.290243f);
    private static Vector3 startPos1 = new Vector3(5f, 0f, -2.9f);
    private Vector3 addedOffsetPos1 = new Vector3(-1.7f, 3f, 1f);
    
    // Translater 2 Vectors
    private static Vector3 direction2 = new Vector3(0f, -0.8171f, 0.57648f);
    private static Vector3 startPos2 = new Vector3(0f, 0f, 5.75f);
    private static Vector3 addedOffsetPos2 = new Vector3(0f, 3.05f, -2.15f);

    // Translater 3 Vectors
    private static Vector3 direction3 = new Vector3(-0.5004f, -0.8156f, -0.2902f);
    private static Vector3 startPos3 = new Vector3(-5f, 0f, -2.9f);
    private static Vector3 addedOffsetPos3 = new Vector3(1.8f, 3.1f, 1.1f);

    // Generic Translater Vectors
    private Vector3 direction = new Vector3(0f, 0f, 0f);
    private Vector3 startPos = new Vector3(0f, 0f, 0f);
    private Vector3 addedOffsetPos = new Vector3(0f, 0f, 0f);

    // Translaters tables
    private Vector3[] directions = { direction1, direction2, direction3, };
    private Vector3[] startPositions = { startPos1, startPos2, startPos3 };
    private Vector3[] addedOffsets = { Vector3.zero, addedOffsetPos2 * 1.1f, addedOffsetPos3 };
    private string[] translaterNames = { "translater-1", "translater-2", "translater-3" };

    static public SerialPort arduino = new SerialPort("COM7", 9600);

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
        if (gameObject.name == "actor")
        {
            // (x0, y0, z0)
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            camCurr = Camera.main.ScreenToWorldPoint(curScreenPoint);
            transform.position = camCurr;

            // length of the bicep
            double l = 14;

            // x0^2 + y0^2 + z0^2 - l^2
            double c11 = Math.Pow(Vector3.Distance(camCurr, Vector3.zero), 2) - l*l;

            for (int i = 0; i < 3; i++)
            {
                // Xv1^2 + Yv1^2 + Zv1^2
                double c15 = Math.Pow(Vector3.Distance(directions[i], Vector3.zero), 2);

                // 2.a11.xv1 + 2.a12.yv1 + 2.a13.zv1 - xv1.c12 - yv1.c13 - zv1.c14
                double c16 = 2 * Vector3.Dot(startPos1, directions[i]) - 2 * Vector3.Dot(directions[i], camCurr);

                // a11^2 + a12^2 + a13^2 - a11.c12 - a12.c13 - a13.c14 + c11
                double c17 = Math.Pow(Vector3.Distance(startPositions[i], Vector3.zero), 2) - 2 * Vector3.Dot(startPositions[i], camCurr) + c11;

                // solution alpha (2 Solutions of 2nd degree polynomial)
                double alpha1 = (-c16 + Math.Sqrt(Math.Pow(c16, 2) - 4 * c15 * c17)) / (2 * c15);
                Vector3 positionAlpha1 = (float)alpha1 * directions[i] + startPositions[i] - addedOffsets[i];

                double alpha2 = (-c16 - Math.Sqrt(Math.Pow(c16, 2) - 4 * c15 * c17)) / (2 * c15);
                Vector3 positionAlpha2 = (float)alpha2 * directions[i] + startPositions[i] - addedOffsets[i];

                // Used Vector Buffer
                Vector3 usedVector = startPositions[i];

                // If the 1st Alpha solution is in the accepted Range
                if ((positionAlpha1.y > 0) && (positionAlpha1.y < maxPos.y))
                {
                    usedVector = positionAlpha1;
                }
                // If the 2nd Alpha solution is in the accepted Range
                else if ((positionAlpha2.y > 0) && (positionAlpha2.y < maxPos.y))
                {
                    usedVector = positionAlpha2;
                }
                // If both solutions are out of bound in the negative direction
                else if ((positionAlpha1.y < 0) && (positionAlpha2.y < 0))
                {
                    usedVector = startPositions[i];
                }
                // If both solutions are out of bound in the positive direction
                else if ((positionAlpha1.y > maxPos.y) && (positionAlpha2.y > maxPos.y))
                {
                    usedVector = maxPos;
                }
                // The following 4 case, if both solutions are out of bound in opposite direction
                // --> Choose the solution that's closer to a corner point (Max or start points)
                else if (Math.Abs(positionAlpha1.y) < Math.Abs(positionAlpha2.y - maxPos.y))
                {
                    usedVector = startPositions[i];
                }
                else if (Math.Abs(positionAlpha2.y) < Math.Abs(positionAlpha1.y - maxPos.y))
                {
                    usedVector = startPositions[i];
                }
                else if (Math.Abs(positionAlpha1.y) > Math.Abs(positionAlpha2.y - maxPos.y))
                {
                    usedVector = maxPos;
                }
                else if (Math.Abs(positionAlpha2.y) > Math.Abs(positionAlpha1.y - maxPos.y))
                {
                    usedVector = maxPos;
                }

                // Use chosen position vector
                GameObject.Find(translaterNames[i]).transform.position = usedVector;

                arduino.Open();
                if (arduino.IsOpen)
                {
                    arduino.Write(i + "");
                    Debug.Log(i);
                }
                else
                {
                    Debug.Log("Not open");
                }
                arduino.Close();
            }
        }
        else
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
}
