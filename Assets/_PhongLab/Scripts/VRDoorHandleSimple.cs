using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Cho XRI 3.x

public class VRDoorHandleSimple : MonoBehaviour
{
    [Header("Bản lề cánh cửa")]
    [Tooltip("Kéo đối tượng BanLe_Cua vào đây")]
    public Transform doorHinge;

    [Header("Cài đặt góc xoay tay nắm")]
    [Tooltip("Góc tay nắm gạt xuống (trục Z)")]
    public float handleOpenAngleZ = 90f;
    public float handleSpeed = 7f;

    [Header("Cài đặt góc mở cánh cửa")]
    [Tooltip("Góc cánh cửa mở ra ngoài quanh trục Y (-90 hoặc 90 tùy hướng)")]
    public float doorOpenAngleY = -90f;
    [Tooltip("Tốc độ mở/đóng cửa")]
    public float doorSpeed = 1.2f;

    private bool isOpen = false;      // false = đang đóng, true = đang mở
    private bool isMoving = false;    // Cờ khóa, tránh bị bấm liên tục lúc cửa đang chạy
    private XRSimpleInteractable simpleInteractable;

    private Quaternion initialHandleRot;
    private Quaternion closedDoorRot;

    void Awake()
    {
        InitInteractable();
    }

    void Start()
    {
        // Ghi nhớ góc ban đầu (lúc đóng hoàn toàn)
        initialHandleRot = transform.localRotation;

        if (doorHinge != null)
        {
            closedDoorRot = doorHinge.localRotation;
        }
    }

    void OnEnable()
    {
        InitInteractable();
        if (simpleInteractable != null)
        {
            simpleInteractable.selectEntered.AddListener(OnGrab);
        }
    }

    void OnDisable()
    {
        if (simpleInteractable != null)
        {
            simpleInteractable.selectEntered.RemoveListener(OnGrab);
        }
    }

    private void InitInteractable()
    {
        if (simpleInteractable == null)
        {
            simpleInteractable = GetComponent<XRSimpleInteractable>();
            if (simpleInteractable == null)
            {
                simpleInteractable = gameObject.AddComponent<XRSimpleInteractable>();
            }
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Nếu cửa đang trong quá trình chuyển động thì không nhận tương tác tiếp
        if (isMoving) return;

        StartCoroutine(ToggleDoorRoutine());
    }

    private IEnumerator ToggleDoorRoutine()
    {
        isMoving = true;

        // 1. Gạt tay nắm chúc xuống (cộng thêm góc theo trục Z)
        Quaternion targetHandleRot = initialHandleRot * Quaternion.Euler(0, 0, handleOpenAngleZ);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * handleSpeed;
            transform.localRotation = Quaternion.Slerp(initialHandleRot, targetHandleRot, t);
            yield return null;
        }
        transform.localRotation = targetHandleRot;

        // Khựng lại 0.15s mô phỏng nhả chốt
        yield return new WaitForSeconds(0.15f);

        // 2. Xác định góc đích của cửa (nếu đang đóng thì mở ra, nếu đang mở thì về góc đóng ban đầu)
        if (doorHinge != null)
        {
            Quaternion targetDoorRot;
            if (!isOpen)
            {
                // Mở ra ngoài
                targetDoorRot = closedDoorRot * Quaternion.Euler(0, doorOpenAngleY, 0);
            }
            else
            {
                // Khép về vị trí ban đầu
                targetDoorRot = closedDoorRot;
            }

            while (Quaternion.Angle(doorHinge.localRotation, targetDoorRot) > 0.5f)
            {
                doorHinge.localRotation = Quaternion.Slerp(doorHinge.localRotation, targetDoorRot, Time.deltaTime * doorSpeed);
                yield return null;
            }
            doorHinge.localRotation = targetDoorRot;
        }

        // 3. Tay nắm bật trở lại vị trí thẳng ban đầu
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * handleSpeed;
            transform.localRotation = Quaternion.Slerp(targetHandleRot, initialHandleRot, t);
            yield return null;
        }
        transform.localRotation = initialHandleRot;

        // Cập nhật lại trạng thái đóng/mở và mở khóa chuyển động
        isOpen = !isOpen;
        isMoving = false;
    }
}