using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField]
    private GridManager _gridManager;
    public GridManager GridManager { get => _gridManager; }
    [SerializeField]
    private Pathfinding _pathfinding;
    public Pathfinding Pathfinding { get => _pathfinding; }

    [SerializeField]
    private TurnManager _turnManager;
    public TurnManager TurnManager { get => _turnManager; }

    [SerializeField]
    private HUD _hud;
    public HUD HUD { get => _hud; }

    #region Singleton
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion

    public void InitGame()
    {
        //init grid

        //instantiate ally robot

        //instantiate enemy robot

        //select first platyers and activate actions HUI

        //UpdateVisibleTiles

        //start record player's robots actions
    }
}
