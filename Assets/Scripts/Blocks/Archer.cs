using System;
using System.Collections;
using System.Collections.Generic;
using Levels;
using UnityEngine;

namespace Blocks
{
    public class Archer : MonoBehaviour, ILevelBlock
    {
        public GameObject arrow;
        public GameObject redCircle;
        public float shotCooldown;

        private Camera myCamera;
        private GameObject myTarget;
        private bool isShooting;
        
        [SerializeField] private float rotationSpeed = 20;
        [SerializeField] private float slowedRotationSpeed = 20;

        private float myRotationSpeed;
        private LineRenderer lineRenderer;

        private void Awake() {
            myRotationSpeed = rotationSpeed;
        }

        void Start()
        {
            myCamera = Camera.main;
            lineRenderer = GetComponent<LineRenderer>();
        }
        
        void Update()
        {
            if (GameSettings.currentLevel == null)
            {
                return;
            }

            if (GameSettings.currentLevel.isGameEnd)
            {
                return;
            }
            
            if (GameSettings.currentLevel.arrows <= 0)
            {
                if (GameObject.FindWithTag("Arrow") == null)
                {
                    GameSettings.currentLevel.Lose();
                    return;
                }
            }
            
            transform.Rotate(Vector3.forward * myRotationSpeed * Time.deltaTime);
            DrawRay(transform.position, transform.right);

            if (Input.GetMouseButtonDown(0)) {
                myRotationSpeed = slowedRotationSpeed;
            }
            if (Input.GetMouseButtonUp(0)) {
                StartCoroutine(Shoot());
                myRotationSpeed = rotationSpeed;
            }
        }

        private void DrawRay(Vector3 position, Vector3 direction)
        {
            var positions = new List<Vector3>() { position };

            var hit = Physics2D.Raycast(position, direction);
            
            if (hit.collider != null)
            {
                positions.Add(hit.point);
                if (myTarget == null)
                {
                    myTarget = Instantiate(redCircle);
                }
                var point = hit.point;
                myTarget.transform.position = new Vector3(point.x, point.y, transform.position.z);
            }
            else
            {
                positions.Add(position + direction * 100f);
                if (myTarget != null) {
                    Destroy(myTarget);
                    myTarget = null;
                }
            }
            
            lineRenderer.SetPositions(positions.ToArray());
            lineRenderer.positionCount = positions.Count;
        }

        private IEnumerator Shoot()
        {
            if (GameSettings.currentLevel.arrows > 0 && !isShooting)
            {
                isShooting = true;
                Instantiate(arrow, transform.position, transform.rotation).transform.SetParent(GameSettings.currentLevel.transform);
                GameSettings.currentLevel.score += 10;
                GameSettings.currentLevel.arrows--;
                yield return new WaitForSeconds(shotCooldown);
                isShooting = false;
            }
        }

        public void SaveToLayout(LevelLayout level)
        {
            level.playerPosition = transform.position;
        }

        public bool IsDependent => false;
    }
}
