using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Forms = System.Windows.Forms;

namespace Optomo
{
    public enum Panels
    {
        X_PANEL,
        Y_PANEL,
        Z_PANEL,
        THREED,
        NONE
    }
    public enum State
    {
        MAX,
        NOR
    }

    public partial class TouchScreenWindow : Window
    {
        public Panels panel = Panels.NONE;
        public State state = State.NOR;
        public bool isStart = false;

        SolidColorBrush green = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#304557"));
        SolidColorBrush blue = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4f7492"));
        SolidColorBrush gray = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8b8b8c"));

        private ImagingWindow window;
        public TouchScreenWindow()
        {
            InitializeComponent();
            window = new ImagingWindow(this, "C:/Users/fatil/OneDrive/Belgeler/Dicoms/beyin");
            window.Show();
            PositionWindowOnSecondMonitor(window);
            StartControl();
        }

        private void PositionWindowOnSecondMonitor(Window window1)
        {
            var screens = Forms.Screen.AllScreens;

            if (screens.Length > 1)
            {
                var screen1 = screens[1];
                var screen2 = screens[0];

                // İlk pencere için
                window1.Left = screen1.WorkingArea.Left;
                window1.Top = screen1.WorkingArea.Top;
                window1.Width = screen1.WorkingArea.Width;
                window1.Height = screen1.WorkingArea.Height;
                window1.WindowState = WindowState.Maximized;
                window1.Show();

                // İkinci pencere için
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                this.Left = screen2.WorkingArea.Left;
                this.Top = screen2.WorkingArea.Top;
                this.Width = screen2.WorkingArea.Width;
                this.Height = screen2.WorkingArea.Height;
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                this.Show();
            }
        }

        private void StartControl()
        {
            layerUpimg.Source = new BitmapImage(new Uri("Assets/upArrowBlue.png", UriKind.Relative));
            layerDownimg.Source = new BitmapImage(new Uri("Assets/downArrowBlue.png", UriKind.Relative));
            motionUpimg.Source = new BitmapImage(new Uri("Assets/upArrowBlue.png", UriKind.Relative));
            motionDownimg.Source = new BitmapImage(new Uri("Assets/downArrowBlue.png", UriKind.Relative));
            motionLeftimg.Source = new BitmapImage(new Uri("Assets/leftArrowBlue.png", UriKind.Relative));
            motionRightimg.Source = new BitmapImage(new Uri("Assets/rightArrowBlue.png", UriKind.Relative));
            centerimg.Source = new BitmapImage(new Uri("Assets/centerArrowBlue.png", UriKind.Relative));
            zoomDownimg.Source = new BitmapImage(new Uri("Assets/minuseBlue.png", UriKind.Relative));
            zoomUpimg.Source = new BitmapImage(new Uri("Assets/plusBlue.png", UriKind.Relative));

            layerUp.IsEnabled = false;
            layerDown.IsEnabled = false;
            motionDown.IsEnabled = false;
            motionUp.IsEnabled = false;
            motionRight.IsEnabled = false;
            motionLeft.IsEnabled = false;
            zoomDown.IsEnabled = false;
            zoomUp.IsEnabled = false;
            zoomSlider.IsEnabled = false;
        }

        #region Pencere Tuşları
        private void minimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            window.Close();
            this.Close();
        }
        #endregion

