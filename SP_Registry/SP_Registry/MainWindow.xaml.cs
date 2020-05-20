using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading;
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

namespace SP_Registry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<RegistryNode> Nodes { get; set; } = new ObservableCollection<RegistryNode>()
        {
            new RegistryNode{ Key = Registry.ClassesRoot, Name = Registry.ClassesRoot.Name },
            new RegistryNode{ Key = Registry.CurrentUser, Name = Registry.CurrentUser.Name },
            new RegistryNode{ Key = Registry.LocalMachine, Name = Registry.LocalMachine.Name },
            new RegistryNode{ Key = Registry.Users, Name = Registry.Users.Name },
            new RegistryNode{ Key = Registry.CurrentConfig, Name = Registry.CurrentConfig.Name },

        };
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            treeView.ItemsSource = Nodes;
            OpenAllSubkeys(new RegistryNode { Nodes = Nodes }, true);
        }

        public void OpenAllSubkeys(RegistryNode selectedNode, bool mainNodes = false)
        {
            Task.Run(() =>
            {
                foreach (var node in selectedNode.Nodes)
                {
                    node.Nodes = new ObservableCollection<RegistryNode>();
                    string[] names = node.Key.GetSubKeyNames();
                    foreach (var name in names)
                    {
                        try
                        {
                            var regNode = new RegistryNode { Name = name, Key = node.Key.OpenSubKey(name) };
                            Dispatcher.Invoke(() => {
                                try
                                {
                                    node.Nodes.Add(regNode);
                                }
                                catch (SecurityException se)
                                {
                                    //Debug.WriteLine(se.Message);
                                }
                            });
                        }
                        catch(SecurityException se) { Debug.WriteLine(se.Message); }
                    }
                }
            });
        }

        public void CloseAllSubkeys(RegistryNode node)
        {
            foreach(var nod in node.Nodes)
            {
                nod.Nodes?.Clear();
            }
        }

        private void treeView_Expanded(object sender, RoutedEventArgs e)
        {
            var node = (e.OriginalSource as TreeViewItem).DataContext as RegistryNode;
            OpenAllSubkeys(node);
        }

        private void treeView_Collapsed(object sender, RoutedEventArgs e)
        {
            var node = (e.OriginalSource as TreeViewItem).DataContext as RegistryNode;
            CloseAllSubkeys(node);
        }

        private void treeView_Selected(object sender, RoutedEventArgs e)
        {
            List<RegistryValue> values = new List<RegistryValue>();
            var node = (e.OriginalSource as TreeViewItem).DataContext as RegistryNode;
            string[] valueNames = node.Key.GetValueNames();
            foreach (var item in valueNames)
            {
                values.Add(new RegistryValue
                {
                    Name = item,
                    Type = node.Key.GetValueKind(item).ToString(),
                    Value = node.Key.GetValue(item).ToString(),
                });
            }
            listView.ItemsSource = values;
        }
    }

    public class RegistryValue
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }


    public class RegistryNode : INotifyPropertyChanged
    {

        private string name;
        private RegistryKey key;
        private ObservableCollection<RegistryNode> nodes;


        public string FullPath { get
            {
                return Key.ToString();
            }
        }

        public string Name { get => name; set 
            {
                name = value;
                OnChange();
            } 
        }
        public RegistryKey Key { get => key; set
            {
                key = value;
                OnChange();
            }
        }
        public ObservableCollection<RegistryNode> Nodes { get => nodes; set
            {
                nodes = value;
                OnChange();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnChange([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
