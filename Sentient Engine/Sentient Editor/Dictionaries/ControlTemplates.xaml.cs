using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sentient_Editor.Dictionaries
{
    public partial class ControlTemplates : ResourceDictionary
    {
        private void OnTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            var expression = textBox.GetBindingExpression(TextBox.TextProperty);
            if (expression != null) 
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    if (textBox.Tag is ICommand command && command.CanExecute(textBox.Text))
                    {
                        command.Execute(textBox.Text);
                    }
                    else
                    {
                        expression.UpdateSource();
                    }

                    Keyboard.ClearFocus();
                    e.Handled = true;
                }
                else if (e.Key == System.Windows.Input.Key.Escape)
                {
                    expression.UpdateSource(); 
                    Keyboard.ClearFocus();
                }
            }
        }
    }
}