        #region Panel Değişim Tuşları
        private void TransversalMax_Click(object sender, RoutedEventArgs e)
        {
            if (panel == Panels.THREED || panel == Panels.NONE)
            {
                layerUpimg.Source = new BitmapImage(new Uri("Assets/upArrow.png", UriKind.Relative));
                layerDownimg.Source = new BitmapImage(new Uri("Assets/downArrow.png", UriKind.Relative));
                motionUpimg.Source = new BitmapImage(new Uri("Assets/upArrow.png", UriKind.Relative));
                motionDownimg.Source = new BitmapImage(new Uri("Assets/downArrow.png", UriKind.Relative));
                motionLeftimg.Source = new BitmapImage(new Uri("Assets/leftArrow.png", UriKind.Relative));
                motionRightimg.Source = new BitmapImage(new Uri("Assets/rightArrow.png", UriKind.Relative));
                centerimg.Source = new BitmapImage(new Uri("Assets/centerArrow.png", UriKind.Relative));
                zoomDownimg.Source = new BitmapImage(new Uri("Assets/minuse.png", UriKind.Relative));
                zoomUpimg.Source = new BitmapImage(new Uri("Assets/plus.png", UriKind.Relative));
                layerUp.IsEnabled = true;
                layerDown.IsEnabled = true;
                motionDown.IsEnabled = true;
                motionUp.IsEnabled = true;
                motionRight.IsEnabled = true;
                motionLeft.IsEnabled = true;
                zoomDown.IsEnabled = true;
                zoomUp.IsEnabled = true;
                zoomSlider.IsEnabled = true;
            }
            panel = Panels.Y_PANEL;
            zoomSlider.Value = window.zooms[1];
            if (state == State.NOR)
            {
                window.MainGrid.RowDefinitions.Clear();
                window.MainGrid.ColumnDefinitions.Clear();
                window.Y_Slice.Visibility = Visibility.Visible;
                window.X_Slice.Visibility = Visibility.Collapsed;
                window.Z_Slice.Visibility = Visibility.Collapsed;
                window.ThreeD.Visibility = Visibility.Collapsed;
                state = State.MAX;
                var newUri = new Uri("Assets/minimize.png", UriKind.Relative);
                YMax.Source = new BitmapImage(newUri);
                XMax.Source = new BitmapImage(newUri);
                ZMax.Source = new BitmapImage(newUri);
                TMax.Source = new BitmapImage(newUri);
            }
            else
            {
                window.MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                window.MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                window.MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                window.MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                window.X_Slice.Visibility = Visibility.Visible;
                window.Y_Slice.Visibility = Visibility.Visible;
                window.Z_Slice.Visibility = Visibility.Visible;
                window.ThreeD.Visibility = Visibility.Visible;
                state = State.NOR;
                var newUri = new Uri("Assets/maximize.png", UriKind.Relative);
                YMax.Source = new BitmapImage(newUri);
                XMax.Source = new BitmapImage(newUri);
                ZMax.Source = new BitmapImage(newUri);
                TMax.Source = new BitmapImage(newUri);
            }
            layerNum.Text = window.extent[3].ToString();
            layerCur.Text = window.layers[1].ToString();
            TransversalGrid.Background = green;
            SagittalGrid.Background = blue;
            CoronalGrid.Background = blue;
            D3ViewGrid.Background = blue;
            window.YBorder.BorderBrush = blue;
            window.XBorder.BorderBrush = gray;
            window.ZBorder.BorderBrush = gray;
            window.TBorder.BorderBrush = gray;
        }

        private void Transversal_Click(object sender, RoutedEventArgs e)
        {
            if (panel == Panels.THREED || panel == Panels.NONE)
            {
                layerUpimg.Source = new BitmapImage(new Uri("Assets/upArrow.png", UriKind.Relative));
                layerDownimg.Source = new BitmapImage(new Uri("Assets/downArrow.png", UriKind.Relative));
                motionUpimg.Source = new BitmapImage(new Uri("Assets/upArrow.png", UriKind.Relative));
                motionDownimg.Source = new BitmapImage(new Uri("Assets/downArrow.png", UriKind.Relative));
                motionLeftimg.Source = new BitmapImage(new Uri("Assets/leftArrow.png", UriKind.Relative));
                motionRightimg.Source = new BitmapImage(new Uri("Assets/rightArrow.png", UriKind.Relative));
                centerimg.Source = new BitmapImage(new Uri("Assets/centerArrow.png", UriKind.Relative));
                zoomDownimg.Source = new BitmapImage(new Uri("Assets/minuse.png", UriKind.Relative));
                zoomUpimg.Source = new BitmapImage(new Uri("Assets/plus.png", UriKind.Relative));
                layerUp.IsEnabled = true;
                layerDown.IsEnabled = true;
                motionDown.IsEnabled = true;
                motionUp.IsEnabled = true;
                motionRight.IsEnabled = true;
                motionLeft.IsEnabled = true;
                zoomDown.IsEnabled = true;
                zoomUp.IsEnabled = true;
                zoomSlider.IsEnabled = true;
            }
            panel = Panels.Y_PANEL;
            zoomSlider.Value = window.zooms[1];
            layerNum.Text = window.extent[3].ToString();
            layerCur.Text = window.layers[1].ToString();
            TransversalGrid.Background = green;
            SagittalGrid.Background = blue;
            CoronalGrid.Background = blue;
            D3ViewGrid.Background = blue;
            if (state == State.MAX)
            {
                window.Y_Slice.Visibility = Visibility.Visible;
                window.X_Slice.Visibility = Visibility.Collapsed;
                window.Z_Slice.Visibility = Visibility.Collapsed;
                window.ThreeD.Visibility = Visibility.Collapsed;
            }
            window.YBorder.BorderBrush = blue;
            window.XBorder.BorderBrush = gray;
            window.ZBorder.BorderBrush = gray;
            window.TBorder.BorderBrush = gray;
        }

