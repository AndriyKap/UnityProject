using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClotheAdder : MonoBehaviour
{
    [SerializeField] private GameObject topPrefab;
    [SerializeField] private GameObject jacketPrefab;
    [SerializeField] private GameObject jeansPrefab;
    [SerializeField] private GameObject shoesPrefab;
    [SerializeField] private SkinnedMeshRenderer playerSkin;
    [SerializeField] private List<GameObject> _equipedClothes;
    // Start is called before the first frame update
    void Start()
    {
        _equipedClothes = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            addClothes(topPrefab);
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            addClothes(jacketPrefab);
        }

        if(Input.GetKeyDown(KeyCode.Y))
        {
            addClothes(jeansPrefab);
        }     

        if(Input.GetKeyDown(KeyCode.U))
        {
            addClothes(shoesPrefab);
        }  
    }

    public void addClothes(GameObject clothPrefab)
    {
        GameObject clothObj = Instantiate(clothPrefab, playerSkin.transform.parent);
        SkinnedMeshRenderer[] renderers = clothObj.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(SkinnedMeshRenderer renderer in renderers)
        {
            renderer.bones = playerSkin.bones;
            renderer.rootBone = playerSkin.rootBone;
        }
        _equipedClothes.Add(clothObj);
    }

    public void RemoveClothes(GameObject searchedClothObject)
    {
        foreach(GameObject clothObj in _equipedClothes)
        {
            if(clothObj.name.Contains(searchedClothObject.name))
            {
                _equipedClothes.Remove(clothObj);
                Destroy(clothObj);
                return;
            }
        }
    }
}
