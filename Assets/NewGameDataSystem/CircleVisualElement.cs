using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Geometry;
public class CircleVisualElement : VisualElement
{
    public Circle circle;
    public CircleVisualElement(Circle _circle)
    {
        circle = _circle;
        var centerField = new Vector3Field("Center") { value = circle.center };
        var radiusField = new FloatField("Radius");
        Add(centerField);
        Add(radiusField);
        centerField.RegisterValueChangedCallback(evt => circle.center = evt.newValue);
        radiusField.RegisterValueChangedCallback(evt => circle.radius = evt.newValue);
    }
}