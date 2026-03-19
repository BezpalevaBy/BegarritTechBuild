using System.Collections.Generic;
using UnityEngine;

namespace GameMenu
{
    public class CloudSystem : MonoBehaviour
    {
        public float baseSpeed = 0.5f;
        public float randSpeed = 0.25f;
        public float baseScale = 0.2f;
        public float randScale = 0.1f;

        public float minX = -9f;
        public float maxX = 9f;
        public float maxY = 4f;
        public float minY = 1.7f;

        public float z = -2.4f;

        public float orderLayer = 1;
        
        public float chanceToSpawnCloudEverySecond = 0.05f;

        private float deltaTime = 0f;

        private GameObject firstOriginalCloud;
        private GameObject secondOriginalCloud;
        private DaySystem _daySystem;
        
        void Start()
        {
            firstOriginalCloud = GameObject.Find($"cloudOriginal1");
            secondOriginalCloud = GameObject.Find($"cloudOriginal2");
            _daySystem = GetComponent<DaySystem>();
        }
        
        void Update()
        {
            deltaTime += Time.fixedDeltaTime;
            if (deltaTime > 1)
            {
                deltaTime = 0;
                ProcessSecUpdate();
            }
        }

        private Vector3 GetStartPos()
        {
            var x = minX;
            var y = Random.Range(minY, maxY);
            return new Vector3(x, y, z);
        }

        private IEnumerator<float> MoveCloud(GameObject cloud)
        {
            var speed = baseSpeed + Random.Range(-randSpeed, randSpeed);
            
            while (cloud.transform.position.x < maxX)
            {
                yield return Time.fixedDeltaTime;

                cloud.transform.position += Vector3.right * speed/100;
            }
            
            Destroy(cloud);
        }

        void ProcessSecUpdate()
        {
            if (Random.Range(0f, 1f) < chanceToSpawnCloudEverySecond && _daySystem.alpha >= 0.5f)
            {
                var cloud = Instantiate(Random.Range(0,2) == 0 ? firstOriginalCloud : secondOriginalCloud,GetStartPos(), Quaternion.identity) as GameObject;
                
                var scale = baseScale + Random.Range(-randScale, randScale);
                cloud.transform.localScale = new Vector3(scale, scale, scale);
                
                cloud.transform.SetAsLastSibling();
                cloud.transform.SetSiblingIndex((int)orderLayer);
                
                StartCoroutine(MoveCloud(cloud));
            }
        }
    }
}
