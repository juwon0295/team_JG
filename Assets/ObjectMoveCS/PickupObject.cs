using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public bool isHeld = false;

    [Header("이 물건의 지정 위치")]
    public Transform placeTarget; // 각 물건마다 따로 지정!
}
