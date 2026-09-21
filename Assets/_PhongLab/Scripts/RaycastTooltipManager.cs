using UnityEngine;
using TMPro;

public class RaycastTooltipManager : MonoBehaviour
{
    [Header("UI Tooltip (Con của Main Camera)")]
    public GameObject tooltipUI;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtStatus;
    
    [Header("Cài đặt tia Ray")]
    public float maxDistance = 10f;
    public LayerMask hitLayers = ~0;

    void Update()
    {
        if (tooltipUI == null) return;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistance, hitLayers))
        {
            InteractableInfo info = hit.collider.GetComponentInParent<InteractableInfo>();

            if (info != null)
            {
                txtName.text = $"Tên vật thể: {info.objectName}";
                txtStatus.text = info.GetStatusText();
                tooltipUI.SetActive(true);
                return;
            }
        }

        if (tooltipUI.activeSelf)
        {
            tooltipUI.SetActive(false);
        }
    }
}