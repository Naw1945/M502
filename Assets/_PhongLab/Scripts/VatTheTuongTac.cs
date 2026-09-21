using UnityEngine;
using UnityEngine.Events;

public class VatTheTuongTac : MonoBehaviour
{
    [Header("Thông tin hiển thị lên HUD")]
    public string tenVatThe = "Tên đồ vật";
    public string huongDan = "[E] / [Trigger] Tương tác";

    [Header("Hành vi khi tương tác")]
    public UnityEvent onInteract;

    public virtual void TuongTac()
    {
        // Kích hoạt toàn bộ sự kiện đã cấu hình trong Inspector
        onInteract?.Invoke();
    }
}