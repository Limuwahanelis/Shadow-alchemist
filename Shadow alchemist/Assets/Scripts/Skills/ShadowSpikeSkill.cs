using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShadowSpikeSkill : MonoBehaviour
{
    [SerializeField] Transform _startTrans;
    [SerializeField] GameObject _spikePrefab;
    [SerializeField] AnimationDataReader _spikeAnimMan;
    [SerializeField] float _distanceBetweenSpikes;
    [SerializeField] ControllableShadow _shadowToSpawnSpikes;
    [SerializeField] float _firstSpikeDelay;
    //List<SpikeAttackLogic> _attackLogicList= new List<SpikeAttackLogic>();
    List<AnimationManager> anims=new List<AnimationManager>();
    private Coroutine _castSpikeCor;
    float _spikeAnimLength;
    float _time;
    private void Start()
    {
        _spikeAnimLength = _spikeAnimMan.GetAnimationLength("Spike");

    }
    private void Update()
    {

    }
    public void SetOriginShadow(ControllableShadow _shadow)
    {
        _shadowToSpawnSpikes = _shadow;
    }
    public void CastSpikes()
    {
        _castSpikeCor=StartCoroutine(SpikeSpawnCor());
    }
    public void StopCastingSpikes()
    {
        if(_castSpikeCor!=null)
        {
            //_castSpikeCor
            StopCoroutine(_castSpikeCor);
            //DespawnRemainingSpikes();
            _castSpikeCor = null;
        }
    }
    //public void DespawnRemainingSpikes()
    //{
    //    for(int i=0;i< _attackLogicList.Count;i++)
    //    {
    //        StartCoroutine(DespawnSpikeCor(_attackLogicList[i]));
    //    }
    //}
    IEnumerator DespawnSpikeCor(SpikeAttackLogic spikeLogic)
    {
        yield return new WaitForSeconds(_spikeAnimLength);
        Destroy(spikeLogic.gameObject);
    }
    IEnumerator SpikeSpawnCor()
    {
        yield return new WaitForSeconds(_firstSpikeDelay);
        //List<SpikeAttackLogic> _attackLogicList;
        for (int i = 0; i < 30; i++)
        {

            while (_time < _spikeAnimLength)
            {
                _time += Time.deltaTime;
                yield return null;
            }
            Vector3 spawnPos = _startTrans.position;
            spawnPos.x -= i * _distanceBetweenSpikes;
            SpikeAttackLogic spike = Instantiate(_spikePrefab, spawnPos, _spikePrefab.transform.rotation, null).GetComponent<SpikeAttackLogic>();
            spike.SetUp(2 * _spikeAnimMan.GetAnimationLength("Spike"));
            _time = 0;
            Vector3 tmp = spawnPos;
            tmp.x -= 1.5f;
            if (!_shadowToSpawnSpikes.ShadowBounds.Contains(tmp)) yield break;

        }
    }

}
