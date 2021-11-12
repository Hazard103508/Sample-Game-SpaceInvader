using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Text scoreLabel;
    [SerializeField] private Text levelLabel;
    [SerializeField] private List<GameObject> lstLifeIcons;

    // Start is called before the first frame update
    void Start()
    {
        Session.LifeChanged += LifeChanged;
        Session.ScoreChanged += ScoreChanged;

        levelLabel.text = $"LEVEL {Session.Level}";
        Set_Score();
        LifeChanged();
    }

    private void LifeChanged()
    {
        for (int i = 0; i < lstLifeIcons.Count; i++)
            lstLifeIcons[i].SetActive(Session.Lives > i);
    }
    private void ScoreChanged()
    {
        Set_Score();
    }

    private void OnDestroy()
    {
        Session.LifeChanged -= LifeChanged;
        Session.ScoreChanged -= ScoreChanged;
    }

    private void Set_Score()
    {
        scoreLabel.text = $"SCORE {Session.Score}";
    }
}
