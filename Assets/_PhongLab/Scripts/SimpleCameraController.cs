using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCameraController : MonoBehaviour
{
    public float tocDoDiChuyen = 3.5f;
    public float doNhayChuot = 0.15f;

    private float xoayX = 0f;
    private float xoayY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Vector3 rot = transform.localEulerAngles;
        xoayY = rot.y;
        xoayX = rot.x;
    }

    void Update()
    {
        // 1. Xoay bằng chuột (New Input System)
        if (Mouse.current != null)
        {
            Vector2 deltaChuot = Mouse.current.delta.ReadValue() * doNhayChuot;
            xoayY += deltaChuot.x;
            xoayX -= deltaChuot.y;
            xoayX = Mathf.Clamp(xoayX, -80f, 80f);
            transform.localRotation = Quaternion.Euler(xoayX, xoayY, 0f);
        }

        // 2. Di chuyển bằng phím W A S D
        if (Keyboard.current != null)
        {
            float ngang = 0f;
            float doc = 0f;

            if (Keyboard.current.wKey.isPressed) doc += 1f;
            if (Keyboard.current.sKey.isPressed) doc -= 1f;
            if (Keyboard.current.dKey.isPressed) ngang += 1f;
            if (Keyboard.current.aKey.isPressed) ngang -= 1f;

            Vector3 huongDi = transform.forward * doc + transform.right * ngang;
            huongDi.y = 0;
            huongDi.Normalize();

            transform.position += huongDi * tocDoDiChuyen * Time.deltaTime;

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}