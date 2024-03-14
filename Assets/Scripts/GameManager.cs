using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public GameObject pauseMenu;
    public Animator transition;

    public bool isPaused = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }


    }

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused && SceneManager.GetActiveScene().buildIndex == 1)
        {
            Pause();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            Resume();
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        isPaused = true;

        //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        isPaused = false;

        //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        Time.timeScale = 1f;
    }

    public void StartNextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) AudioManager.Instance.Play("SwordClash");
        StartCoroutine(NextLevel());
    }

    public void RestartCurrentLevel()
    {
        StartCoroutine(RestartLevel());
    }

    public void BackOneLevel()
    {
        Resume();
        StartCoroutine(PreviousLevel());
    }

    public void LoadTitleLevel()
    {
        StartCoroutine(LoadTitle());
    }

    IEnumerator PreviousLevel()
    {
        transition.SetTrigger("CutIn");
        yield return new WaitForSeconds(2f);
        AudioManager.Instance.Stop("Music");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        AudioManager.Instance.Play("MenuMusic");
    }

    IEnumerator LoadTitle()
    {
        transition.SetTrigger("CutIn");
        yield return new WaitForSeconds(2f);
        AudioManager.Instance.Stop("Music");
        SceneManager.LoadScene(0);
        AudioManager.Instance.Play("MenuMusic");
    }

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(0.1f);
        transition.SetTrigger("CutIn");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator RestartLevel()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.7f);
        transition.SetTrigger("CutIn");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
