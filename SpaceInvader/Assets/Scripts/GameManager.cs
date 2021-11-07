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

    private int rows = 4;
    private int columns = 10;
    private Enemy[,] enemies;
    private GameState _state;

    /// <summary>
    /// Estado del juego en curso
    /// </summary>
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
                for (int col = 0; col < columns; col++)
                    this.enemies[0, col].State = Enemy.EnemyState.Shooting; // le indico a la primera fila que comiece a disparar

                player.enabled = true;
            }
        }
    }

    #region Unity Methods
    void Start()
    {
        State = GameState.Loading;
    }
    #endregion


    #region Methods
    /// <summary>
    /// Carga las naves enemigas en el mapa
    /// </summary>
    private IEnumerator Load_Enemies()
    {
        var parentFolder = GameObject.Find("Enemies");
        enemies = new Enemy[4, 10];

        Vector2 marginPos = new Vector2(3, 1.8f);
        Vector2 startPos = new Vector2(-(columns - 1) * (marginPos.x / 2), 3);

        for (int row = 0; row < rows; row++)
            for (int col = 0; col < columns; col++)
            {
                var pref = EnemyPref[Random.Range(0, EnemyPref.Length)];
                var obj = Instantiate(pref, parentFolder.transform);
                obj.transform.position = startPos + new Vector2(col, row) * marginPos;

                var enemy = obj.GetComponent<Enemy>();
                enemy.location = new Vector2Int(col, row);
                enemy.Destroyed.AddListener(OnEnemyDestroyed);
                enemies[row, col] = enemy;

                yield return new WaitForSeconds(0.05f); // aplico demora para generar efecto de aparicion en fila como el juego original
            }

        this.State = GameState.Playing;
    }
    /// <summary>
    /// Al destruir una nave valida destruye las naves cercanas del mismo color
    /// </summary>
    /// <param name="enemy">Enemigo destruido por el player</param>
    private void OnEnemyDestroyed(Enemy enemy)
    {
        this.enemies[enemy.location.y, enemy.location.x] = null;

        Enemy leftEnemy = enemy.location.x > 0 ? this.enemies[enemy.location.y, enemy.location.x - 1] : null;
        Enemy rightEnemy = enemy.location.x < this.columns - 1 ? this.enemies[enemy.location.y, enemy.location.x + 1] : null;
        Enemy topEnemy = enemy.location.y < this.rows - 1 ? this.enemies[enemy.location.y + 1, enemy.location.x] : null;
        Enemy bottomEnemy = enemy.location.y > 0 ? this.enemies[enemy.location.y - 1, enemy.location.x] : null;

        // al cambiar el estado a la nave se desencadenara el evento indicando que fue destruida generando una llada recursiva
        if (leftEnemy != null && leftEnemy.model == enemy.model)
            leftEnemy.State = Enemy.EnemyState.Dying;

        if (rightEnemy != null && rightEnemy.model == enemy.model)
            rightEnemy.State = Enemy.EnemyState.Dying;

        if (bottomEnemy != null && bottomEnemy.model == enemy.model)
            bottomEnemy.State = Enemy.EnemyState.Dying;

        if (topEnemy != null && topEnemy.model == enemy.model)
            topEnemy.State = Enemy.EnemyState.Dying;


        // recorre la columna de la nave eliminada para buscar la siguiente nave que tiene q disparar
        for (int row = 0; row < this.rows; row++)
        {
            Enemy shootingEnemy = this.enemies[row, enemy.location.x];
            if (shootingEnemy != null && shootingEnemy.State != Enemy.EnemyState.Dying) 
            {
                shootingEnemy.State = Enemy.EnemyState.Shooting; 
                break;
            }
        }
    }
    /// <summary>
    /// Desplaza los enemigos por el escenario
    /// </summary>
    //private void MoveEnemies()
    //{
    //    enemies.ForEach(obj =>
    //    {
    //        obj.transform.Translate(Vector3.right * 1);
    //    });
    //}
    #endregion

    #region Structures
    public enum GameState
    {
        Loading,
        Playing
    }
    #endregion
}
