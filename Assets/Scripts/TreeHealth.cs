using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using KrisDevelopment.EnvSpawn;

public class TreeHealth : MonoBehaviour
{
    public int startHealth;
    public int health;
    public float destroyTime = 5f;
    private Transform treeSpawner;
    [SerializeField] private string spawnerName = "TreeSpawner";

    public void Start()
    {
        treeSpawner = GameObject.Find(spawnerName).transform;
        health = startHealth;
    }
    
    public void TreeFall()
    {
        gameObject.AddComponent<Rigidbody>();
        Rigidbody rig = GetComponent<Rigidbody>();
        rig.isKinematic  = false;
        rig.useGravity = true;
        rig.mass = 200;
        rig.constraints = RigidbodyConstraints.FreezeRotationY;

        RespawnTree();
        Destroy(gameObject, destroyTime);
    }

    public void RespawnTree()
    {
        float randomX, randomY;
        Vector3 rayPos;

        do
        {
            randomX = Random.Range(treeSpawner.position.x - treeSpawner.GetComponent<EnviroSpawn_CS>().dimensions.x / 2, treeSpawner.position.x + treeSpawner.GetComponent<EnviroSpawn_CS>().dimensions.x / 2);
            randomY = Random.Range(treeSpawner.position.z - treeSpawner.GetComponent<EnviroSpawn_CS>().dimensions.y / 2, treeSpawner.position.z + treeSpawner.GetComponent<EnviroSpawn_CS>().dimensions.y / 2);
            rayPos = new Vector3(randomX, 100, randomY);
        } while (!TryRespawnTree(rayPos));
    }

    private bool TryRespawnTree(Vector3 position)
    {
        RaycastHit hit;

        if (Physics.SphereCast(position, 2, Vector3.down, out hit, 200))
        {
            if (hit.collider.gameObject.layer == 8)
            {
               // Створити нове дерево
                GameObject newTree = Instantiate(gameObject, hit.point, Quaternion.identity);

                // Встановити батьківський об'єкт для новоствореного дерева
                newTree.transform.parent = treeSpawner;

                // Знищити компонент Rigidbody новоствореного дерева
                Destroy(newTree.GetComponent<Rigidbody>());

                // Зберегти початковий розташування
                newTree.transform.position = hit.point;

                // Встановити початковий оберт для новоствореного дерева
                newTree.transform.rotation = Quaternion.identity;
                return true;
            }
        }

        return false;
    }
}
