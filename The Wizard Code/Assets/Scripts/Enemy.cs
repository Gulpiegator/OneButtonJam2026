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
    
    // Renderers
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer[] runeRenderers;
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
    }

    private void Start()
    {
        spriteRenderer.enabled = false;
        SetRuneSpritesVisible(false);
        appearanceTimer = appearanceDelay;
        attackTimer = attackDelay;
        RefreshRuneSprites();
    }

    private void Update()
    {
        if (!hasAppeared)
        {
            appearanceTimer -= Time.deltaTime;
            if (appearanceTimer <= 0f)
            {
                hasAppeared = true;
                spriteRenderer.enabled = true;
                SetRuneSpritesVisible(true);
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
            }
        }
    }

    // Create rune sprites and store them in array
    private void CreateRuneSprites()
    {
        runeRenderers = new SpriteRenderer[targetRunes.Length];
        for (int i = 0; i < targetRunes.Length; i++)
        {
            GameObject runeObject = new GameObject($"EnemyRune{i}");
            runeObject.transform.SetParent(transform, false);

            SpriteRenderer runeRenderer = runeObject.GetComponent<SpriteRenderer>();
            runeRenderer = runeObject.AddComponent<SpriteRenderer>();

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

    // Set visibility of rune sprites
    private void SetRuneSpritesVisible(bool visible)
    {
        foreach (SpriteRenderer runeRenderer in runeRenderers)
        {
                runeRenderer.enabled = visible;
        }
    }

    private void Attack()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        SetRuneSpritesVisible(false);
        if (GameController.Instance != null)
        {
            //GameController.Instance.EndGame();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
