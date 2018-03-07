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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
//using System.Windows.Forms;
using System.IO;

namespace DiskExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string root;
        public TreeViewItem item = new TreeViewItem();
        ContextMenu FileContextMenu = new ContextMenu();
        ContextMenu DirectoryContextMenu = new ContextMenu();


        public MainWindow()
        {
            InitializeComponent();
            
            MenuItem Create = new MenuItem();
            Create.Header = "Create";
            Create.Click += Create_Click;
            MenuItem Open = new MenuItem();
            Open.Header = "Open";
            Open.Click += Open_Click;
            MenuItem OpenDirectory = new MenuItem();
            OpenDirectory.Header = "Otworz lokalizacje";
            OpenDirectory.Click += OpenDirectory_Click;
            MenuItem CalculateFiles = new MenuItem();
            CalculateFiles.Header = "Ile plikow";
            CalculateFiles.Click += Count_Click;
            MenuItem DeleteFile = new MenuItem();
            DeleteFile.Header = "Delete File";
            DeleteFile.Click += DeleteFile_Click;
            MenuItem DeleteFolder = new MenuItem();
            DeleteFolder.Header = "Delete Folder";
            DeleteFolder.Click += DeleteFolder_Click;
            FileContextMenu.Items.Add(OpenDirectory);
            FileContextMenu.Items.Add(Open);
            FileContextMenu.Items.Add(DeleteFile);
            DirectoryContextMenu.Items.Add(CalculateFiles);
            DirectoryContextMenu.Items.Add(Create);
            DirectoryContextMenu.Items.Add(DeleteFolder);




        }

        private void OpenDirectory_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem file = (TreeViewItem)(treeView.SelectedItem);
            TreeViewItem parent = (TreeViewItem)(file.Parent);
            string path = parent.Tag.ToString();
            Process.Start(path);
        }

        private void Count_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem folder = (TreeViewItem)(treeView.SelectedItem);
            string path = folder.Tag.ToString();
            DirectoryInfo directory= new DirectoryInfo(path);
            MessageBox.Show("Plikow w katalogu " + directory.Name + ": " + directory.GetFiles().Length + "\nFolderow w katalogu " + directory.Name + ": " + directory.GetDirectories().Length);
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            CreateWindow createDialog = new CreateWindow();
            createDialog.Owner = this;
            createDialog.ShowDialog();
            TreeViewItem folder = (TreeViewItem)(treeView.SelectedItem);
            string path = folder.Tag.ToString();
            path += "\\" + createDialog.textBoxName.Text;
            TreeViewItem newItem = new TreeViewItem();
            newItem.Header = createDialog.textBoxName.Text;
            newItem.Tag = path;
            folder.Items.Add(newItem);
            if (createDialog.canCreate) {
                if (createDialog.radioButtonFile.IsChecked == true)
                {
                    FileStream fs = File.Create(path);
                    fs.Close();
                    newItem.ContextMenu = FileContextMenu;
                }
                else
                {
                    Directory.CreateDirectory(path);
                    newItem.ContextMenu = DirectoryContextMenu;
                }
                if (createDialog.checkBoxArchive.IsChecked == true)
                {
                    File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Archive);
                }
                if (createDialog.checkBoxHidden.IsChecked == true)
                {
                    File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
                }
                if (createDialog.checkBoxReadOnly.IsChecked == true)
                {
                    File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.ReadOnly);
                }
                if (createDialog.checkBoxSystem.IsChecked == true)
                {
                    File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.System);
                }
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem file = (TreeViewItem)(treeView.SelectedItem);
            string path = file.Tag.ToString();
            textBlock.Text = File.ReadAllText(path);
            
        }

        private void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem file = (TreeViewItem)(treeView.SelectedItem);
            string path = file.Tag.ToString();
            DeleteFile(new FileInfo(path));
            TreeViewItem parent = (TreeViewItem)(file.Parent);
            parent.Items.Remove(file);
        }

        private void DeleteFolder_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem folder = (TreeViewItem)(treeView.SelectedItem);
            string path = folder.Tag.ToString();
            DeleteFolder(new DirectoryInfo(path));
            TreeViewItem parent = (TreeViewItem)(folder.Parent);
            parent.Items.Remove(folder);
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog()
            {
                Description = "Wybierz korzen"
            };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                treeView.Items.Clear();
                root = dlg.SelectedPath;
                item = createTree(new DirectoryInfo(root));
                treeView.Items.Add(item);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void DeleteFolder(DirectoryInfo directory)
        {
            foreach (FileInfo f in directory.GetFiles())
            {
                DeleteFile(f);
            }
            foreach (DirectoryInfo d in directory.GetDirectories())
            {
                File.SetAttributes(d.FullName, FileAttributes.Normal);
                DeleteFolder(d);
            }
            Directory.Delete(directory.FullName);
        }

        public void DeleteFile(FileInfo file)
        {
            File.SetAttributes(file.FullName, FileAttributes.Normal);
            File.Delete(file.FullName);
        }


        private TreeViewItem createTree(FileInfo file)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = file.Name;
            item.ContextMenu = FileContextMenu;
            item.Tag = file.FullName;
            return item;
        }

        private TreeViewItem createTree(DirectoryInfo directory)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = directory.Name;
            item.ContextMenu = DirectoryContextMenu;
            item.Tag = directory.FullName;
            foreach (DirectoryInfo d in directory.GetDirectories())
            {
                item.Items.Add(createTree(d));

            }
            foreach (FileInfo f in directory.GetFiles())
            {
                item.Items.Add(createTree(f));
            }

            return item;
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem item = (TreeViewItem)(treeView.SelectedItem);
            string rahs = "";
            if (item != null)
            {
                string path = item.Tag.ToString();
                rahs = "";
                FileAttributes attributes = File.GetAttributes(path);
                if ((attributes & FileAttributes.ReadOnly) != 0) rahs += "r";
                else rahs += "-";
                if ((attributes & FileAttributes.Archive) != 0) rahs += "a";
                else rahs += "-";
                if ((attributes & FileAttributes.Hidden) != 0) rahs += "h";
                else rahs += "-";
                if ((attributes & FileAttributes.System) != 0) rahs += "s";
                else rahs += "-";

            } else
                rahs = "----";
            textBlockRAHS.Text = rahs;
        }
    }


}

