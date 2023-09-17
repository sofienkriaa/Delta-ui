using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateCamera : MonoBehaviour
{
    public float sensitivity = 1;
    public float speed = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /*void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Debug.Log("Pressed left-click.");

        if (Input.GetMouseButtonDown(1))
        {

            Debug.Log("Pressed right-click.");
            float rotateHorizontal = Input.GetAxis("Mouse X");
            float rotateVertical = Input.GetAxis("Mouse Y");
            transform.Rotate(-transform.up * rotateHorizontal * sensitivity);
            transform.Rotate(transform.right * rotateVertical * sensitivity);

        }

        if (Input.GetMouseButtonDown(2))
            Debug.Log("Pressed middle-click.");
    }*/
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
