using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Reflection;
using System.Resources;
using System.Collections;
using System.Globalization;

namespace NightOwl
{
    /// <summary>
    /// Interaction logic for Emotions.xaml
    /// </summary>
    public partial class Emotions : UserControl
    {
        public Emotions()
        {
            InitializeComponent();
            GetRes();
        }

        public void GetRes()
        {
            Assembly assembly = Assembly.GetAssembly(this.GetType());
            string resourceName = assembly.GetName().Name + ".g";
            ResourceManager rm = new ResourceManager(resourceName, assembly);

            List<string> list = new List<string>();


            using (ResourceSet set = rm.GetResourceSet(CultureInfo.CurrentCulture, true, true))
            {
                foreach (DictionaryEntry res in set)
                {
                    if (res.Key.ToString().Contains("emotions") && res.Key.ToString().Contains(".gif"))
                    {
                        list.Add(res.Key.ToString());
                    }
                }
            }
            myEmotion.ItemsSource = list;
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;
            string name = image.Source.ToString().Remove(0, image.Source.ToString().LastIndexOf('/')+1);
            name = name.Remove(name.LastIndexOf('.'), 4);
            TextBox tb = (TextBox)Application.Current.Windows[1].FindName("SthToSayTextBox");
            
            tb.Text += name;
        }

    }
}
