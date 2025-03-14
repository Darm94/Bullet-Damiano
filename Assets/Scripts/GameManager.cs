using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Turrets
    [SerializeField] private GameObject[] turretsPrefab; // CHANGED TO PRIVATE
    [SerializeField] private int numberOfTurrets = 5; // CHANGED TO PRIVATE
    private GameObject[] _turrets;

    [SerializeField] [Range(0.1f, 50f)] private float minDistanceX = 5;
    [SerializeField] [Range(0.1f, 50f)] private float minDistanceZ = 5;

    [SerializeField] [Range(1f, 100f)] private float deltaX = 15;
    [SerializeField] [Range(1f, 100f)] private float deltaZ = 15;

    [SerializeField] [Range(0.05f, 5f)] private float minFireRate = 0.5f; // "*0.5"
    [SerializeField] [Range(0.05f, 5f)] private float maxFireRate = 2f;

    [SerializeField] [Range(1f, 50f)] private float minFireDistance = 10f;
    [SerializeField] [Range(1f, 100f)] private float maxFireDistance = 20f;
    #endregion

    #region Walls
    private int _wallsAvailable = 0;

    [SerializeField] private GameObject[] wallsPrefab; // Serializzabile

    [SerializeField] [Range(0.1f, 50f)] private float minDistanceWallX = 1;
    [SerializeField] [Range(0.1f, 50f)] private float minDistanceWallZ = 5;

    [SerializeField] [Range(1f, 50f)] private float deltaWallX = 1;
    [SerializeField] [Range(1f, 100f)] private float deltaWallZ = 1;
    #endregion

    [SerializeField] GameObject gameOver; // Game Over Text (TMP)
    [SerializeField] GameObject youWin; // W
                                        // in text

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (numberOfTurrets == 0)
        {
            Debug.LogWarning("No number of turrets detected");
            return;
        }

        _wallsAvailable = numberOfTurrets;
        _turrets = new GameObject[numberOfTurrets];

        for (int i = 0; i < numberOfTurrets; i++)
        {
            _turrets[i] = Instantiate(turretsPrefab[Random.Range(0, turretsPrefab.Length)]); //istantiante randomply from classical prefab array
            
            int tries = 5;
            bool intersect = false;

            do
            {
                _turrets[i].transform.position = new Vector3(
                    minDistanceX * Random.Range(-1f, 1f) + deltaX, 
                    0, 
                    minDistanceZ * Random.Range(-1f, 1f) + deltaZ);

                _turrets[i].transform.Rotate(Vector3.up, Random.Range(0f, 360f), Space.World);

                foreach (var addedTurret in _turrets)
                {
                    if (addedTurret == _turrets[i] || addedTurret == null) continue;

                    if (addedTurret.GetComponent<CapsuleCollider>().bounds.Intersects(
                            _turrets[i].GetComponent<CapsuleCollider>().bounds))
                    {
                        intersect = true;
                        break;
                    }
                }

                tries--;
            } while (intersect && tries > 0);

            FireBulletsAtTarget turretScript = _turrets[i].GetComponent<FireBulletsAtTarget>();
            turretScript.Configure(
                Random.Range(minFireRate, maxFireRate), //fireRate
                Random.Range(minFireDistance, maxFireDistance),//fire Distance 
                transform);

            // Place wall around turret
            GameObject wall = Instantiate(wallsPrefab[Random.Range(0, wallsPrefab.Length)]);

            wall.transform.position = new Vector3(
                minDistanceWallX * Random.Range(-1f, 1f) + deltaWallX, 
                wall.transform.localScale.y * 0.5f,  // Move up (half height -> pivot is half)
                minDistanceWallZ * Random.Range(-1f, 1f) + deltaWallZ
            );

            // Rotate around turret randomly
            wall.transform.RotateAround(
                _turrets[i].transform.position, 
                Vector3.up, 
                Random.Range(0f, 360f)
            );

            // Rotate around itself randomly locally
            wall.transform.Rotate(Vector3.up, Random.Range(-45f, 45f), Space.Self);

            // The Game mAnager to be notified (better to use delegates, actions, function etc.)
            DestroyOnMultiHit destroyOnMultipleHit = wall.GetComponent<DestroyOnMultiHit>();
            destroyOnMultipleHit.GameManager = this; // wall will call --> DidDestroyWall
        }
    }

    public void GameOver()
    {
        DestroyAllTurrets();

        Debug.Log($"Game OVER: Play time: {Time.time}");

        gameOver.SetActive(true);
    }

    public void DidDestroyWall()
    {
        _wallsAvailable--;

        if (_wallsAvailable <= 0)
        {
            DestroyAllTurrets();

            Debug.Log($"Game OVER: YOU WIN! Play time: {Time.time}");

            youWin.SetActive(true);
        }
    }

    private void DestroyAllTurrets()
    {
        foreach (var turret in _turrets)
        {
            Destroy(turret);
        }
    }
}
