using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DOTS_Test
{
    public class TestObjectsSpawner : MonoBehaviour
    {
        [SerializeField] private float _spawnFieldWidth = 15;
        [SerializeField] private int _maxObjectsInLine = 15;
        [SerializeField] private int _objectsCount = 25;
        [SerializeField] private GameObject _testObjectReference;

        private Vector3 _startPoint;
        private float _step;

        // Start is called before the first frame update
        void Start()
        {
            CalculateStartPoint();
            SpawnObjects();
        }

        void CalculateStartPoint()
        {
            _startPoint = transform.position;

            _startPoint.x -= _spawnFieldWidth / 2;
            _startPoint.z += _spawnFieldWidth / 2;

            _step = _spawnFieldWidth / _maxObjectsInLine;
        }

        void SpawnObjects()
        {
            for (int i = 0; i < _objectsCount; i++)
            {
                var dist = i * _step;
                var pos = _startPoint;

                pos.x += dist % _spawnFieldWidth;
                pos.z -= (int) (dist / _spawnFieldWidth);

                Instantiate(_testObjectReference, pos, Quaternion.identity);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            var startPoint = new Vector3();
            var endPoint = new Vector3();

            startPoint = endPoint = transform.position;

            startPoint.x -= _spawnFieldWidth / 2;
            startPoint.z += _spawnFieldWidth / 2;

            endPoint.x += _spawnFieldWidth / 2;
            endPoint.z += _spawnFieldWidth / 2;

            Gizmos.DrawLine(startPoint, endPoint);
        }
    }
}
