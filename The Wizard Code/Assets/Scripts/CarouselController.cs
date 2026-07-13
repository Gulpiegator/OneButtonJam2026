using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarouselController : MonoBehaviour
{
    [Header("Rune Settings")]
    [SerializeField] private int[] levelRunes;
    [SerializeField] private Vector2 runeSpawnPoint;
    [SerializeField] private float runeSpawnInterval;
    [SerializeField] private float runeMoveSpeed;
    [SerializeField] private int runesPerRotation;

    [Header("Selection Settings")]
    [SerializeField] private Vector2 selectionPoint;
    [SerializeField] private Vector2 runeDisplayCenter;
    [SerializeField] private float runeSize = 0.4f;
    [SerializeField] private float runeSpacing = 0.8f;
    private float castSpellDelay;

    [Header("Drag Settings")]
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject runePrefab;

    // Private variables
    private int[] enemyRunes;
    private List<int> currentRotation;
    private int currentRuneIndex;
    private float runeSpawnTimer;
    private List<int> selectedRunes;
    private SpriteRenderer[] selectedRuneDisplayRenderers;
    private int requiredRuneCount;
    private bool spellQueued;
    private float spellCastTimer;
    private bool input;
    private int remainingEnemyCount = 0;

    void Start()
    {
        castSpellDelay = .1f;
        enemyRunes = GatherUniqueEnemyRunes();

        requiredRuneCount = enemies[0].GetComponent<Enemy>().targetRunes.Length;

        selectedRunes = new List<int>(requiredRuneCount);
        CreateSelectedRuneDisplay();
        CreateNewRotation();
    }

    void Update()
    {
        if (GameController.Instance.sceneChangeEnabled)
        {
            Destroy(gameObject);
        }

        // Timer for spawning runes
        runeSpawnTimer += Time.deltaTime;
        if (runeSpawnTimer >= runeSpawnInterval)
        {
            runeSpawnTimer = 0f;
            SpawnNextRune();
        }

        bool spacePressed = input || (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame);
        if (spacePressed)
        {
            Debug.Log("Spacebar pressed");
            input = false;
            SelectRune();
        }

        if (spellQueued)
        {
            spellCastTimer -= Time.deltaTime;
            if (spellCastTimer <= 0f)
            {
                CastSpell();
            }
        }
    }

    public void OnPressSpacebar(InputAction.CallbackContext context)
    {
        input = context.ReadValue<float>() > 0.5f;
    }

    private int[] GatherUniqueEnemyRunes()
    {
        var uniqueRunes = new List<int>();
        var seenRunes = new HashSet<int>();

        foreach (var enemyObject in enemies)
        {
            remainingEnemyCount++;

            var enemyScript = enemyObject.GetComponent<Enemy>();

            foreach (var runeType in enemyScript.targetRunes)
            {
                if (seenRunes.Add(runeType))
                {
                    uniqueRunes.Add(runeType);
                }
            }
        }

        return uniqueRunes.ToArray();
    }

    private void CreateSelectedRuneDisplay()
    {
        selectedRuneDisplayRenderers = new SpriteRenderer[requiredRuneCount];
        for (int i = 0; i < requiredRuneCount; i++)
        {
            GameObject displayRune = new GameObject($"SelectedRuneDisplay{i}");
            displayRune.transform.SetParent(transform, false);
            SpriteRenderer runeRenderer = displayRune.AddComponent<SpriteRenderer>();
            runeRenderer.sortingOrder = 100;
            runeRenderer.enabled = false;
            selectedRuneDisplayRenderers[i] = runeRenderer;
        }

        RefreshSelectedRuneDisplay();
    }

    private void RefreshSelectedRuneDisplay()
    {
        for (int i = 0; i < selectedRuneDisplayRenderers.Length; i++)
        {
            SpriteRenderer runeRenderer = selectedRuneDisplayRenderers[i];
            if (runeRenderer == null)
            {
                continue;
            }

            if (i < selectedRunes.Count)
            {
                runeRenderer.transform.position = GetDisplayPosition(i);
                runeRenderer.transform.localScale = Vector3.one * runeSize;
                runeRenderer.enabled = true;

                if (GameController.Instance != null && GameController.Instance.runesSprites != null && GameController.Instance.runesSprites.Length > selectedRunes[i])
                {
                    runeRenderer.sprite = GameController.Instance.runesSprites[selectedRunes[i]];
                }
            }
            else
            {
                runeRenderer.enabled = false;
            }
        }
    }

    private Vector3 GetDisplayPosition(int index)
    {
        float totalWidth = Mathf.Max(0f, (requiredRuneCount - 1) * runeSpacing);
        Vector3 startPosition = new Vector3(runeDisplayCenter.x, runeDisplayCenter.y, 0f) + Vector3.left * (totalWidth / 2f);
        return startPosition + Vector3.right * (index * runeSpacing);
    }

    private void CreateNewRotation()
    {
        currentRotation = new List<int>();
        currentRuneIndex = 0;

        currentRotation.AddRange(enemyRunes);

        // Fill the rest with random runes
        int remainingSlots = runesPerRotation - currentRotation.Count;
        for (int i = 0; i < remainingSlots; i++)
        {
            int randomIndex = Random.Range(0, levelRunes.Length);
            currentRotation.Add(levelRunes[randomIndex]);
        }

        ShuffleList(currentRotation);
    }

    private void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void SpawnNextRune()
    {
        if (currentRuneIndex >= currentRotation.Count)
        {
            CreateNewRotation();
        }

        int runeId = currentRotation[currentRuneIndex];
        GameObject spawnedRune = Instantiate(runePrefab, runeSpawnPoint, Quaternion.identity);

        Rune rune = spawnedRune.GetComponent<Rune>();
        rune.moveSpeed = runeMoveSpeed;
        rune.runeType = runeId;

        currentRuneIndex++;
    }

    private void SelectRune()
    {
        if (selectedRunes.Count >= requiredRuneCount || spellQueued)
        {
            Debug.Log("Cannot select more runes or spell is already queued.");
            return;
        }

        Rune runeAtSelection = FindRuneAtSelectionPoint();
        if (runeAtSelection == null)
        {
            Debug.Log("No rune found at selection point.");
            return;
        }

        Debug.Log($"Selected rune of type: {runeAtSelection.runeType}");

        if (runeAtSelection.runeType >= 0)
        {
            selectedRunes.Add(runeAtSelection.runeType);
            RefreshSelectedRuneDisplay();
            Destroy(runeAtSelection.gameObject);
        }

        if (selectedRunes.Count >= requiredRuneCount)
        {
            spellQueued = true;
            spellCastTimer = castSpellDelay;
        }
    }

    private Rune FindRuneAtSelectionPoint()
    {
        Collider2D hitCollider = Physics2D.OverlapPoint(selectionPoint);
        if (hitCollider != null)
        {
            Rune rune = hitCollider.GetComponent<Rune>();
            if (rune != null)
            {
                return rune;
            }

            return hitCollider.GetComponentInParent<Rune>();
        }

        foreach (Rune rune in Object.FindObjectsByType<Rune>())
        {
            if (Vector2.Distance(rune.transform.position, selectionPoint) < 0.2f)
            {
                return rune;
            }
        }

        return null;
    }

    private void CastSpell()
    {
        spellQueued = false;
        spellCastTimer = 0f;

        if (selectedRunes.Count == 0)
        {
            return;
        }

        CompareSpell();
        selectedRunes.Clear();
        RefreshSelectedRuneDisplay();
    }

    private void CompareSpell()
    {
        foreach (var enemyObject in enemies)
        {
            if (enemyObject == null)
            {
                continue;
            }

            var enemyScript = enemyObject.GetComponent<Enemy>();
            if (enemyScript == null || enemyScript.targetRunes == null)
            {
                continue;
            }

            if (MatchesSelectedRunes(enemyScript.targetRunes))
            {
                DestroyEnemy(enemyObject);
            }
        }
    }

    private bool MatchesSelectedRunes(int[] targetRunes)
    {
        if (targetRunes.Length != selectedRunes.Count)
        {
            return false;
        }

        for (int i = 0; i < targetRunes.Length; i++)
        {
            if (targetRunes[i] != selectedRunes[i])
            {
                return false;
            }
        }

        return true;
    }

    private void DestroyEnemy(GameObject enemyObject)
    {
        if (enemyObject != null)
        {
            Destroy(enemyObject);
        }

        if (remainingEnemyCount > 0)
        {
            remainingEnemyCount--;
        }

        if (remainingEnemyCount <= 0)
        {
            GameController.Instance.LevelWin();
        }
    }
}
