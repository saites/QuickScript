using System.Windows;

namespace QuickScript
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ScriptVm ScriptVm { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            ScriptVm = new ScriptVm(@"C:\Users\ajsaites\Documents\Scripts\ScriptFile.xml");
            DataContext = ScriptVm;
        }
    }
}
