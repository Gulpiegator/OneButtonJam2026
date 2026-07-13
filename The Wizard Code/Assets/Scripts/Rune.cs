using UnityEngine;

public class Rune : MonoBehaviour
{
    public float moveSpeed;
    public int runeType = -1; // So we know if it changes
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private bool spriteSet = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.sceneChangeEnabled)
        {
            Destroy(gameObject);
        }
        
        if(runeType != -1 && !spriteSet) // Checks that runeType is given and sprite isn't already set
        {
            spriteRenderer.sprite = GameController.Instance.runesSprites[runeType]; // Set sprite from GameController list
            spriteSet = true; // Indicate sprite has been set
        }
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        // Destroy the rune if it goes off screen
        Vector3 screenPosition = mainCamera.WorldToViewportPoint(transform.position);
        if (screenPosition.x < 0)
        {
            Destroy(gameObject);
        }
    }
}
