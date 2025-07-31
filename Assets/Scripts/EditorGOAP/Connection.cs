using UnityEditor;
using UnityEngine;

namespace EditorGOAP
{
    public class Connection
    {
        public Node inNode;
        public Node outNode;

        public void Draw()
        {
            Vector3 startPos = new Vector3(outNode.rect.x + outNode.rect.width, 
                outNode.rect.y + outNode.rect.height / 2, 0);
            Vector3 endPos = new Vector3(inNode.rect.x, 
                inNode.rect.y + inNode.rect.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
        
            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.white, null, 2f);
        }
    }
}