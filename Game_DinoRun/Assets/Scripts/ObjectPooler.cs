using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [Header("Cài đặt Pool")]
    public GameObject[] obstaclePrefabs; //Mảng chứa prefab xương rồng
    public int amountToPool = 15;        // Số lượng tạo sẵn để trong kho

    private List<GameObject> pooledObjects;
    void Awake()
    {
        Instance = this;

        pooledObjects = new List<GameObject>();
        //Tạo sẵn các object và ẩn chúng đi
        for(int i = 0;i < amountToPool; i++)
        {
            //Random chọn 1 loại chướng ngại vật từ mảng
            int randomIndex = Random.Range(0, obstaclePrefabs.Length);
            GameObject obj = Instantiate(obstaclePrefabs[randomIndex]);

            obj.SetActive(false); // Tắt đi
            pooledObjects.Add(obj); //Cho vào kho
        }
    }

    //Hàm lấy chướng ngại vật ra khỏi kho
    public GameObject GetPooledObject()
    {
        // Duyệt kho, tìm cái nào đang rảnh (đang bị tắt) thì lôi ra
        for(int i = 0;i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null; // Nếu kho hết đồ rảnh thì trả về null
    }
}
