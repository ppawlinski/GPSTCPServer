using System.Windows.Controls;

namespace GPSTCPClient.Components
{
    public class SearchingBox : ComboBox
    {
        private int caretPosition;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var element = GetTemplateChild("PART_EditableTextBox");
            if (element != null)
            {
                var textBox = (TextBox)element;
                textBox.SelectionChanged += OnDropSelectionChanged;
                
            }
            this.LostFocus += SearchingBox_LostFocus;
        }

        private void SearchingBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)GetTemplateChild("PART_EditableTextBox");
            textBox.CaretIndex = 0;
        }

        private void OnDropSelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox txt = (TextBox)sender;

            if (base.IsDropDownOpen && txt.SelectionLength > 0)
            {
                caretPosition = txt.SelectionLength;
                txt.CaretIndex = caretPosition;
            }
            if (txt.SelectionLength == 0 && txt.CaretIndex != 0)
            {
                caretPosition = txt.CaretIndex;
            }
        }
    }
}
