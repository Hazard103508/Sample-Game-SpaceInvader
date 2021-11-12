using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PixelButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] Text Label;
    [SerializeField] private AudioSource clickSound;
    [SerializeField] private AudioSource SelectSound;
    private Button button;

    private void Start()
    {
        if (clickSound != null)
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(onClick);
        }
    }
    private void onClick()
    {
        clickSound.Play();
        button.interactable = false; // desactivo el boton para evitar que se presione mas de una vez
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (SelectSound != null)
            SelectSound.Play();
        Label.color = Color.yellow;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Label.color = Color.white;
    }
}
