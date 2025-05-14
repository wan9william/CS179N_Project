using UnityEngine;
using System.Collections.Generic;
    struct Request {
        public int quantity;

        Request(int _q) {
            quantity = _q;
        }
    };


    public class ObjectManager : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private const int _MAXObj = 256;
        [SerializeField] private int _index = 0;
        private GameObject[] objs;
        private GameObject[] bullet_objs;
        [SerializeField] private GameObject bullet_prefab;

    private List<Request> request_queue = new List<Request>(); 

        void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void Initialize() {

            /*
            objs = new GameObject[_MAXObj];
            for (int i = 0; i < _MAXObj; ++i) {
                objs[i] = new GameObject($"obj_{i}");
                objs[i].transform.parent = transform;
                objs[i].transform.position = Vector3.zero;
            }
            */


            bullet_objs = new GameObject[20];
            for (int i = 0; i < 20; ++i)
            {
                bullet_objs[i] = Instantiate(bullet_prefab, Vector3.zero, Quaternion.identity);
                bullet_objs[i].name = $"bullet_obj_{i}";
                bullet_objs[i].transform.parent = transform;
                bullet_objs[i].transform.position = Vector3.zero;
                bullet_objs[i].SetActive(false);
            }
        }

    //THE BULLET STILL DELETES ITSELF UPON DESTRUCTION. IT SHOULD INSTEAD JUST MAKE ITSELF INACTIVE
    //MAKE THE BULLETS INACTIVE UPON START  



        public GameObject RequestBulletObj() {

            int initial_index = -1;

            while (initial_index != _index) {

            if (!bullet_objs[_index].activeInHierarchy)
            {
                bullet_objs[_index].SetActive(true);
                bullet_objs[_index].GetComponent<Bullet>().ResetTimer();
                return bullet_objs[_index];
            }

                if(initial_index == -1)initial_index = _index;

                _index = _index >= 20-1 ? 0 : _index + 1;
            }
            return null;
        }
    }
