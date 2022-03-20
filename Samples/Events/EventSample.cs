using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Samples.Events
{
    public class ButtonSample
    {
        public event EventHandler<ButtonSampleEventArg>? ButtonPressed;

        public void OnButtonPressed(string input)
        {
            ButtonPressed?.Invoke(this, new ButtonSampleEventArg(input));
        }
    }

    public class ButtonSampleEventArg
    {
        public ButtonSampleEventArg(string input)
        {
            Input = input;
        }

        public string Input { get; }
    }

    public class EventSampleTest
    {
        [Fact]
        public void ButtonPressedTest()
        {
            // Arrange
            var buttonSample = new ButtonSample();
            string value = String.Empty;
            object? obj = null;
            buttonSample.ButtonPressed += (sender, eventArgs) =>
            {
                value = eventArgs.Input;
                obj = sender;
            };

            // Act
            buttonSample.OnButtonPressed("inputしたよ");

            // Assertion
            Assert.Equal("inputしたよ", value);
            Assert.Equal(typeof(ButtonSample), obj.GetType());
        }
    }
}
