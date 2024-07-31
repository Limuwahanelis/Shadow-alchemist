using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    //[SceneName] public string sceneToLoad;
    [SerializeField] LoadScene _sceneLoader;
    [SerializeField] RectTransform _transitionCircleTransform;
   // [SerializeField] SceneTransitionManager.TransitionTags _transitionTag;
    //[SerializeField] Transform _playerSpawnPos;
   // [SerializeField] GameObject _player;
    [SerializeField] InputActionAsset _playerControls;
    [SerializeField] bool _startOnLoad;
    Animator _anim;
    //[SerializeField] InputActionAsset _menuControls;
    private void Start()
    {
        _anim = _transitionCircleTransform.GetComponent<Animator>();
        if (_startOnLoad)
        {
            //if (SceneTransitionManager.tagToTeleportPlayer == _transitionTag)
            //{

            _anim.SetTrigger("FadeIn");
            //_player.transform.position = _playerSpawnPos.position;
            _playerControls.Enable();
        }
            //SceneTransitionManager.tagToTeleportPlayer = SceneTransitionManager.TransitionTags.NONE;
            // _menuControls.Enable();
        //}
    }

    public void Load()
    {
        _playerControls.Disable();
        //_menuControls.Disable();
        StartCoroutine(TransitionCor());
    }
    IEnumerator TransitionCor()
    {
        _anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.7f);
        //SceneTransitionManager.tagToTeleportPlayer = _transitionTag;
        _sceneLoader.LoadSceneWithIndex(1);

    }
}
