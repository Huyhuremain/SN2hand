using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    
    // Đặt là public để các script AuthManager/GameUi có thể truy cập
    [HideInInspector] public float maxHp = 100f;
    [HideInInspector] public float currentHp; 
    
    [SerializeField] private Image hpBar;
    [SerializeField] private GameManager gameManager;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent <SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    
    void Start()
    {
        // Khởi tạo máu ban đầu (chỉ dùng nếu không có dữ liệu tải về)
        currentHp = maxHp;
        UpdateHpBar();
    }

    void Update()
    {
        MovePlayer();
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.PauseGameMenu();
        }
    }
    
    void MovePlayer()
    {
        // Logic di chuyển (Không đổi)
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        rb.linearVelocity = playerInput.normalized * moveSpeed;
        
        if(playerInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }else if(playerInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        
        if(playerInput != Vector2.zero)
        {
            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        UpdateHpBar();
        if(currentHp <= 0)
        {
            Die();
        } 
    }
    
    private void Die()
    {
        gameManager.GameOverMenu();
    }
    
    // HÀM MỚI: Dùng để TẢI máu từ GameUi
    public void SetHealth(float hp)
    {
        currentHp = hp;
        UpdateHpBar();
    }
    
    // HÀM MỚI: Dùng để RESET máu từ GameUi (Đăng xuất)
    public void ResetHealth()
    {
        currentHp = maxHp;
        UpdateHpBar();
    }
    
    private void UpdateHpBar()
    {
        if(hpBar != null)
        {
            hpBar.fillAmount = currentHp/maxHp;
        }
    }
}