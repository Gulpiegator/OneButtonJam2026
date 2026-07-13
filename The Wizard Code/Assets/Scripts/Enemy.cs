using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int[] targetRunes;

    [Header("Rune Display Settings")]
    [SerializeField] private float runeSize = 0.2f;
    [SerializeField] private float runeSpacing = 0.5f;
    [SerializeField] private float runeVerticalPositionOffset = 0f;

    [Header("Enemy Behavior Settings")]
    [SerializeField] private float appearanceDelay = 1f;
    [SerializeField] private float attackDelay = 1f;
    [SerializeField] private bool doesAttack = true;

    [Header("Attack Bar Settings")]
    [SerializeField] private float attackBarWidth = 0.8f;
    [SerializeField] private float attackBarHeight = 0.08f;
    [SerializeField] private float attackBarVerticalPositionOffset = 0.35f;
    [SerializeField] private Color attackBarColor = Color.red;

    // Renderers
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer[] runeRenderers;
    private SpriteRenderer attackBarRenderer;
    // Timers
    private float appearanceTimer;
    private float attackTimer;
    // States
    private bool hasAppeared;
    private bool hasAttacked;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        CreateRuneSprites();
        CreateAttackBar();
    }

    private void Start()
    {
        spriteRenderer.enabled = false;
        SetRuneSpritesVisible(false);
        SetAttackBarVisible(false);
        appearanceTimer = appearanceDelay;
        attackTimer = attackDelay;
        RefreshRuneSprites();
        UpdateAttackBarVisual();
    }

    private void Update()
    {
        if (GameController.Instance.sceneChangeEnabled)
        {
            Destroy(gameObject);
        }
        
        if (!hasAppeared)
        {
            appearanceTimer -= Time.deltaTime;
            if (appearanceTimer <= 0f)
            {
                hasAppeared = true;
                spriteRenderer.enabled = true;
                SetRuneSpritesVisible(true);
                SetAttackBarVisible(true);
            }
        }
        else
        {
            if (!doesAttack || hasAttacked)
            {
                return;
            }

            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                hasAttacked = true;
                Attack();
                return;
            }

            UpdateAttackBarVisual();
        }
    }

    private void CreateRuneSprites()
    {
        runeRenderers = new SpriteRenderer[targetRunes.Length];
        for (int i = 0; i < targetRunes.Length; i++)
        {
            GameObject runeObject = new GameObject($"EnemyRune{i}");
            runeObject.transform.SetParent(transform, false);

            SpriteRenderer runeRenderer = runeObject.AddComponent<SpriteRenderer>();

            runeRenderer.sortingOrder = spriteRenderer != null ? spriteRenderer.sortingOrder + 1 : 10;
            runeRenderer.enabled = false;
            runeRenderers[i] = runeRenderer;
        }
    }

    // Positions sprites above the enemy
    private void RefreshRuneSprites()
    {
        float totalWidth = Mathf.Max(0f, (targetRunes.Length - 1) * runeSpacing);
        Vector3 startPosition = new Vector3(-totalWidth / 2f, runeVerticalPositionOffset, 0f);

        for (int i = 0; i < runeRenderers.Length; i++)
        {
            runeRenderers[i].transform.localPosition = startPosition + Vector3.right * (i * runeSpacing);
            runeRenderers[i].transform.localScale = Vector3.one * runeSize;
            runeRenderers[i].sprite = GameController.Instance.runesSprites[targetRunes[i]];
        }
    }

    private void SetRuneSpritesVisible(bool visible)
    {
        foreach (SpriteRenderer runeRenderer in runeRenderers)
        {
            runeRenderer.enabled = visible;
        }
    }

    private void CreateAttackBar()
    {
        GameObject attackBarObject = new GameObject("EnemyAttackBar");
        attackBarObject.transform.SetParent(transform, false);

        attackBarRenderer = attackBarObject.AddComponent<SpriteRenderer>();
        attackBarRenderer.sortingOrder = spriteRenderer != null ? spriteRenderer.sortingOrder + 2 : 11;
        attackBarRenderer.color = attackBarColor;
        attackBarRenderer.enabled = false;

        Texture2D barTexture = new Texture2D(1, 1);
        barTexture.SetPixel(0, 0, attackBarColor);
        barTexture.Apply();

        attackBarRenderer.sprite = Sprite.Create(
            barTexture,
            new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f),
            100f
        );

        attackBarObject.transform.localPosition = new Vector3(0f, attackBarVerticalPositionOffset, 0f);
        attackBarObject.transform.localScale = new Vector3(attackBarWidth*100, attackBarHeight*100, 1f);
    }
    private void SetAttackBarVisible(bool visible)
    {
        if (attackBarRenderer != null)
        {
            attackBarRenderer.enabled = visible;
        }
    }

    private void UpdateAttackBarVisual()
    {
        if (attackBarRenderer == null)
        {
            return;
        }

        float progress = doesAttack ? Mathf.Clamp01(attackTimer / attackDelay) : 1f;
        attackBarRenderer.transform.localScale = new Vector3(progress * attackBarWidth * 100, attackBarHeight * 100, 1f);
    }

    private void Attack()
    {
        if (spriteRenderer != null)
        {
            Debug.Log("Enemy attacks!");
        }
        // SetRuneSpritesVisible(false);
        // SetAttackBarVisible(false);
        if (GameController.Instance != null)
        {
            GameController.Instance.EndGame();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
