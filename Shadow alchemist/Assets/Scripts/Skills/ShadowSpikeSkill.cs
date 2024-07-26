using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShadowSpikeSkill : MonoBehaviour
{
    [SerializeField] Transform _startTrans;
    [SerializeField] GameObject _spikePrefab;
    [SerializeField] AnimationManager _spikeAnimMan;
    [SerializeField] float _distanceBetweenSpikes;
    [SerializeField] ControllableShadow _shadowToSpawnSpikes;
    List<SpikeAttackLogic> _attackLogicList= new List<SpikeAttackLogic>();
    List<AnimationManager> anims=new List<AnimationManager>();
    float _spikeAnimLength;
    float _time;
    private void Start()
    {
        _spikeAnimLength = _spikeAnimMan.GetAnimationLength("Spike");
        StartCoroutine(SpikeSpawnCor());
    }
    private void Update()
    {

    }

    IEnumerator SpikeSpawnCor()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 spawnPos = _startTrans.position;
            spawnPos.x -= i * _distanceBetweenSpikes;
            _attackLogicList.Add(Instantiate(_spikePrefab, spawnPos, _spikePrefab.transform.rotation, null).GetComponent<SpikeAttackLogic>());
            _attackLogicList[_attackLogicList.Count - 1].StartAttackCor();
            if(i>=2)
            {
                _attackLogicList[i - 2].GetComponent<AnimationManager>().PlayAnimation("Reverse spike");
                _attackLogicList[i - 2].StopAttckCor();
                
            }
            while (_time < _spikeAnimLength)
            {
                _time += Time.deltaTime;
                yield return null;
            }
            _time = 0;
            Vector3 tmp = spawnPos;
            tmp.x -= 1.5f;
            if (!_shadowToSpawnSpikes.ShadowBounds.Contains(tmp)) yield break;

        }
    }
}
