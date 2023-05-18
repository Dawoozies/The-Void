using UnityEngine.UIElements;
using Geometry;
using GameData.StateData;
public class CircleCollider2DVisualElement : VisualElement
{
    private CircleCollider2DStateData circleCollider2DStateData;
    private VisualElement circlesContainer;
    public CircleCollider2DVisualElement(CircleCollider2DStateData circleCollider2DStateData)
    {
        this.circleCollider2DStateData = circleCollider2DStateData;
        circlesContainer = new VisualElement();
        Add(circlesContainer);
        var addCircleButton = new Button(() =>
        {
            var newCircle = new Circle();
            circlesContainer.Add(new CircleVisualElement(newCircle));
            circleCollider2DStateData.circles.Add(newCircle);
        });
        addCircleButton.text = "Add Circle";
        Add(addCircleButton);
        foreach (var circle in circleCollider2DStateData.circles)
        {
            CircleVisualElement circleElement = new CircleVisualElement(circle);
            circlesContainer.Add(circleElement);
            var removeCircleButton = new Button(() =>
            {
                circlesContainer.Remove(circleElement);
                circleCollider2DStateData.circles.Remove(circle);
            });
        }
    }
}