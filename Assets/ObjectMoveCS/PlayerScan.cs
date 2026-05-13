using UnityEngine;

public class PlayerScan : MonoBehaviour
{
    public float scanRange = 3f;      // 스캔 거리
    public POSManager posManager;     // 포스기 연결

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 좌클릭
        {
            Scan();
        }
    }

    void Scan()
    {
        Ray ray = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width / 2, Screen.height / 2)
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, scanRange))
        {
            ScanableObject item = hit.collider.GetComponent<ScanableObject>();

            // 스캔 가능한 물건인지 확인
            if (item != null && !item.isScanned)
            {
                item.isScanned = true;

                posManager.AddPrice(item.price);

                Debug.Log("스캔 성공: " + item.price);
            }
        }
    }
}
