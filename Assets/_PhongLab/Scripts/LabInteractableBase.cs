using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Collider))]
public abstract class LabInteractableBase : XRSimpleInteractable
{
    protected override void OnEnable()
    {
        base.OnEnable();
        // Lắng nghe cả Trigger (Activate/Chuột trái) lẫn Grip (Select/Phím G)
        activated.AddListener(OnActivatedByVR);
        selectEntered.AddListener(OnSelectedByVR);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        activated.RemoveListener(OnActivatedByVR);
        selectEntered.RemoveListener(OnSelectedByVR);
    }

    private void OnActivatedByVR(ActivateEventArgs args) => ExecuteInteraction();
    private void OnSelectedByVR(SelectEnterEventArgs args) => ExecuteInteraction();

    // Hàm hành động chung để các vật thể con tự định nghĩa
    public abstract void ExecuteInteraction();
}