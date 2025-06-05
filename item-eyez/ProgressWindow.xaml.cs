using System.Windows.Controls;
using System.Windows;

namespace item_eyez
{
    public partial class ProgressWindow : Window
    {
        public ProgressWindow()
        {
            InitializeComponent();
        }

        public ProgressBar Bar => progressBar;
    }
}
