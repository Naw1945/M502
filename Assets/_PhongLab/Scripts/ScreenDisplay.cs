using System.Collections;
using UnityEngine;

public class ScreenDisplay : MonoBehaviour
{
    [Header("Mesh Renderer Màn Hình")]
    [SerializeField] private MeshRenderer screenRenderer;

    [Header("3 Material Trạng Thái")]
    [SerializeField] private Material matTat;        // Mat_ManHinhTat
    [SerializeField] private Material matKhoiDong;   // Mat_ManHinhKhoiDong
    [SerializeField] private Material matBat;        // Mat_ManHinhBat

    [Header("Thời Gian Khởi Động (Giây)")]
    [SerializeField] private float bootDuration = 2.5f;

    public bool IsOn { get; private set; } = false;
    private bool isBooting = false;
    private Coroutine bootRoutine;

    void Start()
    {
        if (screenRenderer == null)
            screenRenderer = GetComponent<MeshRenderer>();

        // Mặc định lúc bắt đầu game là màn hình tắt
        screenRenderer.material = matTat;
    }

    // Hàm gọi từ ngoài (như Case máy tính) để bật/tắt
    public void TogglePower()
    {
        if (isBooting) return; // Đang chạy khởi động thì không bấm tắt đột ngột

        if (!IsOn)
        {
            bootRoutine = StartCoroutine(BootSequence());
        }
        else
        {
            TurnOff();
        }
    }

    private IEnumerator BootSequence()
    {
        isBooting = true;

        // 1. Chuyển sang màn hình khởi động
        screenRenderer.material = matKhoiDong;
        yield return new WaitForSeconds(bootDuration);

        // 2. Chuyển sang màn hình chính (Desktop)
        screenRenderer.material = matBat;

        IsOn = true;
        isBooting = false;
    }

    public void TurnOff()
    {
        if (bootRoutine != null)
        {
            StopCoroutine(bootRoutine);
        }

        screenRenderer.material = matTat;
        IsOn = false;
        isBooting = false;
    }
}