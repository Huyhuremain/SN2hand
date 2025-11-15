using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Cần gán Prefab BasicEnemy (hoặc loại Enemy cơ bản khác) trong Inspector
    [Header("Respawn Configuration")]
    [SerializeField] private Enemy enemyPrefab; 
    [SerializeField] private Vector3 spawnPosition = new Vector3(5f, 5f, 0f); 

    [SerializeField] private float respawnTime = 120f; // 2 phút = 120 giây
    private const string LastKillTimeKey = "LastEnemyKillTime"; 
    
    // Biến lưu trữ cá thể Enemy hiện tại (để theo dõi xem nó đã bị giết chưa)
    private Enemy currentEnemyInstance;

    void Start()
    {
        // Khi game bắt đầu, kiểm tra xem đã đến lúc hồi sinh chưa
        CheckRespawnTime();
    }

    void CheckRespawnTime()
    {
        // 1. Nếu đã có quái vật trong scene, tìm tham chiếu và thoát
        currentEnemyInstance = FindAnyObjectByType<Enemy>();
        if (currentEnemyInstance != null) return;
        
        // 2. Tính toán thời gian
        float lastKillTime = PlayerPrefs.GetFloat(LastKillTimeKey, 0f);
        float currentTime = Time.time;
        float timeSinceLastKill = currentTime - lastKillTime;

        if (timeSinceLastKill >= respawnTime)
        {
            SpawnEnemy(); // Spawn ngay lập tức
        }
        else
        {
            // Hẹn giờ cho lần spawn tiếp theo
            float timeToWait = respawnTime - timeSinceLastKill;
            Invoke(nameof(SpawnEnemy), timeToWait);
            Debug.Log($"[EnemyManager] Quái vật sẽ hồi sinh sau {timeToWait:F1} giây.");
        }
    }

    public void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            // Chỉ spawn nếu chưa có cá thể nào tồn tại
            if (FindAnyObjectByType<Enemy>() == null)
            {
                currentEnemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
                Debug.Log($"[EnemyManager] Quái vật {enemyPrefab.name} đã hồi sinh.");
            }
        }
    }

    /// <summary>
    /// GỌI BỞI ENEMY: Ghi lại thời điểm Enemy bị tiêu diệt và bắt đầu hẹn giờ respawn.
    /// </summary>
    public void RecordKillAndStartRespawnTimer()
    {
        // Ghi lại thời điểm giết (dùng Time.time)
        PlayerPrefs.SetFloat(LastKillTimeKey, Time.time);
        PlayerPrefs.Save();
        
        // Hủy bỏ lệnh Invoke cũ (nếu có) và bắt đầu hẹn giờ mới
        CancelInvoke(nameof(SpawnEnemy)); 
        Invoke(nameof(SpawnEnemy), respawnTime);
        currentEnemyInstance = null;
        Debug.Log($"[EnemyManager] Bắt đầu hẹn giờ hồi sinh mới: {respawnTime} giây.");
    }

    /// <summary>
    /// GỌI BỞI GAMEMANAGER KHI ĐĂNG XUẤT (RESET). Xóa dữ liệu và hủy quái vật hiện tại.
    /// </summary>
    public void ResetManager()
    {
        PlayerPrefs.DeleteKey(LastKillTimeKey);
        CancelInvoke(nameof(SpawnEnemy));
        
        // Hủy quái vật hiện tại nếu có
        Enemy existingEnemy = FindAnyObjectByType<Enemy>();
        if (existingEnemy != null)
        {
            Destroy(existingEnemy.gameObject);
        }
        currentEnemyInstance = null;
        
        Debug.Log("[EnemyManager] Đã reset trạng thái Manager.");
    }
}