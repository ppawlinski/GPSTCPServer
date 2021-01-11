using System.Windows.Controls;

namespace GPSTCPClient.Components
{
    public class SearchingBox : ComboBox
    {
        private int caretPosition;
        private TextBox textBox;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var element = GetTemplateChild("PART_EditableTextBox");
            if (element != null)
            {
                textBox = (TextBox)element;
            }
            this.LostFocus += SearchingBox_LostFocus;
            this.DropDownOpened += SearchingBox_DropDownOpened;
        }

        private void SearchingBox_DropDownOpened(object sender, System.EventArgs e)
        {
            if (base.IsDropDownOpen && textBox.SelectionLength > 0)
            {
                caretPosition = textBox.SelectionLength;
                textBox.CaretIndex = caretPosition;
            }
            if (textBox.SelectionLength == 0 && textBox.CaretIndex != 0)
            {
                caretPosition = textBox.CaretIndex;
            }
        }

        private void SearchingBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)GetTemplateChild("PART_EditableTextBox");
            textBox.CaretIndex = 0;
        }
    }
}
