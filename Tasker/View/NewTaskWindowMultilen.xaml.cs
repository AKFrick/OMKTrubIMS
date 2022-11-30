using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Tasker.ModelView;
using System.Text.RegularExpressions;

namespace Tasker.View
{
    /// <summary>
    /// Interaction logic for NewTaskWindowMultilen.xaml
    /// </summary>
    public partial class NewTaskWindowMultilen : Window
    {
        public NewTaskWindowMultilen(NewTask newTask)
        {
            DataContext = newTask;
            newTask.TaskCreated += Close;
            InitializeComponent();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void ComboBox_Selected(object sender, RoutedEventArgs e)
        {
            (DataContext as NewTask).task.Diameter = Int32.Parse(((sender as ComboBox).SelectedItem as TextBlock).Text);
        }
    }
}
