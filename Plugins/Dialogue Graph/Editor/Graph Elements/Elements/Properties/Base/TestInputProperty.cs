using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor.Properties
{
    [SearchEntry("Test", "Test Input")]
    [Input("Input 1", typeof(string), Name = "Duration Input 1 (float) with absurdly long name lol")]
    [Input("Input 2", typeof(string), Name = "Duration Input 2 (float)")]
    [Output("Output 1", typeof(string), Name = "Duration Output 1 (string)")]
    [Output("Output 2", typeof(string), Name = "Duration Output 2 (string)")]
    [Output("Output 3", typeof(string), Name = "Duration Output 3 (string)")]
    public class TestInputProperty : AbstractBlockProperty
    {
        public override string Title => "Test Input";

        public TestInputProperty()
        {
            contentContainer.Add(new Label("Duration : "));
            contentContainer.Add(new FloatField());

            contentContainer.Add(new Label("Duration : "));
            contentContainer.Add(new FloatField());

            contentContainer.Add(new Label("Duration : "));
            contentContainer.Add(new FloatField());
        }

        public override Object GetObject()
        {
            return null;
        }

        public override void Compile()
        {

        }
    }
}