        private void SagittalMax_Click(object sender, RoutedEventArgs e)
        {
            if (panel == Panels.THREED || panel == Panels.NONE)
            {
                layerUpimg.Source = new BitmapImage(new Uri("Assets/upArrow.png", UriKind.Relative));
                layerDownimg.Source = new BitmapImage(new Uri("Assets/downArrow.png", UriKind.Relative));
                motionUpimg.Source = new BitmapImage(new Uri("Assets/upArrow.png", UriKind.Relative));
                motionDownimg.Source = new BitmapImage(new Uri("Assets/downArrow.png", UriKind.Relative));
                motionLeftimg.Source = new BitmapImage(new Uri("Assets/leftArrow.png", UriKind.Relative));
                motionRightimg.Source = new BitmapImage(new Uri("Assets/rightArrow.png", UriKind.Relative));
                centerimg.Source = new BitmapImage(new Uri("Assets/centerArrow.png", UriKind.Relative));
                zoomDownimg.Source = new BitmapImage(new Uri("Assets/minuse.png", UriKind.Relative));
                zoomUpimg.Source = new BitmapImage(new Uri("Assets/plus.png", UriKind.Relative));
                layerUp.IsEnabled = true;
                layerDown.IsEnabled = true;
                motionDown.IsEnabled = true;
                motionUp.IsEnabled = true;
                motionRight.IsEnabled = true;
                motionLeft.IsEnabled = true;
                zoomDown.IsEnabled = true;
                zoomUp.IsEnabled = true;
                zoomSlider.IsEnabled = true;
            }
            panel = Panels.X_PANEL;
            zoomSlider.Value = window.zooms[0];
            if (state == State.NOR)
            {
                window.MainGrid.RowDefinitions.Clear();
                window.MainGrid.ColumnDefinitions.Clear();
                window.X_Slice.Visibility= Visibility.Visible;
                window.Y_Slice.Visibility = Visibility.Collapsed;
                window.Z_Slice.Visibility = Visibility.Collapsed;
                window.ThreeD.Visibility = Visibility.Collapsed;
                state = State.MAX;
                var newUri = new Uri("Assets/minimize.png", UriKind.Relative);
                YMax.Source = new BitmapImage(newUri);
                XMax.Source = new BitmapImage(newUri);
                ZMax.Source = new BitmapImage(newUri);
                TMax.Source = new BitmapImage(newUri);
            }
            else
            {
                window.MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                window.MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                window.MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                window.MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                window.X_Slice.Visibility = Visibility.Visible;
                window.Y_Slice.Visibility = Visibility.Visible;
                window.Z_Slice.Visibility = Visibility.Visible;
                window.ThreeD.Visibility = Visibility.Visible;
                state = State.NOR;
                var newUri = new Uri("Assets/maximize.png", UriKind.Relative);
                YMax.Source = new BitmapImage(newUri);
                XMax.Source = new BitmapImage(newUri);
                ZMax.Source = new BitmapImage(newUri);
                TMax.Source = new BitmapImage(newUri);
            }
            layerNum.Text = window.extent[1].ToString();
            layerCur.Text = window.layers[0].ToString();
            TransversalGrid.Background = blue;
            SagittalGrid.Background = green;
            CoronalGrid.Background = blue;
            D3ViewGrid.Background = blue;
            window.YBorder.BorderBrush = gray;
            window.XBorder.BorderBrush = blue;
            window.ZBorder.BorderBrush = gray;
            window.TBorder.BorderBrush = gray;
        }

