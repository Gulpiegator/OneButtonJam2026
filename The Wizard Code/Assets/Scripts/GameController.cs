using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public Sprite[] runesSprites;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
