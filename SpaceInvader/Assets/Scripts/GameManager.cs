using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject[] EnemyPref;

    [Header("Components")]
    [SerializeField] private Player player;

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
            {
                Enable_EnemiesWeapon();
                player.enabled = true;
            }
        }
    }

    void Start()
    {
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
        Vector2 marginPos = new Vector2(3, 1.5f);
        Vector2 startPos = new Vector2(-(enemyCount.x - 1) * (marginPos.x / 2), 3);

        int row = 0;
        int col = 0;
        while (true)
        {
            var pref = EnemyPref[Random.Range(0, EnemyPref.Length)];
            var obj = Instantiate(pref, parentFolder.transform);
            obj.transform.position = startPos + new Vector2(col, row) * marginPos;

            var enemy = obj.GetComponent<Enemy>();
            enemy.location = new Vector2Int(col, row);
            enemies.Add(enemy);

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
    /// Activa el arma de las naves para que puedan disparar
    /// </summary>
    private void Enable_EnemiesWeapon()
    {
        bool[] shootingCol = new bool[10]; // indica las columnas de naves enemigas que disparan

        // recorro las naves en orden de creacion (abajo/izquierda - arriba/derecha)
        foreach (Enemy enemy in enemies)
        {
            bool isShooting = shootingCol[enemy.location.x]; // determina si en la columna que se encuentra la nave existe alguna nave disparando
            if (!isShooting)
            {
                enemy.canShoot = true; // le indico a la nave que puede disparar
                shootingCol[enemy.location.x] = true; // marco el flag para que las las otras naves de la misma columna no disparen
            }
        }
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
