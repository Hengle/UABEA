using AssetsTools.NET;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Globalization;

namespace UABEAvalonia
{
    public partial class AddDependencyWindow : Window
    {
        //controls
        private TextBox boxFileName;
        private TextBox boxOrigFileName;
        private ComboBox ddDepType;
        private TextBox boxGuid;
        private Button btnOk;
        private Button btnCancel;

        public AddDependencyWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            //generated controls
            boxFileName = this.FindControl<TextBox>("boxFileName")!;
            boxOrigFileName = this.FindControl<TextBox>("boxOrigFileName")!;
            ddDepType = this.FindControl<ComboBox>("ddDepType")!;
            boxGuid = this.FindControl<TextBox>("boxGuid")!;
            btnOk = this.FindControl<Button>("btnOk")!;
            btnCancel = this.FindControl<Button>("btnCancel")!;
            //generated events
            boxFileName.AddHandler(TextInputEvent, BoxFileName_TextInput, RoutingStrategies.Tunnel); // no textchanged event
            boxFileName.KeyDown += TextBoxKeyDown;
            boxOrigFileName.KeyDown += TextBoxKeyDown;
            ddDepType.SelectionChanged += DdDepType_SelectionChanged;
            boxGuid.KeyDown += TextBoxKeyDown;
            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        public AddDependencyWindow(string fileName, string origFileName, AssetsFileExternalType depType, GUID128 guid) : this()
        {
            boxFileName.Text = fileName;
            boxOrigFileName.Text = origFileName;
            ddDepType.SelectedIndex = (int)depType;
            boxGuid.Text = guid.ToString();
        }

        private GUID128 GetGuid()
        {
            string guidText = (boxGuid.Text ?? string.Empty).Trim().Replace(" ", "").Replace("-", "");
            if (guidText.Length != 32)
                return default;

            if (GUID128.TryParse(guidText, out GUID128 guid))
            {
                return guid;
            }

            return default;
        }

        private void BoxFileName_TextInput(object? sender, TextInputEventArgs e)
        {
            // avalonia textboxes are null initially, not empty string .-.
            string fileNameTextLower = boxFileName.Text?.ToLower() ?? string.Empty;
            // add new text as well .-. (this doesn't work for backspace/editing, I can't figure out how to yet)
            fileNameTextLower += e.Text?.ToLower();
            boxOrigFileName.IsEnabled = 
                fileNameTextLower.StartsWith("library/") || fileNameTextLower.StartsWith("resources/");
        }

        private void TextBoxKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                AddDependency();
            }
        }

        private void DdDepType_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            // no idea when this should be enabled as I've only seen 0 and 3
            boxGuid.IsEnabled = ddDepType.SelectedIndex != 0;
        }

        private void BtnOk_Click(object? sender, RoutedEventArgs e)
        {
            AddDependency();
        }

        private void AddDependency()
        {
            AssetsFileExternal dependency = new AssetsFileExternal
            {
                VirtualAssetPathName = string.Empty,
                PathName = boxFileName.Text ?? string.Empty, // thanks avalonia
                OriginalPathName = boxOrigFileName.Text != string.Empty ? boxOrigFileName.Text : boxFileName.Text,
                Type = (AssetsFileExternalType)ddDepType.SelectedIndex,
                Guid = GetGuid()
            };

            Close(dependency);
        }

        private void BtnCancel_Click(object? sender, RoutedEventArgs e)
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Close(null);
#pragma warning restore CS8625
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
