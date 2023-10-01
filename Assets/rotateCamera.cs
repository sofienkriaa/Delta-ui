using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class rotateCamera : MonoBehaviour
{
    public float sensitivity = 1;
    public float speed = 5.0f;

    // For my setup, Arduino is connected on the COM7 Port.
    // In the future, the user shall be able to choose the right port.
    static public SerialPort arduino = new SerialPort("COM7", 9600);

    void Start()
    {
        // Arduino is initialized here, Because this code is only used
        // for the camera, not multiple components like the translate script
        arduino.Open();
    }

    void Update()
    {
        // Control Camera rotation after pressing space
        if (Input.GetKey(KeyCode.Space))
        {
            float rotateHorizontal = Input.GetAxis("Mouse X");
            float rotateVertical = Input.GetAxis("Mouse Y");
            transform.Rotate(-transform.up * rotateHorizontal * sensitivity);
            transform.Rotate(transform.right * rotateVertical * sensitivity);
        }

        //Control Translation with arrows
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        }

        // Zoom in/Zoom out with + and - Keys
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
        }

    }
}
