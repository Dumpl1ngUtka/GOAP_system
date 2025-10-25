using UnityEngine;

namespace EditorGOAP
{
    public class Node
    {
        public Rect rect;
        public string title;
        public bool isDragged;
        public bool isSelected;

        public GUIStyle style;
        public GUIStyle defaultNodeStyle;
        public GUIStyle selectedNodeStyle;

        public Node(Vector2 position, float width, float height, 
            GUIStyle nodeStyle, GUIStyle selectedStyle)
        {
            rect = new Rect(position.x, position.y, width, height);
            style = nodeStyle;
            defaultNodeStyle = nodeStyle;
            selectedNodeStyle = selectedStyle;
        }

        public void Drag(Vector2 delta)
        {
            rect.position += delta;
        }

        public void Draw()
        {
            GUI.Box(rect, title, style);
        }

        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (rect.Contains(e.mousePosition))
                        {
                            isDragged = true;
                            isSelected = true;
                            style = selectedNodeStyle;
                            e.Use();
                            return true;
                        }
                    }
                    break;

                case EventType.MouseUp:
                    isDragged = false;
                    if (isSelected)
                    {
                        e.Use();
                        return true;
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }

            return false;
        }
    }
}