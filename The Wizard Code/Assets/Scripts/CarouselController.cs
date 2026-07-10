using System.Collections.Generic;
using UnityEngine;

public class CarouselController : MonoBehaviour
{
    [Header("Rune Settings")]
    [SerializeField] private int[] levelRunes;
    [SerializeField] private Vector2 runeSpawnPoint;
    [SerializeField] private float runeSpawnInterval;
    [SerializeField] private float runeMoveSpeed;
    [SerializeField] private int runesPerRotation;
    [Header("Drag Settings")]
    [SerializeField] private GameObject[] Enemies;
    [SerializeField] private GameObject runePrefab;
    
    // Private variables
    private int[] enemyRunes;
    private List<int> currentRotation;
    private int currentRuneIndex;
    private float runeSpawnTimer;

    void Start()
    {
        // Goes through each enemy and adds their runes to a list and converts to array
        var tempRunesList = new List<int>();
        foreach (var enemyObject in Enemies)
        {
            var enemyScript = enemyObject.GetComponent<Enemy>();
            tempRunesList.AddRange(enemyScript.targetRunes);
        }
        enemyRunes = tempRunesList.ToArray();

        CreateNewRotation(); // Creates first rotation
    }

    void Update()
    {
        // Timer for spawning runes
        runeSpawnTimer += Time.deltaTime;

        if (runeSpawnTimer >= runeSpawnInterval)
        {
            runeSpawnTimer = 0f;
            SpawnNextRune();
        }
    }

    // Creates a list of runes at random always including the enemy runes
    void CreateNewRotation()
    {
        // Erase the old list and reset index
        currentRotation = new List<int>();
        currentRuneIndex = 0;

        currentRotation.AddRange(enemyRunes); // Add the enemy runes to make sure they are present

        // Fill the rest with random runes
        int remainingSlots = runesPerRotation - currentRotation.Count;
        for (int i = 0; i < remainingSlots; i++)
        {
            if (levelRunes.Length > 0)
            {
                int randomIndex = Random.Range(0, levelRunes.Length);
                currentRotation.Add(levelRunes[randomIndex]);
            }
        }

        ShuffleList(currentRotation);
    }

    // Shuffle the list of runes
    void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // Spawn a rune and give it speed and type
    void SpawnNextRune()
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
}