        private void Sagittal_Click(object sender, RoutedEventArgs e)
        {
            if (panel == Panels.THREED || panel == Panels.NONE)
            {
                layerUpimg.Source = new BitmapImage(new Uri("Assets/upArrow.png", UriKind.Relative));
                layerDownimg.Source = new BitmapImage(new Uri("Assets/downArrow.png", UriKind.Relative));
                motionUpimg.Source = new BitmapImage(new Uri("Assets/upArrow.png", UriKind.Relative));
                motionDownimg.Source = new BitmapImage(new Uri("Assets/downArrow.png", UriKind.Relative));
                motionLeftimg.Source = new BitmapImage(new Uri("Assets/leftArrow.png", UriKind.Relative));
                motionRightimg.Source = new BitmapImage(new Uri("Assets/rightArrow.png", UriKind.Relative));
                centerimg.Source = new BitmapImage(new Uri("Assets/centerArrow.png", UriKind.Relative));
                zoomDownimg.Source = new BitmapImage(new Uri("Assets/minuse.png", UriKind.Relative));
                zoomUpimg.Source = new BitmapImage(new Uri("Assets/plus.png", UriKind.Relative));
                layerUp.IsEnabled = true;
                layerDown.IsEnabled = true;
                motionDown.IsEnabled = true;
                motionUp.IsEnabled = true;
                motionRight.IsEnabled = true;
                motionLeft.IsEnabled = true;
                zoomDown.IsEnabled = true;
                zoomUp.IsEnabled = true;
                zoomSlider.IsEnabled = true;
            }
            panel = Panels.X_PANEL;
            zoomSlider.Value = window.zooms[0];
            layerNum.Text = window.extent[1].ToString();
            layerCur.Text = window.layers[0].ToString();
            TransversalGrid.Background = blue;
            SagittalGrid.Background = green;
            CoronalGrid.Background = blue;
            D3ViewGrid.Background = blue;
            if (state == State.MAX)
            {
                window.X_Slice.Visibility = Visibility.Visible;
                window.Y_Slice.Visibility = Visibility.Collapsed;
                window.Z_Slice.Visibility = Visibility.Collapsed;
                window.ThreeD.Visibility = Visibility.Collapsed;
            }
            window.YBorder.BorderBrush = gray;
            window.XBorder.BorderBrush = blue;
            window.ZBorder.BorderBrush = gray;
            window.TBorder.BorderBrush = gray;
        }

        private void CoronalMax_Click(object sender, RoutedEventArgs e)
        {
            if (panel == Panels.THREED || panel == Panels.NONE)
            {
                layerUpimg.Source = new BitmapImage(new Uri("Assets/upArrow.png", UriKind.Relative));
                layerDownimg.Source = new BitmapImage(new Uri("Assets/downArrow.png", UriKind.Relative));
                motionUpimg.Source = new BitmapImage(new Uri("Assets/upArrow.png", UriKind.Relative));
                motionDownimg.Source = new BitmapImage(new Uri("Assets/downArrow.png", UriKind.Relative));
                motionLeftimg.Source = new BitmapImage(new Uri("Assets/leftArrow.png", UriKind.Relative));
                motionRightimg.Source = new BitmapImage(new Uri("Assets/rightArrow.png", UriKind.Relative));
                centerimg.Source = new BitmapImage(new Uri("Assets/centerArrow.png", UriKind.Relative));
                zoomDownimg.Source = new BitmapImage(new Uri("Assets/minuse.png", UriKind.Relative));
                zoomUpimg.Source = new BitmapImage(new Uri("Assets/plus.png", UriKind.Relative));
                layerUp.IsEnabled = true;
                layerDown.IsEnabled = true;
                motionDown.IsEnabled = true;
                motionUp.IsEnabled = true;
                motionRight.IsEnabled = true;
                motionLeft.IsEnabled = true;
                zoomDown.IsEnabled = true;
                zoomUp.IsEnabled = true;
                zoomSlider.IsEnabled = true;
            }
            panel = Panels.Z_PANEL;
            zoomSlider.Value = window.zooms[2];
            if (state == State.NOR)
            {
                window.MainGrid.RowDefinitions.Clear();
                window.MainGrid.ColumnDefinitions.Clear();
                window.Z_Slice.Visibility = Visibility.Visible;
                window.Y_Slice.Visibility = Visibility.Collapsed;
                window.X_Slice.Visibility = Visibility.Collapsed;
                window.ThreeD.Visibility = Visibility.Collapsed;
                state = State.MAX;
                var newUri = new Uri("Assets/minimize.png", UriKind.Relative);
                YMax.Source = new BitmapImage(newUri);
                XMax.Source = new BitmapImage(newUri);
                ZMax.Source = new BitmapImage(newUri);
                TMax.Source = new BitmapImage(newUri);
            }
            else
            {
                window.MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                window.MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                window.MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                window.MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                window.X_Slice.Visibility = Visibility.Visible;
                window.Y_Slice.Visibility = Visibility.Visible;
                window.Z_Slice.Visibility = Visibility.Visible;
                window.ThreeD.Visibility = Visibility.Visible;
                state = State.NOR;
                var newUri = new Uri("Assets/maximize.png", UriKind.Relative);
                YMax.Source = new BitmapImage(newUri);
                XMax.Source = new BitmapImage(newUri);
                ZMax.Source = new BitmapImage(newUri);
                TMax.Source = new BitmapImage(newUri);
            }
            layerNum.Text = window.extent[5].ToString();
            layerCur.Text = window.layers[2].ToString();
            TransversalGrid.Background = blue;
            SagittalGrid.Background = blue;
            CoronalGrid.Background = green;
            D3ViewGrid.Background = blue;
            window.YBorder.BorderBrush = gray;
            window.XBorder.BorderBrush = gray;
            window.ZBorder.BorderBrush = blue;
            window.TBorder.BorderBrush = gray;
        }

