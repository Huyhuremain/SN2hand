using UnityEngine;
using UnityEngine.UI;
using System.Linq; // Cần dùng cho FindObjectsByType

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected float enemyMoveSpeed = 3f;
    protected Player player;
    [SerializeField] protected float maxHp = 50f;
    protected float currentHp;
    [SerializeField] private Image hpBar;

    [SerializeField] protected float enterDamage = 10f;
    [SerializeField] protected float stayDamage = 1f;

    // THAM CHIẾU MỚI
    protected EnemyManager enemyManager; 

    protected virtual void Awake() // Đã sửa: dùng Awake để tìm Manager và Player
    {
        // SỬA: Dùng FindObjectsByType để tránh cảnh báo
        player = FindObjectsByType<Player>(FindObjectsSortMode.None).FirstOrDefault();
        
        // TÌM ENEMY MANAGER TRONG AWAKE
        enemyManager = FindAnyObjectByType<EnemyManager>();
    }

    protected virtual void Start()
    {
        currentHp = maxHp;
        UpdateHpBar();
    }

    protected virtual void Update()
    {
        MoveToPlayer();
    }

    protected void MoveToPlayer()
    {
        if(player != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemyMoveSpeed*Time.deltaTime);
            FlipEnemy();
        }
    }

    protected void FlipEnemy()
    {
        if(player != null)
        {
            transform.localScale = new Vector3(player.transform.position.x < transform.position.x ? -1 : 1, 1, 1);
        }
    }
    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        UpdateHpBar();
        if(currentHp <= 0)
        {
            Die();
        } 
    }
    protected virtual void Die()
    {
        // GỌI HÀM CỦA MANAGER: Thông báo Enemy đã chết để Manager bắt đầu hẹn giờ Respawn
        if (enemyManager != null)
        {
            enemyManager.RecordKillAndStartRespawnTimer();
        }
        
        Destroy(gameObject);
    }
    protected void UpdateHpBar()
    {
        if(hpBar != null)
        {
            hpBar.fillAmount = currentHp/maxHp;
        }
    }
}