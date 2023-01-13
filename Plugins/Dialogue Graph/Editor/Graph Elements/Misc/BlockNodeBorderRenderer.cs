using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor
{
    public class BlockNodeBorderRenderer : ImmediateModeElement
    {
        public new class UxmlFactory : UxmlFactory<BlockNodeBorderRenderer, UxmlTraits> { } // UI Builder Custom Controls Entry

        private static Mesh s_Mesh;

        private Material m_Material;

        private GraphView m_GraphView;

        public Color StartColor { get; set; }

        public Color EndColor { get; set; }

        protected override void ImmediateRepaint()
        {
            if (m_GraphView == null)
            {
                m_GraphView = GetFirstAncestorOfType<GraphView>();
                return;
            }

            if (s_Mesh == null)
            {
                s_Mesh = CreateMesh();
            }

            if (m_Material == null)
            {
                Shader shaderAsset = AssetDatabase.LoadAssetAtPath<Shader>(DialogueResourceManager.BorderShaderPath);
                m_Material = new Material(shaderAsset);
            }

            Vector4 size = new Vector4(layout.width * 0.5f, layout.height * 0.5f, 0, 0);

            m_Material.SetVector("_Size", size);

            m_Material.SetFloat("_Border", 2f / m_GraphView.scale);
            m_Material.SetFloat("_Radius", resolvedStyle.borderTopLeftRadius);

            m_Material.SetColor("_ColorStart", QualitySettings.activeColorSpace == ColorSpace.Linear ? StartColor.gamma : StartColor);
            m_Material.SetColor("_ColorEnd", QualitySettings.activeColorSpace == ColorSpace.Linear ? EndColor.gamma : EndColor);

            m_Material.SetPass(0);

            Graphics.DrawMeshNow(s_Mesh, Matrix4x4.Translate(new Vector3(size.x, size.y, 0)));
        }

        private Mesh CreateMesh()
        {
            Mesh temp = new Mesh();

            int vertCount = 16;

            Vector3[] verts = new Vector3[vertCount];
            Vector2[] uvsBorder = new Vector2[vertCount];

            int[] indices = new int[4 * 8];

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    verts[x + y * 4] = new Vector3(x < 2 ? -1 : 1, y < 2 ? -1 : 1, 0);
                    uvsBorder[x + y * 4] = new Vector2(x == 0 || x == 3 ? 1 : 0, y == 0 || y == 3 ? 1 : 0);
                }
            }

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    int quadIndex = x + y * 3;
                    if (quadIndex == 4) continue;
                    else if (quadIndex > 4)
                        --quadIndex;

                    int vertIndex = quadIndex * 4;
                    indices[vertIndex] = x + y * 4;
                    indices[vertIndex + 1] = x + (y + 1) * 4;
                    indices[vertIndex + 2] = x + 1 + (y + 1) * 4;
                    indices[vertIndex + 3] = x + 1 + y * 4;
                }
            }

            temp.vertices = verts;
            temp.uv = uvsBorder;
            temp.SetIndices(indices, MeshTopology.Quads, 0);

            return temp;
        }
    }
}