        private void Coronal_Click(object sender, RoutedEventArgs e)
        {
            if (panel == Panels.THREED || panel == Panels.NONE)
            {
                layerUpimg.Source = new BitmapImage(new Uri("Assets/upArrow.png", UriKind.Relative));
                layerDownimg.Source = new BitmapImage(new Uri("Assets/downArrow.png", UriKind.Relative));
                motionUpimg.Source = new BitmapImage(new Uri("Assets/upArrow.png", UriKind.Relative));
                motionDownimg.Source = new BitmapImage(new Uri("Assets/downArrow.png", UriKind.Relative));
                motionLeftimg.Source = new BitmapImage(new Uri("Assets/leftArrow.png", UriKind.Relative));
                motionRightimg.Source = new BitmapImage(new Uri("Assets/rightArrow.png", UriKind.Relative));
                centerimg.Source = new BitmapImage(new Uri("Assets/centerArrow.png", UriKind.Relative));
                zoomDownimg.Source = new BitmapImage(new Uri("Assets/minuse.png", UriKind.Relative));
                zoomUpimg.Source = new BitmapImage(new Uri("Assets/plus.png", UriKind.Relative));
                layerUp.IsEnabled = true;
                layerDown.IsEnabled = true;
                motionDown.IsEnabled = true;
                motionUp.IsEnabled = true;
                motionRight.IsEnabled = true;
                motionLeft.IsEnabled = true;
                zoomDown.IsEnabled = true;
                zoomUp.IsEnabled = true;
                zoomSlider.IsEnabled = true;
            }
            panel = Panels.Z_PANEL;
            zoomSlider.Value = window.zooms[2];
            layerNum.Text = window.extent[5].ToString();
            layerCur.Text = window.layers[2].ToString();
            TransversalGrid.Background = blue;
            SagittalGrid.Background = blue;
            CoronalGrid.Background = green;
            D3ViewGrid.Background = blue;
            if (state == State.MAX)
            {
                window.Z_Slice.Visibility = Visibility.Visible;
                window.X_Slice.Visibility = Visibility.Collapsed;
                window.Y_Slice.Visibility = Visibility.Collapsed;
                window.ThreeD.Visibility = Visibility.Collapsed;
            }
            window.YBorder.BorderBrush = gray;
            window.XBorder.BorderBrush = gray;
            window.ZBorder.BorderBrush = blue;
            window.TBorder.BorderBrush = gray;
        }

