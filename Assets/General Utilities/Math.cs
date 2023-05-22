using UnityEngine;
using System;
using UnityEditor;
using OLD.GameData.EditorWindow;
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
namespace OLD.Geometry
{
    [Serializable]
    public class Circle : SceneGUI
    {
        public Vector3 center;
        public float radius;
        public Circle ()
        {
            center = Vector3.zero;
            radius = 0f;
        }
        public Circle(Vector3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }
        public Circle Copy()
        {
            return new Circle(center, radius);
        }
        public void OnSceneGUI(SceneView sceneView)
        {
            Handles.DrawSolidDisc(center, Vector3.forward, radius);
            Handles.DrawDottedLine(center, center + Vector3.right*radius, 3f);
            EditorGUI.BeginChangeCheck();
            Vector3 oldArrowPosition = center + Vector3.right * radius;
            Vector3 newArrowPosition = Handles.Slider(oldArrowPosition, Vector3.right, 0.75f, Handles.ArrowHandleCap, 0.1f);
            Vector3 oldSquarePosition = center;
            Vector3 newSquarePosition = Handles.FreeMoveHandle(oldSquarePosition, Quaternion.identity, 0.35f, Vector3.one * 0.1f, Handles.RectangleHandleCap);
            if(EditorGUI.EndChangeCheck())
            {
                radius += newArrowPosition.x - oldArrowPosition.x;
                center += newSquarePosition - oldSquarePosition;
            }
        }
        public void SubscribeGUI()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }
        public void UnsubscribeGUI()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            Debug.Log("Is this happening?");
        }
    }
    [Serializable]
    public class Box
    {
        public Vector2 center;
        public Vector2 size;
        public float angle;
        public Box()
        {
            center = Vector2.zero;
            size = Vector2.zero;
            angle = 0f;
        }
        public Box(Vector2 center, Vector2 size, float angle)
        {
            this.center = center;
            this.size = size;
            this.angle = angle;
        }
        public Box Copy()
        {
            return new Box(center, size, angle);
        }
        public void Paste(Box box)
        {
            this.center = box.center;
            this.size = box.size;
            this.angle = box.angle;
        }
    }
    [Serializable]
    public class Area
    {
        public Vector2 pointA;
        public Vector2 pointB;
        public Area()
        {
            pointA = Vector2.zero;
            pointB = Vector2.zero;
        }
        public Area(Vector2 pointA, Vector2 pointB)
        {
            this.pointA = pointA;
            this.pointB = pointB;
        }
        public Area Copy()
        {
            return new Area(pointA, pointB);
        }
        public void Paste(Area area)
        {
            this.pointA = area.pointA;
            this.pointB = area.pointB;
        }
    }
}