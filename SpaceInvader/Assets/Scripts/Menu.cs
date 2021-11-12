using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(playButton.gameObject, new BaseEventData(EventSystem.current));
#if UNITY_WEBGL
        exitButton.gameObject.SetActive(false);
#endif
    }

    public void StartGame()
    {
        Session.ArmyMoveTime = 2f;
        Session.Lives = 3;
        Session.Score = 0;
        Session.Level = 1;

        Invoke("Load_GameScene", 1f);
    }
    private void Load_GameScene()
    {
        SceneManager.LoadScene("Game");
    }
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