        private void D3ViewMax_Click(object sender, RoutedEventArgs e)
        {
            panel = Panels.THREED;
            if (state == State.NOR)
            {
                window.MainGrid.RowDefinitions.Clear();
                window.MainGrid.ColumnDefinitions.Clear();
                window.ThreeD.Visibility = Visibility.Visible;
                window.Y_Slice.Visibility = Visibility.Collapsed;
                window.Z_Slice.Visibility = Visibility.Collapsed;
                window.X_Slice.Visibility = Visibility.Collapsed;
                state = State.MAX;
                var newUri = new Uri("Assets/minimize.png", UriKind.Relative);
                YMax.Source = new BitmapImage(newUri);
                XMax.Source = new BitmapImage(newUri);
                ZMax.Source = new BitmapImage(newUri);
                TMax.Source = new BitmapImage(newUri);
            }
            else
            {
                window.MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                window.MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                window.MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                window.MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                window.X_Slice.Visibility = Visibility.Visible;
                window.Y_Slice.Visibility = Visibility.Visible;
                window.Z_Slice.Visibility = Visibility.Visible;
                window.ThreeD.Visibility = Visibility.Visible;
                state = State.NOR;
                var newUri = new Uri("Assets/maximize.png", UriKind.Relative);
                YMax.Source = new BitmapImage(newUri);
                XMax.Source = new BitmapImage(newUri);
                ZMax.Source = new BitmapImage(newUri);
                TMax.Source = new BitmapImage(newUri);
            }
            layerUpimg.Source = new BitmapImage(new Uri("Assets/upArrowBlue.png", UriKind.Relative));
            layerDownimg.Source = new BitmapImage(new Uri("Assets/downArrowBlue.png", UriKind.Relative));
            motionUpimg.Source = new BitmapImage(new Uri("Assets/upArrowBlue.png", UriKind.Relative));
            motionDownimg.Source = new BitmapImage(new Uri("Assets/downArrowBlue.png", UriKind.Relative));
            motionLeftimg.Source = new BitmapImage(new Uri("Assets/leftArrowBlue.png", UriKind.Relative));
            motionRightimg.Source = new BitmapImage(new Uri("Assets/rightArrowBlue.png", UriKind.Relative));
            centerimg.Source = new BitmapImage(new Uri("Assets/centerArrowBlue.png", UriKind.Relative));
            zoomDownimg.Source = new BitmapImage(new Uri("Assets/minuseBlue.png", UriKind.Relative));
            zoomUpimg.Source = new BitmapImage(new Uri("Assets/plusBlue.png", UriKind.Relative));
            layerUp.IsEnabled = false;
            layerDown.IsEnabled = false;
            motionDown.IsEnabled = false;
            motionUp.IsEnabled = false;
            motionRight.IsEnabled = false;
            motionLeft.IsEnabled = false;
            zoomDown.IsEnabled = false;
            zoomUp.IsEnabled = false;
            zoomSlider.IsEnabled = false;
            TransversalGrid.Background = blue;
            SagittalGrid.Background = blue;
            CoronalGrid.Background = blue;
            D3ViewGrid.Background = green;
            window.YBorder.BorderBrush = gray;
            window.XBorder.BorderBrush = gray;
            window.ZBorder.BorderBrush = gray;
            window.TBorder.BorderBrush = blue;
        }

