using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

public class CreateAsteroid : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private Asteroid[] _asteroids;
    [SerializeField] private float _lifeDuration = 15.0f;
    [SerializeField] private float _spawnInterval = 2.0f;
    private List<Asteroid> _asteroidsList = new List<Asteroid>();

    private CancellationTokenSource cancellationToken;

    private async void Start()
    {
        cancellationToken = new CancellationTokenSource();
        await SpawnAsteroids().SuppressCancellationThrow();
    }

    private async UniTask SpawnAsteroids()
    {
        int numOfObjects = Mathf.CeilToInt(_lifeDuration / _spawnInterval);
        for (int i = 0; i < numOfObjects; i++)
        {
            Vector3 spawnPosition = GetRandomPositionInCameraView();
            Quaternion spawnRotation = Quaternion.Euler(RandomRotation(), RandomRotation(), RandomRotation());
            var newAsteroid = Instantiate(_asteroids[Random.Range(0, _asteroids.Length)].gameObject, spawnPosition, spawnRotation);
            newAsteroid.SetActive(false);
            var asteroidObj = newAsteroid.GetComponent<Asteroid>();
            _asteroidsList.Add(asteroidObj);
        }
        while (true)
        {
            foreach (var asteroid in _asteroidsList)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    Vector3 randomPosition = GetRandomPositionInCameraView();
                    asteroid.transform.position = randomPosition;
                    asteroid.TurnOnAsteroid(_lifeDuration, cancellationToken).Forget();
                    await UniTask.Delay((int)(_spawnInterval * 1000), cancellationToken:cancellationToken.Token);
                }
                else
                {
                    return;
                }

            }
        }
    }

    Vector3 GetRandomPositionInCameraView()
    {
        float randomX = Random.Range(0.0f, 1.0f);
        float randomY = Random.Range(0.0f, 1.0f);
        float randomZ = _cam.nearClipPlane * 2 + Random.Range(0.0f, _cam.farClipPlane / 6 - _cam.nearClipPlane * 2);
        Vector3 viewportPoint = new Vector3(randomX, randomY, randomZ);
        Vector3 worldPosition = _cam.ViewportToWorldPoint(viewportPoint);
        return worldPosition;
    }

    private int RandomRotation()
    {
        return Random.Range(0, 360);
    }

    private void OnApplicationQuit()
    {  
        if (cancellationToken != null)
        {
            cancellationToken.Cancel();
        }
    }
}