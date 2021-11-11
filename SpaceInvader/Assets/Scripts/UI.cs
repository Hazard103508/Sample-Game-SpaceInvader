using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private List<GameObject> lstLifeIcons;

    // Start is called before the first frame update
    void Start()
    {
        Session.LifeChanged += Session_LifeChanged;
    }

    private void Session_LifeChanged()
    {
        for (int i = 0; i < lstLifeIcons.Count; i++)
            lstLifeIcons[i].SetActive(Session.Lives > i);
    }
}
