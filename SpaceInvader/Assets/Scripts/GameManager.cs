using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Objects
    [Header("Prefab")]
    [SerializeField] private GameObject[] EnemyPref;
    [SerializeField] private Text labelWin;

    [Header("Components")]
    [SerializeField] private Player player;
    [SerializeField] private Weapon enemiesWeapon;

    private EnemyArmy enemyArmy;
    private GameState _state;
    #endregion

    #region Properties
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
    #endregion

    #region Unity Methods
    void Start()
    {
        Session.EnemiesChanged += EnemiesCountChanged;
        State = GameState.Loading;
    }

    private void EnemiesCountChanged()
    {
        if (Session.Enemies == 0)
            State = GameState.Win;
    }
    private void OnDestroy()
    {
        Session.EnemiesChanged -= EnemiesCountChanged;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Carga las naves enemigas en el mapa
    /// </summary>
    private IEnumerator Load_Enemies()
    {
        var parentFolder = GameObject.Find("Enemies");
        enemyArmy = new EnemyArmy();

        Vector2 marginPos = new Vector2(3, 1.8f);
        Vector2 startPos = new Vector2(-(enemyArmy.Columns - 1) * (marginPos.x / 2), 3);

        for (int row = 0; row < enemyArmy.Rows; row++)
            for (int col = 0; col < enemyArmy.Columns; col++)
            {
                var pref = EnemyPref[Random.Range(0, EnemyPref.Length)];
                var obj = Instantiate(pref, parentFolder.transform);
                obj.transform.position = startPos + new Vector2(col, row) * marginPos;

                var enemy = obj.GetComponent<Enemy>();
                enemy.Row = row;
                enemy.Column = col;
                enemy.Destroyed.AddListener(OnEnemyDestroyed);

                this.enemyArmy[row, col] = enemy;

                yield return new WaitForSeconds(0.05f); // aplico demora para generar efecto de aparicion en fila como el juego original
            }

        Session.Enemies = this.enemyArmy.Rows * this.enemyArmy.Columns;
        this.enemyArmy.Init_FronLine();

        this.State = GameState.Playing;
    }
    /// <summary>
    /// Al destruir una nave valida destruye las naves cercanas del mismo color
    /// </summary>
    /// <param name="enemy">Enemigo destruido por el player</param>
    private void OnEnemyDestroyed(Enemy enemy)
    {
        this.enemyArmy[enemy] = null;

        Enemy sideEnemy = this.enemyArmy.Get_Left(enemy);
        if (sideEnemy != null && sideEnemy.model == enemy.model)
            sideEnemy.State = Enemy.EnemyState.Dying;

        sideEnemy = this.enemyArmy.Get_Right(enemy);
        if (sideEnemy != null && sideEnemy.model == enemy.model)
            sideEnemy.State = Enemy.EnemyState.Dying;

        sideEnemy = this.enemyArmy.Get_Top(enemy);
        if (sideEnemy != null && sideEnemy.model == enemy.model)
            sideEnemy.State = Enemy.EnemyState.Dying;

        sideEnemy = this.enemyArmy.Get_Bottom(enemy);
        if (sideEnemy != null && sideEnemy.model == enemy.model)
            sideEnemy.State = Enemy.EnemyState.Dying;
    }

    /// <summary>
    /// Funcion que se desencadena al cambiar el estado del juego
    /// </summary>
    private void OnStateChange()
    {
        if (this.State == GameState.Loading)
            StartCoroutine(Load_Enemies());
        else if (this.State == GameState.Playing)
        {
            player.enabled = true;
            InvokeRepeating("MoveEnemies", 0, Session.ArmyMoveTime);
            Shoot_ToPlayer();
        }
        else if (this.State == GameState.Win)
        {
            labelWin.gameObject.SetActive(true);
            Invoke("Next_Level", 3);
        }
    }
    /// <summary>
    /// Desplaza los enemigos por el escenario
    /// </summary>
    private void MoveEnemies()
    {
        this.enemyArmy.Move();
    }
    /// <summary>
    /// selecciona una nave de la primera fila de enemigos y dispara hacia abajo
    /// </summary>
    private void Shoot_ToPlayer()
    {
        Enemy enemy = this.enemyArmy.Get_RandomFronLineEnemy();
        if (enemy != null)
        {
            enemiesWeapon.Shoot(enemy.transform.position);
            Invoke("Shoot_ToPlayer", Random.Range(1f, 3f));
        }
    }
    /// <summary>
    /// Carga el siguiente nivel
    /// </summary>
    private void Next_Level()
    {
        if (Session.Lives > 0) // puede morir con una bala perdida
        {
            if (Session.ArmyMoveTime > 0.5f)
                Session.ArmyMoveTime -= 0.05f; // acelero la velocidad de las naves

            Session.Level++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    #endregion

    #region Structures
    public enum ArmyDirection
    {
        Left,
        Right
    }
    public enum GameState
    {
        Loading,
        Playing,
        Win
    }
    public class EnemyArmy
    {
        #region Properties
        /// <summary>
        /// Direccion de desplazamiento de los enemigos
        /// </summary>
        public ArmyDirection Direction { get; set; }
        /// <summary>
        /// Filas de la armada enemiga
        /// </summary>
        public int Rows { get; private set; }
        /// <summary>
        /// Columnas de la armada enemiga
        /// </summary>
        public int Columns { get; private set; }
        /// <summary>
        /// Naves enemigas
        /// </summary>
        private Enemy[,] Enemies { get; set; }
        /// <summary>
        /// linea frontal de naves enemigas
        /// </summary>
        private List<Enemy> FrontLine { get; set; }
        #endregion

        #region Indices
        public Enemy this[int row, int column]
        {
            get => Enemies[row, column];
            set
            {
                if (value == null) // estoy eliminando una nave de la grilla
                {
                    var currentValue = Enemies[row, column];
                    if (currentValue != null)
                    {
                        int frontLineIndex = this.FrontLine.IndexOf(currentValue);
                        if (frontLineIndex >= 0) // la nave eliminada pertenecia a la linea frontal
                        {
                            this.FrontLine.Remove(currentValue);
                            for (int r = currentValue.Row + 1; r < this.Rows; r++)
                                if (this[r, currentValue.Column] != null)
                                {
                                    this.FrontLine.Add(this[r, currentValue.Column]); // busco la siguiente nave disponible en la columna
                                    this.FrontLine = this.FrontLine.OrderBy(obj => obj.Column).ToList();
                                    break;
                                }
                        }
                    }
                }

                Enemies[row, column] = value;
            }
        }
        public Enemy this[Enemy enemy]
        {
            get => this[enemy.Row, enemy.Column];
            set => this[enemy.Row, enemy.Column] = value;
        }
        #endregion

        #region Constructor
        public EnemyArmy()
        {
            this.Rows = 4;
            this.Columns = 10;
            this.Enemies = new Enemy[this.Rows, this.Columns];
            this.Direction = ArmyDirection.Right;
        }
        #endregion

        /// <summary>
        /// Obtiene la nave ubicada a la izquierda de la nave indicada
        /// </summary>
        /// <param name="enemy">nave origen</param>
        /// <returns></returns>
        public Enemy Get_Left(Enemy enemy)
        {
            return enemy.Column > 0 ? this[enemy.Row, enemy.Column - 1] : null;
        }
        /// <summary>
        /// Obtiene la nave ubicada a la derecha de la nave indicada
        /// </summary>
        /// <param name="enemy">nave origen</param>
        /// <returns></returns>
        public Enemy Get_Right(Enemy enemy)
        {
            return enemy.Column < this.Columns - 1 ? this[enemy.Row, enemy.Column + 1] : null;
        }
        /// <summary>
        /// Obtiene la nave ubicada arriba de la nave indicada
        /// </summary>
        /// <param name="enemy">nave origen</param>
        /// <returns></returns>
        public Enemy Get_Top(Enemy enemy)
        {
            return enemy.Row < this.Rows - 1 ? this[enemy.Row + 1, enemy.Column] : null;
        }
        /// <summary>
        /// Obtiene la nave ubicada abajo de la nave indicada
        /// </summary>
        /// <param name="enemy">nave origen</param>
        /// <returns></returns>
        public Enemy Get_Bottom(Enemy enemy)
        {
            return enemy.Row > 0 ? this[enemy.Row - 1, enemy.Column] : null;
        }
        /// <summary>
        /// inicializa la linea frontal de enemigos con la primera fila de la grilla
        /// </summary>
        public void Init_FronLine()
        {
            this.FrontLine = new List<Enemy>();
            for (int col = 0; col < this.Columns; col++)
                this.FrontLine.Add(this[0, col]);
        }
        /// <summary>
        /// Obtiene un enemigo al azar de la lista de enemigos que pueden disparar
        /// </summary>
        /// <returns></returns>
        public Enemy Get_RandomFronLineEnemy()
        {
            if (this.FrontLine.Any())
                return this.FrontLine[Random.Range(0, this.FrontLine.Count)];
            else
                return null;
        }
        /// <summary>
        /// Mueve el grupo de naves enemigas
        /// </summary>
        public void Move()
        {
            float displacement = 1f;

            Vector3 translation = Vector3.zero;
            float x = this.Direction == ArmyDirection.Right ? displacement : -displacement;

            if (Can_Move(x))
                translation = Vector3.right * x;
            else
            {
                this.Direction = this.Direction == ArmyDirection.Right ? ArmyDirection.Left : ArmyDirection.Right;

                bool verticalLimit = this.FrontLine.Any(obj => obj.transform.position.y <= -10);
                if (!verticalLimit)
                    translation = Vector3.down * displacement;
            }

            for (int row = 0; row < this.Rows; row++)
                for (int col = 0; col < this.Columns; col++)
                {
                    Enemy enemy = this[row, col];
                    if (enemy != null)
                        enemy.transform.Translate(translation);
                }
        }
        /// <summary>
        /// Determina si la naves pueden moverse
        /// </summary>
        /// <returns></returns>
        private bool Can_Move(float x)
        {
            if (this.FrontLine.Any())
                return (this.Direction == ArmyDirection.Right ? this.FrontLine.Last() : this.FrontLine.First()).Can_Move(x);
            else
                return false;
        }
    }
    #endregion
}
