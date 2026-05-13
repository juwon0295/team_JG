using UnityEngine;

public class TrashItem : MonoBehaviour
{
    public string trashName = "¾²·¹±â";
    public bool isPickedUp = false;

    private Rigidbody rb;
    private Collider col;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void PickUp(Transform holdPoint)
    {
        if (isPickedUp) return;

        isPickedUp = true;
        rb.isKinematic = true;
        col.enabled = false;

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }
}

