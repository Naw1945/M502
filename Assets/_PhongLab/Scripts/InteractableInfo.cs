using UnityEngine;

public enum InteractionType
{
    None,       // Chưa có tương tác (như tường, sàn)
    Grabbable,  // Cầm nắm được
    Usable      // Bấm nút, bật tắt, mở cửa...
}

public class InteractableInfo : MonoBehaviour
{
    [Header("Thông tin hiển thị")]
    public string objectName = "Tường";
    public InteractionType interactionType = InteractionType.None;
    
    [TextArea]
    public string customDescription = "";

    public string GetStatusText()
    {
        if (!string.IsNullOrEmpty(customDescription))
            return customDescription;

        return interactionType switch
        {
            InteractionType.None => "Hiện chưa có tương tác với vật thể này",
            InteractionType.Grabbable => "Có thể cầm nắm (Bấm Grip)",
            InteractionType.Usable => "Có thể tương tác (Bấm Trigger)",
            _ => ""
        };
    }
}