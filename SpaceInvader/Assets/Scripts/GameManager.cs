using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject[] EnemyPref;

    [Header("Components")]
    [SerializeField] private Player player;

    private Vector2 screenBounds;
    private List<Enemy> enemies;
    private GameState _state;

    private GameState State
    {
        get => _state;
        set
        {
            _state = value;

            if (value == GameState.Loading)
                StartCoroutine(Load_Enemies());
            else if (value == GameState.Playing)
                player.enabled = true;
        }
    }

    void Start()
    {
        this.screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        State = GameState.Loading;
    }

    void Update()
    {
    }

    #region Methods
    /// <summary>
    /// Carga las naves enemigas en el mapa
    /// </summary>
    private IEnumerator Load_Enemies()
    {
        var parentFolder = GameObject.Find("Enemies");
        enemies = new List<Enemy>();

        Vector2Int enemyCount = new Vector2Int(10, 4);
        Vector2 marginPos = new Vector2(3, 2);
        Vector2 startPos = new Vector2(-(enemyCount.x - 1) * (marginPos.x / 2), 10);

        int row = 0;
        int col = 0;
        while (true)
        {
            var pref = EnemyPref[Random.Range(0, EnemyPref.Length)];
            var obj = Instantiate(pref, parentFolder.transform);
            obj.transform.position = startPos + new Vector2(col, row) * marginPos;
            enemies.Add(obj.GetComponent<Enemy>());

            col++;
            if (col == enemyCount.x)
            {
                col = 0;
                row++;
                if (row == enemyCount.y)
                    break;
            }

            yield return new WaitForSeconds(0.05f); // aplico demora para generar efecto de aparicion en fila como el juego original
        }

        //InvokeRepeating("MoveEnemies", 1f, 2f);
        this.State = GameState.Playing;
    }
    /// <summary>
    /// Desplaza los enemigos por el escenario
    /// </summary>
    private void MoveEnemies()
    {
        enemies.ForEach(obj =>
        {
            obj.transform.Translate(Vector3.right * 1);
        });
    }
    #endregion

    #region Structures
    public enum GameState
    {
        Loading,
        Playing
    }
    #endregion
}
