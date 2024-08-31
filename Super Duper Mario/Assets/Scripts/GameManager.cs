
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] private GameObject _groundPrefab;
    [SerializeField] private GameObject _BackgroundPrefab;
    
    private CameraFollow _cameraFollow;
    private PlayerController _playerController;
    

    private void Awake()
    {
        Instantiate(_BackgroundPrefab, new Vector3(-3.9f, -4.5f, 0), Quaternion.identity);
        GameObject ground = Instantiate(_groundPrefab, new Vector3(0, -4.7f, 0), Quaternion.identity);
        ground.transform.localScale = new Vector3(60, 1, 1); 
        Material groundMaterial = new Material(Shader.Find("Unlit/Transparent"));
        groundMaterial.mainTexture = _groundPrefab.GetComponent<SpriteRenderer>().sprite.texture;
        groundMaterial.mainTextureScale = new Vector2(60, 1); 
        ground.GetComponent<SpriteRenderer>().material = groundMaterial;
        
        _cameraFollow = Camera.main.GetComponent<CameraFollow>();
        _cameraFollow.LeftBound = GameObject.Find("Left Bounds").transform;
        _cameraFollow.RightBound = GameObject.Find("Right Bounds").transform;
        
        _playerController = PlayerController.Instance;
        _playerController.SetCameraFollow(_cameraFollow);
    }
    
  
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
