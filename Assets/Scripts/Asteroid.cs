using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[RequireComponent(typeof(Rigidbody))]
public class Asteroid : MonoBehaviour
{
    [SerializeField] private float _minMoveSpeed = .1f;
    [SerializeField] private float _maxMoveSpeed = .1f;
    [SerializeField] private float _minRotSpeed = 1f;
    [SerializeField] private float _maxRotSpeed = 8f;
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rigidbody;

    private Vector3 _randomDirection;
    private float _speed;
    private float _tumble;
    private float _elapsedTime = 0.0f;

    public async UniTask TurnOnAsteroid(float lifeTime, CancellationTokenSource cancelAsteroid)
    {
        InitAsteroid();
        _elapsedTime = 0.0f;
        await Astrolife(lifeTime, cancelAsteroid);
    }

    void InitAsteroid()
    {
        _randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;

        _speed = Random.Range(_minMoveSpeed, _maxMoveSpeed);
        _tumble = Random.Range(_minRotSpeed, _maxRotSpeed);

        _rigidbody.angularVelocity = Random.insideUnitSphere * _tumble;
    }

    private async UniTask Astrolife(float lifeTime, CancellationTokenSource cancelAsteroid)
    {
        gameObject.SetActive(true);
        _animator.Play("Grow", 0, 0f);
        _animator.SetBool("IsActive", true);
        _rigidbody.velocity = _randomDirection * _speed;

        while (_elapsedTime < lifeTime && !cancelAsteroid.IsCancellationRequested)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && _animator.GetCurrentAnimatorStateInfo(0).IsName("Grow"))
            {
                await Pause(lifeTime - 2f); 
                _animator.Play("Disperse", 0, 0f);
            }

            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && _animator.GetCurrentAnimatorStateInfo(0).IsName("Disperse"))
            {
                _animator.SetBool("IsActive", false);
                gameObject.SetActive(false);
                break;
            }

            await UniTask.Yield(cancellationToken: cancelAsteroid.Token);

            if (!cancelAsteroid.IsCancellationRequested)
            {
                _elapsedTime += Time.deltaTime;
            }
        }

        if (!cancelAsteroid.IsCancellationRequested)
        {
            _animator.SetBool("IsActive", false);
            await UniTask.Yield(cancellationToken: cancelAsteroid.Token);
            gameObject.SetActive(false);
        }
    }

    private async UniTask Pause(float duration)
    {
        await UniTask.Delay((int)(duration * 1000));
    }

    private async UniTask PauseState(float pauseDuration)
    {
        _animator.SetBool("IsPaused", true);
        await UniTask.Delay((int)(pauseDuration * 1000));
        _animator.SetBool("IsPaused", false);
    }
}