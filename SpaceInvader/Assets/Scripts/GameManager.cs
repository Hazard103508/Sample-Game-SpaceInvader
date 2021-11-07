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
    private EnemiesDirection enemiesDirection;

    /// <summary>
    /// Estado del juego en curso
    /// </summary>
    private GameState State
    {
        get => _state;
        set
        {
            _state = value;
            OnStateChange();
        }
    }

    #region Unity Methods
    void Start()
    {
        State = GameState.Loading;
        enemiesDirection = EnemiesDirection.Right;
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
    private void OnStateChange()
    {
        if (this.State == GameState.Loading)
            StartCoroutine(Load_Enemies());
        else if (this.State == GameState.Playing)
        {
            for (int col = 0; col < columns; col++)
                this.enemies[0, col].State = Enemy.EnemyState.Shooting; // le indico a la primera fila que comiece a disparar

            player.enabled = true;
            InvokeRepeating("MoveEnemies", 2, 2);
        }
    }
    /// <summary>
    /// Desplaza los enemigos por el escenario
    /// </summary>
    private void MoveEnemies()
    {
        float x = enemiesDirection == EnemiesDirection.Right ? 1 : -1;
        float y = 1;

        Enemy firtEnemy = Get_FirtColumnEnemy();
        if (firtEnemy != null)
        {
            Vector3 translation = Vector3.zero;
            if (firtEnemy.Can_Move(x))
                translation = Vector3.right * x; // si la primera nave encontrada puede moverse sin salirse de los limites, se aplica el movimiento a todas las nave
            else
            {
                translation = Vector3.down * y; // si no hay movimiento posible las naven bajan una fila
                enemiesDirection = enemiesDirection == EnemiesDirection.Right ? EnemiesDirection.Left : EnemiesDirection.Right;
            }

            for (int row = 0; row < rows; row++)
                for (int col = 0; col < columns; col++)
                {
                    var enemy = enemies[row, col];
                    if (enemy != null)
                        enemy.transform.Translate(translation);
                }
        }
    }
    /// <summary>
    /// Obtengo el primer enemigo vivo en la columna de la izquierda o derecha
    /// </summary>
    /// <returns></returns>
    private Enemy Get_FirtColumnEnemy()
    {
        if (enemiesDirection == EnemiesDirection.Right)
            for (int col = this.columns - 1; col >= 0; col--)
                for (int row = 0; row < rows; row++)
                {
                    var enemy = enemies[row, col];
                    if (enemy != null)
                        return enemy;
                }
        else
            for (int col = 0; col < columns; col++)
                for (int row = 0; row < rows; row++)
                {
                    var enemy = enemies[row, col];
                    if (enemy != null)
                        return enemy;
                }

        return null;
    }
    #endregion

    #region Structures
    public enum EnemiesDirection
    {
        Left,
        Right
    }
    public enum GameState
    {
        Loading,
        Playing
    }
    #endregion
}
