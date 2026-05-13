using UnityEngine;

public class TrashCan : MonoBehaviour
{
    public bool AddTrash(TrashItem trash)
    {
        trash.Dispose();
        Debug.Log("[쓰레기통] 쓰레기 버림 완료!");
        return true;
    }
}
