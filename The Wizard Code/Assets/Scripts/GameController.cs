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

    [Header("Sound Settings")]
    [SerializeField] private AudioClip runeSelectSound;
    [SerializeField] private AudioClip spellCastSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Instance = this;
        Controller = GetComponent<CharacterController>();

        PlayBackgroundMusic();
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

    private void PlayBackgroundMusic()
    {
        if (audioSource == null || backgroundMusic == null)
        {
            return;
        }

        audioSource.loop = true;
        audioSource.clip = backgroundMusic;
        audioSource.volume = 1f;
        audioSource.Play();
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    public void PlayRuneSelectSound()
    {
        PlaySound(runeSelectSound);
    }

    public void PlaySpellCastSound()
    {
        PlaySound(spellCastSound);
    }

    public void PlayLoseSound()
    {
        PlaySound(loseSound);
    }

    public void PlayWinSound()
    {
        PlaySound(winSound);
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
        PlayWinSound();
        winText.SetActive(true);
        sceneChangeEnabled = true;
    }
}
