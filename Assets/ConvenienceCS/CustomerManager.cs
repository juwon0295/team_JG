using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance;

    [Header("설정")]
    public int requiredCustomers = 2;   // Phase 2로 넘어가는 데 필요한 손님 수

    private int servedCount = 0;        // 현재까지 결제 완료된 손님 수

    private void Awake()
    {
        Instance = this;
    }

    // ── 결제 완료 시 POSUIManager에서 호출 ───────
    public void OnCustomerServed()
    {
        servedCount++;
        Debug.Log($"손님 {servedCount}명 처리 완료");

        // 손님 카운트 텍스트 갱신
        StoreMissionManager.Instance.UpdateCustomerMission(servedCount, requiredCustomers);

        // 목표 손님 수 달성 시 Phase 2 시작
        if (servedCount >= requiredCustomers)
        {
            GameManager.Instance.OnPhase2Start();
        }
    }
}