        private void D3View_Click(object sender, RoutedEventArgs e)
        {
            panel = Panels.THREED;
            TransversalGrid.Background = blue;
            SagittalGrid.Background = blue;
            CoronalGrid.Background = blue;
            D3ViewGrid.Background = green;
            if (state == State.MAX)
            {
                window.ThreeD.Visibility = Visibility.Visible;
                window.X_Slice.Visibility = Visibility.Collapsed;
                window.Z_Slice.Visibility = Visibility.Collapsed;
                window.Y_Slice.Visibility = Visibility.Collapsed;
            }
            window.YBorder.BorderBrush = gray;
            window.XBorder.BorderBrush = gray;
            window.ZBorder.BorderBrush = gray;
            window.TBorder.BorderBrush = blue;

            layerNum.Text = "";
            layerCur.Text = "";

            layerUpimg.Source = new BitmapImage(new Uri("Assets/upArrowBlue.png", UriKind.Relative));
            layerDownimg.Source = new BitmapImage(new Uri("Assets/downArrowBlue.png", UriKind.Relative));
            motionUpimg.Source = new BitmapImage(new Uri("Assets/upArrowBlue.png", UriKind.Relative));
            motionDownimg.Source = new BitmapImage(new Uri("Assets/downArrowBlue.png", UriKind.Relative));
            motionLeftimg.Source = new BitmapImage(new Uri("Assets/leftArrowBlue.png", UriKind.Relative));
            motionRightimg.Source = new BitmapImage(new Uri("Assets/rightArrowBlue.png", UriKind.Relative));
            centerimg.Source = new BitmapImage(new Uri("Assets/centerArrowBlue.png", UriKind.Relative));
            zoomDownimg.Source = new BitmapImage(new Uri("Assets/minuseBlue.png", UriKind.Relative));
            zoomUpimg.Source = new BitmapImage(new Uri("Assets/plusBlue.png", UriKind.Relative));

            layerUp.IsEnabled = false;
            layerDown.IsEnabled = false;
            motionDown.IsEnabled = false;
            motionUp.IsEnabled = false;
            motionRight.IsEnabled = false;
            motionLeft.IsEnabled = false;
            zoomDown.IsEnabled = false;
            zoomUp.IsEnabled = false;
            zoomSlider.IsEnabled = false;
        }
        #endregion

        #region Layer Tuşları
        private void layerUp_Click(object sender, RoutedEventArgs e)
        {
            if (panel == Panels.Y_PANEL)
            {
                if (window.layers[1] < window.extent[3])
                {
                    window.YLayerMotion(1);
                    layerCur.Text = window.layers[1].ToString();
                }
            }
            else if (panel == Panels.X_PANEL)
            {
                if (window.layers[0] < window.extent[1])
                {
                    window.XLayerMotion(1);
                    layerCur.Text = window.layers[0].ToString();
                }
            }
            else if (panel == Panels.Z_PANEL)
            {
                if (window.layers[2] < window.extent[5])
                {
                    window.ZLayerMotion(1);
                    layerCur.Text = window.layers[2].ToString();
                }
            }
            window.RenderAll();
        }

        private void layerDown_Click(object sender, RoutedEventArgs e)
        {
            if (panel == Panels.Y_PANEL)
            {
                if (window.layers[1] > 0)
                {
                    window.YLayerMotion(-1);
                    layerCur.Text = window.layers[1].ToString();
                }
            }
            else if (panel == Panels.X_PANEL)
            {
                if (window.layers[0] > 0)
                {
                    window.XLayerMotion(-1);
                    layerCur.Text = window.layers[0].ToString();
                }
            }
            else if (panel == Panels.Z_PANEL)
            {
                if (window.layers[2] > 0)
                {
                    window.ZLayerMotion(-1);
                    layerCur.Text = window.layers[2].ToString();
                }
            }
            window.RenderAll();
        }
        #endregion

        #region Hareket Tuşları
        private void motionUp_Click(object sender, RoutedEventArgs e)
        {
            if (panel == Panels.Y_PANEL)
            {
                window.ZLayerMotion(1);
            }
            else if (panel == Panels.X_PANEL)
            {
                window.ZLayerMotion(1);
            }
            else if (panel == Panels.Z_PANEL)
            {
                window.YLayerMotion(1);
            }
        }

        private void motionDown_Click(object sender, RoutedEventArgs e)
        {
            if (panel == Panels.Y_PANEL)
            {
                window.ZLayerMotion(-1);
            }
            else if (panel == Panels.X_PANEL)
            {
                window.ZLayerMotion(-1);
            }
            else if (panel == Panels.Z_PANEL)
            {
                window.YLayerMotion(-1);
            }
        }

        private void motionLeft_Click(object sender, RoutedEventArgs e)
        {
            if (panel == Panels.Y_PANEL)
            {
                window.XLayerMotion(-1);
            }
            else if (panel == Panels.X_PANEL)
            {
                window.YLayerMotion(-1);
            }
            else if (panel == Panels.Z_PANEL)
            {
                window.XLayerMotion(-1);
            }
        }

        private void motionRight_Click(object sender, RoutedEventArgs e)
        {
            if (panel == Panels.Y_PANEL)
            {
                window.XLayerMotion(1);
            }
            else if (panel == Panels.X_PANEL)
            {
                window.YLayerMotion(1);
            }
            else if (panel == Panels.Z_PANEL)
            {
                window.XLayerMotion(1);
            }
        }
        #endregion
        
