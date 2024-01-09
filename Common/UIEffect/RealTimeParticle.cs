using UnityEngine;

public class RealTimeParticle : MonoBehaviour
{
    private ParticleSystem _particle;
    private float _deltaTime;
    private float _timeAtLastFrame;


    void Start()
    {
        _particle = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (_particle == null) return;
        _deltaTime = Time.realtimeSinceStartup - _timeAtLastFrame;
        _timeAtLastFrame = Time.realtimeSinceStartup;

        //若Time.timeScale等于0，说明游戏暂停，则让特效每帧播放，而不受其影响
        if (Mathf.Abs(Time.timeScale) == 0)
        {
            _particle.Simulate(_deltaTime, false, false);
            _particle.Play();
        }
    }

}


