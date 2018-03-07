using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DiskExplorer
{
    /// <summary>
    /// Interaction logic for Create.xaml
    /// </summary>
    public partial class CreateWindow : Window
    {
        public bool canCreate { get; private set; }
        public CreateWindow()
        {
            InitializeComponent();
            canCreate = false;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (radioButtonFile.IsChecked == true)
            {
                if (Regex.IsMatch(textBoxName.Text, @"^[a-zA-Z0-9_~-]{0,8}\.(txt|html|php)"))
                {
                    canCreate = true;
                    Close();
                } else
                {
                    MessageBox.Show("Bledna nazwa pliku");
                }
            } else
            {
                canCreate = true;
                Close();
            }

        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            canCreate = false;
            Close();
        }
    }
}
