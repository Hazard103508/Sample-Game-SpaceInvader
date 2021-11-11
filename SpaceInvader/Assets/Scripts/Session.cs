using System;
using UnityEngine;

public class Session
{
    private static int _lives;
    public static event Action LifeChanged;

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
}
