using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{

    public float m_keyMoveSpeed;
    public float m_mouseMoveSpeed;
    public float m_rotationSpeed;
    public float m_zoomSpeed;

    private Vector2 m_holdingDownFactor;
    private bool m_holdingDownForRotation;
    private bool m_holdingDownForMovement;
    private KeyCode m_mouseRotateKey = KeyCode.Mouse2;
    private KeyCode m_mouseMoveKey = KeyCode.Mouse1;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyInput();
        HandleMouseRotation();
        HandleMouseScroll();
        // HandleMouseUpDownMovement();

    }

    private void HandleKeyInput()
    {
        Vector3 movement = new Vector3(0, 0, 0);
        // Forward and back
        if (Input.GetKey(KeyCode.W))
        {
            movement += new Vector3(transform.forward.x, 0, transform.forward.z) * m_keyMoveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement += new Vector3(transform.forward.x, 0, transform.forward.z) * m_keyMoveSpeed * Time.deltaTime * -1;
        }
        // Right and left
        if (Input.GetKey(KeyCode.A))
        {
            movement += transform.right * m_keyMoveSpeed * Time.deltaTime * -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += transform.right * m_keyMoveSpeed * Time.deltaTime;
        }
        transform.position += movement;
    }
    private void HandleMouseRotation()
    {
        // Rotation around own axis
        if (Input.GetKeyDown(m_mouseRotateKey))
        {
            m_holdingDownFactor = GetMousePosFactor();
            m_holdingDownForRotation = true;
            m_holdingDownForMovement = false;
        }
        else if (Input.GetKeyUp(m_mouseRotateKey))
        {
            m_holdingDownForRotation = false;
        }

        // Movement along world plane
        if (Input.GetKeyDown(m_mouseMoveKey))
        {
            m_holdingDownFactor = GetMousePosFactor();
            m_holdingDownForMovement = true;
            m_holdingDownForRotation = false;
        }
        else if (Input.GetKeyUp(m_mouseMoveKey))
        {
            m_holdingDownForMovement = false;
        }

        Vector2 currentMousePosFactor = GetMousePosFactor();
        // The further the mouse is from its origin, the faster we rotate
        Vector2 mousePosDiff = currentMousePosFactor - m_holdingDownFactor;

        if (m_holdingDownForMovement)
        {
            Vector3 movement = new Vector3();//new Vector3(mousePosDiff.x, 0, mousePosDiff.y) * m_mouseMoveSpeed * Time.deltaTime;
            movement += transform.right.normalized * mousePosDiff.x * m_mouseMoveSpeed * Time.deltaTime;
            movement += new Vector3(transform.forward.x, 0, transform.forward.z) * mousePosDiff.y * m_mouseMoveSpeed * Time.deltaTime * -1;
            transform.position += movement;

        }
        else if (m_holdingDownForRotation)
        {
            Vector2 rotationAngles = m_rotationSpeed * Time.deltaTime * mousePosDiff;

            transform.Rotate(0, rotationAngles.x, 0, Space.World);
            transform.Rotate(rotationAngles.y, 0, 0, Space.Self);
        }

    }

    // Returns where the mouse x position is in screenspace, from -1 to 1
    private Vector2 GetMousePosFactor()
    {
        float xFactor = (Input.mousePosition.x - Screen.width / 2) / Screen.width;
        float yFactor = (Input.mousePosition.y - Screen.height / 2) / Screen.height * -1;
        return new Vector2(xFactor, yFactor);
    }

    // Zooms with the scroll wheel (should be smoothened out in the future)
    private void HandleMouseScroll()
    {
        Vector3 movement = new Vector3();
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            movement += Vector3.up * m_zoomSpeed * Time.deltaTime;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            movement += Vector3.up * m_zoomSpeed * Time.deltaTime * -1;
        }
        transform.position += movement;
    }

}
