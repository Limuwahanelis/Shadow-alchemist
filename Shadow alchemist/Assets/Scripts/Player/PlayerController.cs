using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Debug"), SerializeField] bool _printState;
    public bool IsAlive => _isAlive;
    public PlayerState CurrentPlayerState => _currentPlayerState;
    public GameObject MainBody=>_mainBody;
    [Header("Player"),SerializeField] Animator _anim;
    [SerializeField] GameObject _mainBody;
    [SerializeField] AnimationManager _playerAnimationManager;
    [SerializeField] PlayerMovement _playerMovement;
    [SerializeField] PlayerChecks _playerChecks;
    [SerializeField] PlayerCombat _playerCombat;
    [SerializeField] PlayerCollisions _playerCollisions;
    [SerializeField] PlayerDodge _playerDodge;
    [SerializeField] PlayerHealthSystem _playerHealthSystem;
    [SerializeField] PlayerShadowsInteractions _playerShadowsInteractions;
    [SerializeField] PlacableShadowSelection _placableShadowSelection;
    [SerializeField] ShadowControlModeSelectionUI _shadowControlModeSelectionUI;
    private PlayerState _currentPlayerState;
    private PlayerContext _context;
    private Dictionary<Type, PlayerState> playerStates = new Dictionary<Type, PlayerState>();
    private bool _isAlive=true;
    void Start()
    {
        _playerHealthSystem.OnPushed += PushPlayer;
        List<Type> states = AppDomain.CurrentDomain.GetAssemblies().SelectMany(domainAssembly => domainAssembly.GetTypes())
            .Where(type => typeof(PlayerState).IsAssignableFrom(type) && !type.IsAbstract).ToArray().ToList();

        _context = new PlayerContext
        {
            ChangePlayerState = ChangeState,
            animationManager = _playerAnimationManager,
            playerMovement = _playerMovement,
            WaitAndPerformFunction = WaitAndExecuteFunction,
            WaitFrameAndPerformFunction = WaitFrameAndExecuteFunction,
            coroutineHolder = this,
            shadowControlModeSelectionUI = _shadowControlModeSelectionUI,
            checks = _playerChecks,
            placableShadowSelection = _placableShadowSelection,
            combat = _playerCombat,
            canPerformAirCombo = true,
            shadowControl = _playerShadowsInteractions,
            collisions = _playerCollisions,
            playerDodge = _playerDodge,
        };
        PlayerState.GetState getState = GetState;
        foreach (Type state in states)
        {
            playerStates.Add(state, (PlayerState)Activator.CreateInstance(state, getState));
        }
        PlayerState newState = GetState(typeof(PlayerIdleState));
        newState.SetUpState(_context);
        _currentPlayerState = newState;
    }

    public PlayerState GetState(Type state)
    {
        return playerStates[state];
    }
    void Update()
    {
        _currentPlayerState.Update();
    }
    private void FixedUpdate()
    {
        _currentPlayerState.FixedUpdate();
    }
    public void ChangeState(PlayerState newState)
    {
        if (_printState) Logger.Log(newState.GetType());
        _currentPlayerState.InterruptState();
        _currentPlayerState = newState;
    }
    public void PushPlayer(Vector3 pushDIrection, IDamager pusher)
    {
        _playerMovement.PushPlayer(pushDIrection, pusher);
        _currentPlayerState.Push();
    }
    public Coroutine WaitAndExecuteFunction(float timeToWait, Action function)
    {
        return StartCoroutine(DelyedFunction( timeToWait, function));
    }
    public Coroutine WaitFrameAndExecuteFunction(Action function)
    {
        return StartCoroutine( WaitFrame(function));
    }
    public IEnumerator DelyedFunction(float timeToWait, Action function)
    {
        yield return new WaitForSeconds(timeToWait);
        function();
    }
    public IEnumerator WaitFrame(Action function)
    {
        yield return new WaitForNextFrameUnit();
        function();
    }

    private void OnDestroy()
    {
        _playerHealthSystem.OnPushed -= PushPlayer;
    }
}
