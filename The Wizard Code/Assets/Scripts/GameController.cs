using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public Sprite[] runesSprites;
    public GameObject winText;

    #region Input Managment
    public static CharacterController Controller;
    private bool input;
    #endregion

    #region Scene Manager
    public bool sceneChangeEnabled;
    public string nextSceneName;
    public Animator fadeAnimator;
    public float fadeDuration = 1f;
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
        fadeAnimator.Play("FadeToBlack");
        StartCoroutine(DelayFade());
    }

    IEnumerator DelayFade()
    {
        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(nextSceneName);
    }

    public void EndGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LevelWin()
    {
        winText.SetActive(true);
        sceneChangeEnabled = true;
    }
}
