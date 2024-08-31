using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] private GameObject _groundPrefab;
    [SerializeField] private GameObject _BackgroundPrefab;

    private void Awake()
    {
        GameObject background = Instantiate(_BackgroundPrefab, new Vector3(0, -4.5f, 0), Quaternion.identity);
        GameObject ground = Instantiate(_groundPrefab, new Vector3(0, -4.7f, 0), Quaternion.identity);
        ground.transform.localScale = new Vector3(30, 1, 1); // Scale width to 10 units, height to 1 unit
        Material groundMaterial = new Material(Shader.Find("Unlit/Transparent"));
        groundMaterial.mainTexture = _groundPrefab.GetComponent<SpriteRenderer>().sprite.texture;
        groundMaterial.mainTextureScale = new Vector2(30, 1); // Tiling X = 10, Y = 1
        ground.GetComponent<SpriteRenderer>().material = groundMaterial;

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
