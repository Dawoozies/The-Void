using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using GameData.StateData;
using Geometry;
using UnityEditor;
namespace GameData.StateData.VisualElements
{
    public class CircleCollider2DStateDataVisualElement : VisualElement
    {
        private CircleCollider2DStateData circleColliderData;
        private VisualElement circlesContainer;
        public CircleCollider2DStateDataVisualElement(CircleCollider2DStateData data)
        {
            circleColliderData = data;

            // Create a field for the dataId property
            var dataIdField = new TextField("Data ID");
            dataIdField.value = circleColliderData.dataId;
            dataIdField.RegisterValueChangedCallback(evt => circleColliderData.dataId = evt.newValue);
            Add(dataIdField);

            // Create a field for the isTrigger property
            var isTriggerToggle = new Toggle("Is Trigger");
            isTriggerToggle.value = circleColliderData.isTrigger;
            isTriggerToggle.RegisterValueChangedCallback(evt => circleColliderData.isTrigger = evt.newValue);
            Add(isTriggerToggle);

            // Create a field for the layer property
            var layerField = new IntegerField("Layer");
            layerField.value = circleColliderData.layer;
            layerField.RegisterValueChangedCallback(evt => circleColliderData.layer = evt.newValue);
            Add(layerField);

            // Create a container for the list of circles
            circlesContainer = new VisualElement();
            Add(circlesContainer);

            // Add the existing circles to the container
            foreach (var circle in circleColliderData.circles)
            {
                AddCircle(circle);
            }

            // Create a button to add a new Circle to the circles list
            var addCircleButton = new Button(() =>
            {
                var newCircle = new Circle();
                circleColliderData.circles.Add(newCircle);
                AddCircle(newCircle);
            });
            addCircleButton.text = "Add Circle";
            Add(addCircleButton);
        }

        private void AddCircle(Circle circle)
        {
            // Create a container for the circle's fields
            var circleContainer = new VisualElement();

            // Create a field for the radius property
            var radiusField = new FloatField("Radius");
            radiusField.value = circle.radius;
            radiusField.RegisterValueChangedCallback(evt => circle.radius = evt.newValue);
            circleContainer.Add(radiusField);

            // Add the circle's container to the list of circles
            circlesContainer.Add(circleContainer);

            // Register a callback for drawing the Handles for the center property
            circleContainer.RegisterCallback<CustomStyleResolvedEvent>(evt =>
            {
                Handles.DrawWireDisc(circle.center, Vector3.forward, circle.radius);
                EditorGUI.BeginChangeCheck();
                var newCenter = Handles.PositionHandle(circle.center, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    circle.center = newCenter;
                }
            });
        }
    }
}