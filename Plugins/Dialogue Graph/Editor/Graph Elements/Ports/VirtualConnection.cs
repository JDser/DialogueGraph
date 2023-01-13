using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor
{
    public class VirtualConnectionBase : VisualElement
    {

    }

    public class VirtualConnection<ElementType>/*, DataType>*/ : VirtualConnectionBase where ElementType : VisualElement
    {
        private ElementType m_InputField;

        //private Action<ElementType, DataType> m_SetData;
        //private Func<ElementType, DataType> m_GetData;

        public ElementType InputField => m_InputField;

        public VirtualConnection(Color color, ElementType inputField/*, Action<ElementType, DataType> setData, Func<ElementType, DataType> getData*/)
        {
            //this.m_SetData = setData;
            //this.m_GetData = getData;

            /* Edge */
            EdgeControl edgeControl = new EdgeControl();
            edgeControl.inputColor = color;
            edgeControl.outputColor = color;
            edgeControl.from = new Vector2(130, 12);
            edgeControl.to = new Vector2(160, 12);
            edgeControl.UpdateLayout();
            Add(edgeControl);

            this.LoadUXMLTree(DialogueResourceManager.VirtualConnectionUXML);
            AddToClassList("virtualConnection");
            this.pickingMode = PickingMode.Ignore;

            m_InputField = inputField;
            this.Q("contentContainer").Insert(1, m_InputField);
            this.Q("dot").style.backgroundColor = color;
        }

        //public DataType GetData()
        //{
        //    return m_GetData(m_InputField);
        //}

        //public void SetData(DataType data)
        //{
        //    m_SetData(m_InputField, data);
        //}
    }
}