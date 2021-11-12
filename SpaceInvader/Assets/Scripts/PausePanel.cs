using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private Button continueButton;

    void Start()
    {
        Hide(); // lo desactivo desde codigo para tenerlo visible en el editor siempre
    }

    public void Exit()
    {
        SceneManager.LoadScene("Main");
    }
    /// <summary>
    /// Muestra el panel
    /// </summary>
    public void Show()
    {
        this.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(continueButton.gameObject, new BaseEventData(EventSystem.current));
    }
    /// <summary>
    /// Oculta el panel
    /// </summary>
    public void Hide()
    {
        this.gameObject.SetActive(false); // lo desactivo desde codigo para tenerlo visible en el editor siempre
    }
}