        #region Zoom Tuşları
        private void zoomDown_Click(object sender, RoutedEventArgs e)
        {
            if (zoomSlider.Value > zoomSlider.Minimum)
            {
                zoomSlider.Value--;
            }
        }

        private void zoomUp_Click(object sender, RoutedEventArgs e)
        {
            if (zoomSlider.Value < zoomSlider.Maximum)
            {
                zoomSlider.Value++;
            }
        }

        private void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isStart)
            {
                if (panel == Panels.Y_PANEL)
                {
                    window.zooms[1] = zoomSlider.Value;
                }
                else if (panel == Panels.X_PANEL)
                {
                    window.zooms[0] = zoomSlider.Value;
                }
                else if (panel == Panels.Z_PANEL)
                {
                    window.zooms[2] = zoomSlider.Value;
                }
                window.SetPanels();
                window.XLayer(0);
                window.YLayer(0);
                window.ZLayer(0);
                window.RenderReslices();
            }
            else
            {
                isStart = true;
            }
        }
        #endregion
        
        #region Reset Tuşu
        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                window.zooms[i] = 1;
                window.motions[i] = 0;
                window.layers[i] = window.extent[i * 2 + 1] / 2;
                window.camera3D.SetPosition(window.cameraPos[0], window.cameraPos[1], window.cameraPos[2]);
                window.camera3D.SetFocalPoint(window.cameraFoc[0], window.cameraFoc[1], window.cameraFoc[2]);
                window.camera3D.SetViewUp(window.cameraView[0], window.cameraView[1], window.cameraView[2]);
                window.SetPanels();
                window.XLayer(0);
                window.YLayer(0);
                window.ZLayer(0);
                window.renderers[3].ResetCamera();
                if (state == State.NOR)
                {
                    window.RenderAll();
                }
                else
                {
                    if (panel == Panels.X_PANEL)
                    {
                        window.RenderReslices(0);
                    }
                    else if (panel == Panels.Y_PANEL)
                    {
                        window.RenderReslices(1);
                    }
                    else if (panel == Panels.Z_PANEL)
                    {
                        window.RenderReslices(2);
                    }
                    else if (panel == Panels.THREED)
                    {
                        window.Render3D();
                    }
                }
            }
        }
        #endregion

        #region WW/WL Tuşları
        private void WWSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            for (int i = 0; i < 3; i++)
            {
                window.mappers[i].SetColorWindow(e.NewValue * 4);
            }
            window.RenderReslices();
        }

        private void WLSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            for (int i = 0; i < 3; i++)
            {
                window.mappers[i].SetColorLevel(e.NewValue * 4);
            }
            window.RenderReslices();
        }
        #endregion

        #region 3D Color Seçenekleri
        private void color1_Click(object sender, RoutedEventArgs e)
        {
            window.color3D.RemoveAllPoints();
            window.color3D.AddRGBPoint(0, 0.0, 0.0, 1.0);   // Blue
            window.color3D.AddRGBPoint(128, 0.0, 1.0, 0.0); // Green
            window.color3D.AddRGBPoint(255, 1.0, 0.0, 0.0); // Red
            window.Render3D();
        }

        private void color2_Click(object sender, RoutedEventArgs e)
        {
            window.color3D.RemoveAllPoints();
            window.color3D.AddRGBPoint(0, 0.0, 0.0, 0.0);   // Black
            window.color3D.AddRGBPoint(128, 1.0, 0.0, 0.0); // Red
            window.color3D.AddRGBPoint(255, 1.0, 1.0, 0.0); // Yellow
            window.Render3D();
        }

        private void color3_Click(object sender, RoutedEventArgs e)
        {
            window.color3D.RemoveAllPoints();
            window.color3D.AddRGBPoint(0, 0.0, 1.0, 1.0);   // Cyan
            window.color3D.AddRGBPoint(128, 0.0, 0.0, 1.0); // Blue
            window.color3D.AddRGBPoint(255, 1.0, 0.0, 1.0); // Magenta
            window.Render3D();
        }
        #endregion
    }
}
