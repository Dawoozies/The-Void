using UnityEngine;
namespace LinearAlgebra
{
    public class ParametrisedLine
    {
        private Vector3 _pathStart;
        private Vector3 _pathEnd;

        Vector3 _direction;
        float _parameter;
        Vector3 _point;

        public ParametrisedLine()
        {
            _pathStart = Vector3.zero;
            _pathEnd = Vector3.zero;
            _direction = Vector3.zero;
            _parameter = 0;
            _point = Vector3.zero;
        }

        public (Vector3, Vector3) path
        {
            set
            {
                _pathStart = value.Item1;
                _pathEnd = value.Item2;
                UpdateLine();
            }
        }

        public Vector3 pathStart
        {
            get => _pathStart;
            set
            {
                _pathStart = value;
                UpdateLine();
            }
        }

        public Vector3 pathEnd
        {
            get => _pathEnd;
            set
            {
                _pathEnd = value;
                UpdateLine();
            }
        }

        public Vector3 point
        {
            get => _point;
        }

        public float parameter
        {
            get => _parameter;
            set
            {
                _parameter = value;
                UpdateLine();
            }
        }

        public Vector3 direction
        {
            get => _direction;
        }

        void UpdateLine()
        {
            Update_Direction();
            Update_Point();
        }

        void Update_Direction()
        {
            _direction = (_pathEnd - _pathStart).normalized;
        }

        void Update_Point()
        {
            _point = _pathStart + parameter * (_pathEnd - _pathStart);
        }
    }
}
namespace Geometry
{
    public class Circle
    {
        public Vector3 center;
        public float radius;

        public Circle ()
        {
            center = Vector3.zero;
            radius = 0f;
        }
    }
}