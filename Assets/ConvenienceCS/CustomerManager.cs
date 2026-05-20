using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance; // 싱글톤

    [Header("설정")]
    public int requiredCustomers = 2;   // Phase 2로 넘어가는 데 필요한 손님 수

    private int servedCount = 0;        // 현재까지 결제 완료된 손님 수

    private void Awake()
    {
        // 씬 어디서든 CustomerManager.Instance로 접근 가능하게 설정
        Instance = this;
    }

    // ── 결제 완료 시 POSUIManager에서 호출 ───────
    public void OnCustomerServed()
    {
        servedCount++;
        Debug.Log($"손님 {servedCount}명 처리 완료");

        // 목표 손님 수 달성 시 Phase 2 시작
        if (servedCount >= requiredCustomers)
        {
            GameManager.Instance.OnPhase2Start();
        }
    }
}
