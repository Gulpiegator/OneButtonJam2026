using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public Sprite[] runesSprites;

    #region Input Managment
    public static CharacterController Controller;
    private bool input;
    #endregion

    #region Scene Manager
    public bool sceneChangeEnabled;
    public string nextSceneName;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Instance = this;
        Controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sceneChangeEnabled) {

            // Check if the Spacebar was pressed down during this frame
            if (input) 
            { 
                LoadNextScene(); 
            } 
        }
    }

    public void OnPressSpacebar(InputAction.CallbackContext context)
    {
        input = context.ReadValue<float>() > 0.5f;
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
