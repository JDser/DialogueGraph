namespace DialogueEditor.Properties
{
    [SearchEntry("Test", "Test Property")]

    [Input("Input 1", typeof(string), Name = "Duration Input 1 (string) with absurdly long name lol")]
    [Output("Output 1", typeof(string), Name = "Output 1 (string)")]  

    public class TestProperty : AbstractBlockProperty
    {
        public override UnityEngine.Object GetObject()
        {
            return null;
        }

        public override void Compile()
        {

        }
    }
}