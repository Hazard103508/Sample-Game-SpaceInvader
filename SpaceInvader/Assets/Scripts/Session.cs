using System;
using UnityEngine;

public class Session
{
    private static int _lives;
    private static int _score;
    private static int _enemies;

    public static event Action LifeChanged;
    public static event Action ScoreChanged;
    public static event Action EnemiesChanged;

    /// <summary>
    /// Cantidad de vidas disponibles del jugador
    /// </summary>
    public static int Lives
    {
        get => _lives;
        set
        {
            _lives = value;
            if (LifeChanged != null)
                LifeChanged.Invoke();
        }
    }

    /// <summary>
    /// puntaje total del jugador
    /// </summary>
    public static int Score
    {
        get => _score;
        set
        {
            _score = value;
            if (ScoreChanged != null)
                ScoreChanged.Invoke();
        }
    }

    /// <summary>
    /// puntaje total del jugador
    /// </summary>
    public static int Enemies
    {
        get => _enemies;
        set
        {
            _enemies = value;
            if (EnemiesChanged != null)
                EnemiesChanged.Invoke();
        }
    }

    /// <summary>
    /// Intervalo en segundos entre los movientos de las naves
    /// </summary>
    public static float ArmyMoveTime { get; set; }
    public static int Level { get; set; }
}
