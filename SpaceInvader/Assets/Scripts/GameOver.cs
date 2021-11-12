using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] Text score;

    void Start()
    {
        score.text = $"TOTAL SCORE {Session.Score}";
        Invoke("Load_Main", 5);
    }

    private void Load_Main()
    {
        SceneManager.LoadScene("Main");
    }
}
