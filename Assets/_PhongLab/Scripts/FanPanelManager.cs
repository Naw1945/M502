using UnityEngine;

public class FanPanelManager : MonoBehaviour
{
    [Header("Danh sách 4 quạt")]
    public CeilingFanController[] fans = new CeilingFanController[4];

    [Header("Danh sách 4 công tắc con")]
    public FanSwitchSlot[] switchSlots = new FanSwitchSlot[4];

    void Start()
    {
        if (switchSlots == null || switchSlots.Length == 0 || switchSlots[0] == null)
        {
            switchSlots = GetComponentsInChildren<FanSwitchSlot>();
        }

        for (int i = 0; i < switchSlots.Length; i++)
        {
            if (i < fans.Length && fans[i] != null && switchSlots[i] != null)
            {
                switchSlots[i].BindFan(fans[i]);
            }
        }
    }
}