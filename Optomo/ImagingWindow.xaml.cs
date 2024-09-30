using Kitware.VTK;
using System.Windows;
using System.Windows.Media;

namespace Optomo
{
    public partial class ImagingWindow : Window
    {
        #region Değişkenler
        private int isStart = 0;
        private TouchScreenWindow touchScreenWindow;
        private string path;
        public int[] extent = new int[6];
        public double[] spacing = new double[3];
        public double[] origin = new double[3];
        public double[] center = new double[3];
        public double[] oran = new double[3];
        public vtkImageReslice[] reslices = new vtkImageReslice[3];
        public vtkImageMapper[] mappers = new vtkImageMapper[3];
        public vtkRenderer[] renderers = new vtkRenderer[4];
        public vtkDICOMImageReader reader;
        public vtkLineSource[] lines2D = new vtkLineSource[6];
        public double[,,] linePos2D = new double[6, 2, 2];
        public vtkLineSource[] lines3D = new vtkLineSource[15];
        public double[,,] linePos3D = new double[15, 2, 3];
        public double height;
        public double width;
        public int[] layers = new int[3];
        public int[] exts = new int[3];
        public int[] offsets = new int[3];
        public int[] motions = new int[3];
        public double[] zooms = new double[3];
        public vtkCamera camera3D;
        public double[] cameraPos = new double[3];
        public double[] cameraFoc = new double[3];
        public double[] cameraView = new double[3];
        public vtkColorTransferFunction color3D;
        private LoadingControl _loadingControl;
        #endregion

        public ImagingWindow(TouchScreenWindow touchScreenWindow, string path)
        {
            InitializeComponent();
            this.touchScreenWindow = touchScreenWindow;
            this.path = path;
            this.Loaded += Window_Loaded;
            Y_Slice.SizeChanged += SliceSizeChanged;
            X_Slice.SizeChanged += SliceSizeChanged;
            Z_Slice.SizeChanged += SliceSizeChanged;
            zooms[0]++;
            zooms[1]++;
            zooms[2]++;
        }

        #region Başlangıç Fonksiyonları
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeVTK(path);
        }

        private void InitializeVTK(string path)
        {
            ReadDicom(path);
            InitYReslice(reader.GetOutputPort());
            InitXReslice(reader.GetOutputPort());
            InitZReslice(reader.GetOutputPort());
            Init3D(reader.GetOutputPort());
            SetPanels();
            Init2DAxesLines();
            Init3DAxesLines();
            InitAspectTexts();
            SetPanels();
            RenderAll();
        }

        private void ReadDicom(string path)
        {
            vtkDICOMImageReader reader = new vtkDICOMImageReader();
            reader.SetDirectoryName(path);
            reader.Update();

            extent = reader.GetDataExtent();
            spacing = reader.GetDataSpacing();
            origin = reader.GetDataOrigin();

            center[0] = origin[0] + ((extent[1] - extent[0]) * spacing[0]) / 2;
            center[1] = origin[1] + ((extent[3] - extent[2]) * spacing[1]) / 2;
            center[2] = origin[2] + ((extent[5] - extent[4]) * spacing[2]) / 2;

            this.reader = reader;
        }

        private void InitYReslice(vtkAlgorithmOutput output)
        {
            var renderWindowControl = new RenderWindowControl();

            renderWindowControl.Load += (sender, args) =>
            {
                vtkImageReslice reslice = new vtkImageReslice();
                reslice.SetInputConnection(output);
                reslice.SetOutputDimensionality(2);
                reslice.SetResliceAxesDirectionCosines(1, 0, 0, 0, 0, 1, 0, 1, 0);
                reslice.SetResliceAxesOrigin(center[0], center[1], center[2]);

                reslices[1] = reslice;

                reslice.GetResliceTransform();

                vtkImageMapper mapper = new vtkImageMapper();
                mapper.SetInputConnection(reslice.GetOutputPort());
                mapper.SetColorWindow(100);
                mapper.SetColorLevel(50);
                mappers[1] = mapper;

                vtkActor2D actor = new vtkActor2D();
                actor.SetMapper(mapper);

                renderers[1] = new vtkRenderer();
                renderers[1].AddActor(actor);
                renderers[1].SetBackground(0.0, 0.0, 0.0);

                renderWindowControl.RenderWindow.AddRenderer(renderers[1]);
                renderWindowControl.RenderWindow.SetMultiSamples(0);
            };
            Y_Slice.Child = renderWindowControl;
        }

        private void InitXReslice(vtkAlgorithmOutput output)
        {
            var renderWindowControl = new RenderWindowControl();

            renderWindowControl.Load += (sender, args) =>
            {
                vtkImageReslice reslice = new vtkImageReslice();
                reslice.SetInputConnection(output);
                reslice.SetOutputDimensionality(2);
                reslice.SetResliceAxesDirectionCosines(0, 1, 0, 0, 0, 1, 1, 0, 0);
                reslice.SetResliceAxesOrigin(center[0], center[1], center[2]);

                reslices[0] = reslice;

                reslice.GetResliceTransform();

                vtkImageMapper mapper = new vtkImageMapper();
                mapper.SetInputConnection(reslice.GetOutputPort());
                mapper.SetColorWindow(100);
                mapper.SetColorLevel(50);
                mappers[0] = mapper;

                vtkActor2D actor = new vtkActor2D();
                actor.SetMapper(mapper);

                renderers[0] = new vtkRenderer();
                renderers[0].AddActor(actor);
                renderers[0].SetBackground(0.0, 0.0, 0.0);

                renderWindowControl.RenderWindow.AddRenderer(renderers[0]);
                renderWindowControl.RenderWindow.SetMultiSamples(0);
            };
            X_Slice.Child = renderWindowControl;
        }

        private void InitZReslice(vtkAlgorithmOutput output)
        {
            var renderWindowControl = new RenderWindowControl();

            renderWindowControl.Load += (sender, args) =>
            {
                vtkImageReslice reslice = new vtkImageReslice();
                reslice.SetInputConnection(output);
                reslice.SetOutputDimensionality(2);
                reslice.SetResliceAxesDirectionCosines(1, 0, 0, 0, 1, 0, 0, 0, 1);
                reslice.SetResliceAxesOrigin(center[0], center[1], center[2]);

                reslices[2] = reslice;

                reslice.GetResliceTransform();

                vtkImageMapper mapper = new vtkImageMapper();
                mapper.SetInputConnection(reslice.GetOutputPort());
                mapper.SetColorWindow(100);
                mapper.SetColorLevel(50);
                mappers[2] = mapper;

                vtkActor2D actor = new vtkActor2D();
                actor.SetMapper(mapper);

                renderers[2] = new vtkRenderer();
                renderers[2].AddActor(actor);
                renderers[2].SetBackground(0.0, 0.0, 0.0);

                renderWindowControl.RenderWindow.AddRenderer(renderers[2]);
                renderWindowControl.RenderWindow.SetMultiSamples(0);
            };
            Z_Slice.Child = renderWindowControl;
        }

        private void Init3D(vtkAlgorithmOutput output)
        {
            var renderWindowControl = new Kitware.VTK.RenderWindowControl();

            renderWindowControl.Load += (sender, args) =>
            {
                vtkSmartVolumeMapper volumeMapper = new vtkSmartVolumeMapper();
                volumeMapper.SetInputConnection(output);

                vtkColorTransferFunction volumeColor = new vtkColorTransferFunction();
                volumeColor.AddRGBPoint(0, 0.0, 0.0, 1.0);
                volumeColor.AddRGBPoint(200, 0.0, 1.0, 0.0);
                volumeColor.AddRGBPoint(1000, 1.0, 0.0, 0.0);
                color3D = volumeColor;

                vtkPiecewiseFunction volumeOpacity = new vtkPiecewiseFunction();
                volumeOpacity.AddPoint(0, 0.0);
                volumeOpacity.AddPoint(100, 0.02);
                volumeOpacity.AddPoint(200, 0.05);
                volumeOpacity.AddPoint(1000, 0.2);

                vtkVolumeProperty volumeProperty = new vtkVolumeProperty();
                volumeProperty.SetColor(volumeColor);
                volumeProperty.SetScalarOpacity(volumeOpacity);
                volumeProperty.ShadeOn();
                volumeProperty.SetInterpolationTypeToLinear();

                // Işık ayarları
                volumeProperty.SetSpecular(0.6);  // Yansıma seviyesi
                volumeProperty.SetSpecularPower(30); // Yansımanın yoğunluğu
                volumeProperty.SetAmbient(0.3);   // Ortam ışığı
                volumeProperty.SetDiffuse(0.9);   // Dağılma oranı

                vtkVolume volume = new vtkVolume();
                volume.SetMapper(volumeMapper);
                volume.SetProperty(volumeProperty);
                

                renderers[3] = new vtkRenderer();
                renderers[3].AddVolume(volume);
                renderers[3].SetBackground(0.0, 0.0, 0.0);

                camera3D = new vtkCamera();
                camera3D = renderers[3].GetActiveCamera();
                
                cameraPos = camera3D.GetPosition();
                cameraFoc = camera3D.GetFocalPoint();
                cameraView = camera3D.GetViewUp();

                renderers[3].ResetCamera();

                renderWindowControl.RenderWindow.AddRenderer(renderers[3]);
                renderWindowControl.RenderWindow.SetMultiSamples(0);
                renderWindowControl.RenderWindow.Render();
            };
            ThreeD.Child = renderWindowControl;
        }

        private void Init2DAxesLines()
        {
            layers[0] = extent[1] / 2;
            layers[1] = extent[3] / 2;
            layers[2] = extent[5] / 2;

            // X Eksen Çizgileri  0 --> Y / 1 --> Z
            linePos2D[0, 0, 0] = (double)layers[0] * oran[1] + offsets[1];
            linePos2D[0, 0, 1] = 0;
            linePos2D[0, 1, 0] = (double)layers[0] * oran[1] + offsets[1];
            linePos2D[0, 1, 1] = height;
            linePos2D[1, 0, 0] = (double)layers[0] * oran[2] + offsets[2];
            linePos2D[1, 0, 1] = 0;
            linePos2D[1, 1, 0] = (double)layers[0] * oran[2] + offsets[2];
            linePos2D[1, 1, 1] = height;
            // Y Eksen Çizgileri 2 --> Z / 3 --> X
            linePos2D[2, 0, 0] = offsets[2];
            linePos2D[2, 0, 1] = (double)layers[1] * oran[2];
            linePos2D[2, 1, 0] = width - offsets[2];
            linePos2D[2, 1, 1] = (double)layers[1] * oran[2];
            linePos2D[3, 0, 0] = (double)layers[1] * oran[0] + offsets[0];
            linePos2D[3, 0, 1] = 0;
            linePos2D[3, 1, 0] = (double)layers[1] * oran[0] + offsets[0];
            linePos2D[3, 1, 1] = height;
            // Z Eksen Çizgileri 4 --> Y / 5 --> X
            linePos2D[4, 0, 0] = offsets[1];
            linePos2D[4, 0, 1] = (double)layers[2] * oran[1];
            linePos2D[4, 1, 0] = width - offsets[1];
            linePos2D[4, 1, 1] = (double)layers[2] * oran[1];
            linePos2D[5, 0, 0] = offsets[0];
            linePos2D[5, 0, 1] = (double)layers[2] * oran[0];
            linePos2D[5, 1, 0] = width - offsets[0];
            linePos2D[5, 1, 1] = (double)layers[2] * oran[0];
            for (int i = 0; i < 6; i++)
            {
                lines2D[i] = new vtkLineSource();
                lines2D[i].SetPoint1(linePos2D[i, 0, 0], linePos2D[i, 0, 1], 0);
                lines2D[i].SetPoint2(linePos2D[i, 1, 0], linePos2D[i, 1, 1], 0);
            }
            for (int i = 0; i < 6; i++)
            {
                vtkPolyDataMapper2D lineMapper = new vtkPolyDataMapper2D();
                vtkActor2D lineActor = new vtkActor2D();
                lineMapper.SetInputConnection(lines2D[i].GetOutputPort());
                lineActor.SetMapper(lineMapper);
                lineActor.GetProperty().SetLineWidth(1);
                // Renk Ayarlama
                if (i/2 == 0)
                {
                    lineActor.GetProperty().SetColor(1, 0, 0);
                }
                else if (i / 2 == 1)
                {
                    lineActor.GetProperty().SetColor(0, 1, 0);
                }
                else if (i / 2 == 2)
                {
                    lineActor.GetProperty().SetColor(0, 0, 1);
                }
                // Panele Ekleme
                if (i == 3 || i == 5)
                {
                    renderers[0].AddActor(lineActor);
                }
                else if (i == 0 || i == 4)
                {
                    renderers[1].AddActor(lineActor);
                }
                else if (i == 1 || i == 2)
                {
                    renderers[2].AddActor(lineActor);
                }
            }
        }

        private void Init3DAxesLines()
        {
            // X Eksenleri
            linePos3D[0, 0, 0] = layers[0] * spacing[0];
            linePos3D[0, 1, 0] = layers[0] * spacing[0];
            linePos3D[0, 0, 2] = 0;
            linePos3D[0, 1, 2] = 0;
            linePos3D[0, 0, 1] = 0;
            linePos3D[0, 1, 1] = extent[3] * spacing[1];
            linePos3D[1, 0, 0] = layers[0] * spacing[0];
            linePos3D[1, 1, 0] = layers[0] * spacing[0];
            linePos3D[1, 0, 2] = extent[5] * spacing[2];
            linePos3D[1, 1, 2] = extent[5] * spacing[2];
            linePos3D[1, 0, 1] = 0;
            linePos3D[1, 1, 1] = extent[3] * spacing[1];
            linePos3D[2, 0, 0] = layers[0] * spacing[0];
            linePos3D[2, 1, 0] = layers[0] * spacing[0];
            linePos3D[2, 0, 2] = 0;
            linePos3D[2, 1, 2] = extent[5] * spacing[2];
            linePos3D[2, 0, 1] = 0;
            linePos3D[2, 1, 1] = 0;
            linePos3D[3, 0, 0] = layers[0] * spacing[0];
            linePos3D[3, 1, 0] = layers[0] * spacing[0];
            linePos3D[3, 0, 2] = 0;
            linePos3D[3, 1, 2] = extent[5] * spacing[2];
            linePos3D[3, 0, 1] = extent[3] * spacing[1];
            linePos3D[3, 1, 1] = extent[3] * spacing[1];
            // Y Eksenleri
            linePos3D[4, 0, 0] = 0;
            linePos3D[4, 1, 0] = extent[1] * spacing[0];
            linePos3D[4, 0, 2] = 0;
            linePos3D[4, 1, 2] = 0;
            linePos3D[4, 0, 1] = layers[1] * spacing[1];
            linePos3D[4, 1, 1] = layers[1] * spacing[1];
            linePos3D[5, 0, 0] = extent[1] * spacing[0];
            linePos3D[5, 1, 0] = extent[1] * spacing[0];
            linePos3D[5, 0, 2] = 0;
            linePos3D[5, 1, 2] = extent[5] * spacing[2];
            linePos3D[5, 0, 1] = layers[1] * spacing[1];
            linePos3D[5, 1, 1] = layers[1] * spacing[1];
            linePos3D[6, 0, 0] = extent[1] * spacing[0];
            linePos3D[6, 1, 0] = 0;
            linePos3D[6, 0, 2] = extent[5] * spacing[2];
            linePos3D[6, 1, 2] = extent[5] * spacing[2];
            linePos3D[6, 0, 1] = layers[1] * spacing[1];
            linePos3D[6, 1, 1] = layers[1] * spacing[1];
            linePos3D[7, 0, 0] = 0;
            linePos3D[7, 1, 0] = 0;
            linePos3D[7, 0, 2] = extent[5] * spacing[2];
            linePos3D[7, 1, 2] = 0;
            linePos3D[7, 0, 1] = layers[1] * spacing[1];
            linePos3D[7, 1, 1] = layers[1] * spacing[1];
            // Z Eksenleri
            linePos3D[8, 0, 0] = 0;
            linePos3D[8, 1, 0] = 0;
            linePos3D[8, 0, 2] = layers[2] * spacing[2];
            linePos3D[8, 1, 2] = layers[2] * spacing[2];
            linePos3D[8, 0, 1] = 0;
            linePos3D[8, 1, 1] = extent[3] * spacing[1];
            linePos3D[9, 0, 0] = extent[1] * spacing[0];
            linePos3D[9, 1, 0] = extent[1] * spacing[0];
            linePos3D[9, 0, 2] = layers[2] * spacing[2];
            linePos3D[9, 1, 2] = layers[2] * spacing[2];
            linePos3D[9, 0, 1] = 0;
            linePos3D[9, 1, 1] = extent[3] * spacing[1];
            linePos3D[10, 0, 0] = 0;
            linePos3D[10, 1, 0] = extent[1] * spacing[0];
            linePos3D[10, 0, 2] = layers[2] * spacing[2];
            linePos3D[10, 1, 2] = layers[2] * spacing[2];
            linePos3D[10, 0, 1] = 0;
            linePos3D[10, 1, 1] = 0;
            linePos3D[11, 0, 0] = 0;
            linePos3D[11, 1, 0] = extent[1] * spacing[0];
            linePos3D[11, 0, 2] = layers[2] * spacing[2];
            linePos3D[11, 1, 2] = layers[2] * spacing[2];
            linePos3D[11, 0, 1] = extent[3] * spacing[1];
            linePos3D[11, 1, 1] = extent[3] * spacing[1];
            // Ortak Nokta Çizgileri
            // Y-Z Ortak
            linePos3D[12, 0, 0] = 0;
            linePos3D[12, 1, 0] = extent[1] * spacing[0];
            linePos3D[12, 0, 2] = layers[2] * spacing[2];
            linePos3D[12, 1, 2] = layers[2] * spacing[2];
            linePos3D[12, 0, 1] = layers[1] * spacing[1];
            linePos3D[12, 1, 1] = layers[1] * spacing[1];
            // X-Z Ortak
            linePos3D[13, 0, 0] = layers[0] * spacing[0];
            linePos3D[13, 1, 0] = layers[0] * spacing[0];
            linePos3D[13, 0, 2] = layers[2] * spacing[2];
            linePos3D[13, 1, 2] = layers[2] * spacing[2];
            linePos3D[13, 0, 1] = 0;
            linePos3D[13, 1, 1] = extent[3] * spacing[1];
            // X-Y Ortak
            linePos3D[14, 0, 0] = layers[0] * spacing[0];
            linePos3D[14, 1, 0] = layers[0] * spacing[0];
            linePos3D[14, 0, 1] = layers[1] * spacing[1];
            linePos3D[14, 1, 1] = layers[1] * spacing[1];
            linePos3D[14, 0, 2] = 0;
            linePos3D[14, 1, 2] = extent[5] * spacing[2];
            for (int i = 0; i < 15; i++)
            {
                lines3D[i] = new vtkLineSource();
                lines3D[i].SetPoint1(linePos3D[i, 0, 0], linePos3D[i, 0, 1], linePos3D[i, 0, 2]);
                lines3D[i].SetPoint2(linePos3D[i, 1, 0], linePos3D[i, 1, 1], linePos3D[i, 1, 2]);
            }
            for (int i = 0; i < 15; i++)
            {
                vtkPolyDataMapper lineMapper = new vtkPolyDataMapper();
                vtkActor lineActor = new vtkActor();
                lineMapper.SetInputConnection(lines3D[i].GetOutputPort());
                lineActor.SetMapper(lineMapper);
                if (i / 4 == 0)
                {
                    lineActor.GetProperty().SetColor(1, 0, 0);
                }
                else if (i / 4 == 1)
                {
                    lineActor.GetProperty().SetColor(0, 1, 0);
                }
                else if (i / 4 == 2)
                {
                    lineActor.GetProperty().SetColor(0, 0, 1);
                }
                else if (i == 12)
                {
                    lineActor.GetProperty().SetColor(0, 1, 1);
                }
                else if (i == 13)
                {
                    lineActor.GetProperty().SetColor(1, 0, 1);
                }
                else if (i == 14)
                {
                    lineActor.GetProperty().SetColor(1, 1, 0);
                }
                renderers[3].AddActor(lineActor);
            }
        }

        private void InitAspectTexts()
        {
            vtkTextActor[] aspects = new vtkTextActor[12];
            for (int i = 0;i < 12;i++)
            {
                aspects[i] = vtkTextActor.New();
                aspects[i].SetTextScaleModeToNone();
                aspects[i].GetPositionCoordinate().SetCoordinateSystemToNormalizedDisplay();
                aspects[i].GetTextProperty().SetFontFamilyToCourier(); ;
                aspects[i].GetTextProperty().SetFontSize(24);
                aspects[i].GetTextProperty().SetColor(1, 1, 1);
            }
            aspects[0].SetPosition(0.05, 0.93);
            aspects[0].SetInput("P");
            aspects[1].SetPosition(0.05, 0.83);
            aspects[1].SetInput("A");
            aspects[2].SetPosition(0.02, 0.88);
            aspects[2].SetInput("L");
            aspects[3].SetPosition(0.08, 0.88);
            aspects[3].SetInput("R");

            aspects[4].SetPosition(0.93, 0.93);
            aspects[4].SetInput("S");
            aspects[5].SetPosition(0.93, 0.83);
            aspects[5].SetInput("I");
            aspects[6].SetPosition(0.96, 0.88);
            aspects[6].SetInput("A");
            aspects[7].SetPosition(0.90, 0.88);
            aspects[7].SetInput("P");

            aspects[8].SetPosition(0.05, 0.93);
            aspects[8].SetInput("S");
            aspects[9].SetPosition(0.05, 0.83);
            aspects[9].SetInput("I");
            aspects[10].SetPosition(0.02, 0.88);
            aspects[10].SetInput("L");
            aspects[11].SetPosition(0.08, 0.88);
            aspects[11].SetInput("R");
            for (int i = 0; i < 12; i++)
            {
                if (i / 4 == 0)
                {
                    renderers[1].AddActor(aspects[i]);
                }
                else if (i / 4 == 1)
                {
                    renderers[0].AddActor(aspects[i]);
                }
                else if (i / 4 == 2)
                {
                    renderers[2].AddActor(aspects[i]);
                }
            }
        }
        #endregion

        #region Araçlar
        public void SetPanels()
        {
            double dpiScale = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            width = YBorder.ActualWidth * dpiScale;
            height = YBorder.ActualHeight * dpiScale;

            oran[0] = height / (double)extent[3];
            reslices[0].SetOutputExtent((int)(motions[1] * oran[0] * zooms[0]) - (int)((zooms[0] - 1) * (width / 2)),
                (int)(width + motions[1] * oran[0] * zooms[0]) + (int)((zooms[0] - 1) * (width / 2)), 
                (int)(motions[2] * oran[0] * zooms[0]) - (int)((zooms[0] - 1) * (height / 2)), 
                (int)(height + motions[2] * oran[0] * zooms[0]) + (int)((zooms[0] - 1) * (height / 2)), 0, 0);
            reslices[0].SetOutputSpacing(spacing[0] / oran[0] / zooms[0], spacing[1] / oran[0] / zooms[0], spacing[2] / oran[0] / zooms[0]);
            
            oran[1] = height / (double)extent[5];
            reslices[1].SetOutputExtent((int)(motions[0] * oran[1] * zooms[1]) - (int)((zooms[1] - 1) * (width / 2)), 
                (int)(width + motions[0] * oran[1] * zooms[1]) + (int)((zooms[1] - 1) * (width / 2)), 
                (int)(motions[2] * oran[1] * zooms[1]) - (int)((zooms[1] - 1) * (height / 2)), 
                (int)(height + motions[2] * oran[1] * zooms[1]) + (int)((zooms[1] - 1) * (height / 2)), 0, 0);
            reslices[1].SetOutputSpacing(spacing[0] / oran[1] / zooms[1], spacing[1] / oran[1] / zooms[1], spacing[2] / oran[1] / zooms[1]);
            
            oran[2] = height / (double)extent[3];
            reslices[2].SetOutputExtent((int)(motions[0] * oran[2] * zooms[2]) - (int)((zooms[2] - 1) * (width / 2)), 
                (int)(width + motions[0] * oran[2] * zooms[2]) + (int)((zooms[2] - 1) * (width / 2)), 
                (int)(motions[1] * oran[2] * zooms[2]) - (int)((zooms[2] - 1) * (height / 2)), 
                (int)(height + motions[1] * oran[2] * zooms[2]) + (int)((zooms[2] - 1) * (height / 2)), 0, 0);
            reslices[2].SetOutputSpacing(spacing[0] / oran[2] / zooms[2], spacing[1] / oran[2] / zooms[2], spacing[2] / oran[2] / zooms[2]);

            exts[0] = extent[5] * (int)height / extent[3];
            offsets[0] = ((int)width - exts[0]) / 2;
            exts[1] = extent[1] * (int)height / extent[5];
            offsets[1] = ((int)width - exts[1]) / 2;
            exts[2] = extent[1] * (int)height / extent[3];
            offsets[2] = ((int)width - exts[2]) / 2;
        }

        public void RenderAll()
        {
            Y_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(Y_Slice.Child)?.GetType().GetMethod("Render")?.Invoke(Y_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(Y_Slice.Child), null);
            X_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(X_Slice.Child)?.GetType().GetMethod("Render")?.Invoke(X_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(X_Slice.Child), null);
            Z_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(Z_Slice.Child)?.GetType().GetMethod("Render")?.Invoke(Z_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(Z_Slice.Child), null);
            ThreeD.Child.GetType().GetProperty("RenderWindow")?.GetValue(ThreeD.Child)?.GetType().GetMethod("Render")?.Invoke(ThreeD.Child.GetType().GetProperty("RenderWindow")?.GetValue(ThreeD.Child), null);
        }

        public void RenderReslices(int i = 3)
        {
            if (i == 0)
            {
                X_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(X_Slice.Child)?.GetType().GetMethod("Render")?.Invoke(X_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(X_Slice.Child), null);
            }
            else if (i == 1)
            {
                Y_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(Y_Slice.Child)?.GetType().GetMethod("Render")?.Invoke(Y_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(Y_Slice.Child), null);
            }
            else if (i == 2)
            {
                Z_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(Z_Slice.Child)?.GetType().GetMethod("Render")?.Invoke(Z_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(Z_Slice.Child), null);
            }
            else
            {
                Y_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(Y_Slice.Child)?.GetType().GetMethod("Render")?.Invoke(Y_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(Y_Slice.Child), null);
                X_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(X_Slice.Child)?.GetType().GetMethod("Render")?.Invoke(X_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(X_Slice.Child), null);
                Z_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(Z_Slice.Child)?.GetType().GetMethod("Render")?.Invoke(Z_Slice.Child.GetType().GetProperty("RenderWindow")?.GetValue(Z_Slice.Child), null);
            }
        }

        public async void Render3D()
        {
            ThreeD.Child.GetType().GetProperty("RenderWindow")?.GetValue(ThreeD.Child)?.GetType().GetMethod("Render")?.Invoke(ThreeD.Child.GetType().GetProperty("RenderWindow")?.GetValue(ThreeD.Child), null);
        }
        #endregion

        private void SliceSizeChanged(object sender, EventArgs e)
        {
            SetPanels();
            XLayer(0);
            YLayer(0);
            ZLayer(0);
            if (touchScreenWindow.state == State.NOR)
            {
                RenderAll();
            }
            else
            {
                if (touchScreenWindow.panel == Panels.X_PANEL)
                {
                    RenderReslices(0);
                }
                else if (touchScreenWindow.panel == Panels.Y_PANEL)
                {
                    RenderReslices(1);
                }
                else if (touchScreenWindow.panel == Panels.Z_PANEL)
                {
                    RenderReslices(2);
                }
                else if (touchScreenWindow.panel == Panels.THREED)
                {
                    Render3D();
                }
            }
        }

        #region Layer Değişim Fonksiyonları
        public void XLayer(int isUp)
        {
            layers[0] += isUp;
            reslices[0].SetResliceAxesOrigin((double)layers[0] * spacing[0], center[1], center[2]);
            // 2D
            linePos2D[0, 0, 0] = (double)(layers[0] + motions[0]) * oran[1] + offsets[1];
            linePos2D[0, 0, 1] = ((double)motions[2] * oran[1] * zooms[1]) - (height / 2 * (zooms[1] - 1));
            linePos2D[0, 1, 0] = (double)(layers[0] + motions[0]) * oran[1] + offsets[1];
            linePos2D[0, 1, 1] = ((double)motions[2] * oran[1] * zooms[1]) + height + (height / 2 * (zooms[1] - 1));
            linePos2D[1, 0, 0] = (double)(layers[0] + motions[0]) * oran[2] + offsets[2];
            linePos2D[1, 0, 1] = ((double)motions[1] * oran[2] * zooms[2]) - (height / 2 * (zooms[2] - 1));
            linePos2D[1, 1, 0] = (double)(layers[0] + motions[0]) * oran[2] + offsets[2];
            linePos2D[1, 1, 1] = ((double)motions[1] * oran[2] * zooms[2]) + height + (height / 2 * (zooms[2] - 1));
            lines2D[0].SetPoint1(linePos2D[0, 0, 0], linePos2D[0, 0, 1], 0);
            lines2D[0].SetPoint2(linePos2D[0, 1, 0], linePos2D[0, 1, 1], 0);
            lines2D[1].SetPoint1(linePos2D[1, 0, 0], linePos2D[1, 0, 1], 0);
            lines2D[1].SetPoint2(linePos2D[1, 1, 0], linePos2D[1, 1, 1], 0);
            // 3D
            linePos3D[0, 0, 0] = layers[0] * spacing[0];
            linePos3D[0, 1, 0] = layers[0] * spacing[0];
            linePos3D[1, 0, 0] = layers[0] * spacing[0];
            linePos3D[1, 1, 0] = layers[0] * spacing[0];
            linePos3D[2, 0, 0] = layers[0] * spacing[0];
            linePos3D[2, 1, 0] = layers[0] * spacing[0];
            linePos3D[3, 0, 0] = layers[0] * spacing[0];
            linePos3D[3, 1, 0] = layers[0] * spacing[0];
            linePos3D[13, 0, 0] = layers[0] * spacing[0];
            linePos3D[13, 1, 0] = layers[0] * spacing[0];
            linePos3D[14, 0, 0] = layers[0] * spacing[0];
            linePos3D[14, 1, 0] = layers[0] * spacing[0];
            lines3D[0].SetPoint1(linePos3D[0, 0, 0], linePos3D[0, 0, 1], linePos3D[0, 0, 2]);
            lines3D[0].SetPoint2(linePos3D[0, 1, 0], linePos3D[0, 1, 1], linePos3D[0, 1, 2]);
            lines3D[1].SetPoint1(linePos3D[1, 0, 0], linePos3D[1, 0, 1], linePos3D[1, 0, 2]);
            lines3D[1].SetPoint2(linePos3D[1, 1, 0], linePos3D[1, 1, 1], linePos3D[1, 1, 2]);
            lines3D[2].SetPoint1(linePos3D[2, 0, 0], linePos3D[2, 0, 1], linePos3D[2, 0, 2]);
            lines3D[2].SetPoint2(linePos3D[2, 1, 0], linePos3D[2, 1, 1], linePos3D[2, 1, 2]);
            lines3D[3].SetPoint1(linePos3D[3, 0, 0], linePos3D[3, 0, 1], linePos3D[3, 0, 2]);
            lines3D[3].SetPoint2(linePos3D[3, 1, 0], linePos3D[3, 1, 1], linePos3D[3, 1, 2]);
            lines3D[14].SetPoint1(linePos3D[14, 0, 0], linePos3D[14, 0, 1], linePos3D[14, 0, 2]);
            lines3D[14].SetPoint2(linePos3D[14, 1, 0], linePos3D[14, 1, 1], linePos3D[14, 1, 2]);
            lines3D[13].SetPoint1(linePos3D[13, 0, 0], linePos3D[13, 0, 1], linePos3D[13, 0, 2]);
            lines3D[13].SetPoint2(linePos3D[13, 1, 0], linePos3D[13, 1, 1], linePos3D[13, 1, 2]);
        }

        public void YLayer(int isUp)
        {
            layers[1] += isUp;
            reslices[1].SetResliceAxesOrigin(center[0], (double)layers[1] * spacing[1], center[2]);
            // 2D
            linePos2D[2, 0, 0] = (offsets[2] + (double)motions[0] * oran[2]) * zooms[2] - ((width / 2) * (zooms[2] - 1));
            linePos2D[2, 0, 1] = (double)(layers[1] + motions[1]) * oran[2];
            linePos2D[2, 1, 0] = width + ((width / 2) * (zooms[2] - 1)) - (offsets[2] * zooms[2]) + ((double)motions[0] * oran[2] * zooms[2]);
            linePos2D[2, 1, 1] = (double)(layers[1] + motions[1]) * oran[2];
            linePos2D[3, 0, 0] = (double)(layers[1] + motions[1]) * oran[0] + offsets[0];
            linePos2D[3, 0, 1] = ((double)motions[2] * oran[0] * zooms[0]) - (height / 2 * (zooms[0] - 1));
            linePos2D[3, 1, 0] = (double)(layers[1] + motions[1]) * oran[0] + offsets[0];
            linePos2D[3, 1, 1] = ((double)motions[2] * oran[0] * zooms[0]) + height + (height / 2 * (zooms[0] - 1));
            lines2D[2].SetPoint1(linePos2D[2, 0, 0], linePos2D[2, 0, 1], 0);
            lines2D[2].SetPoint2(linePos2D[2, 1, 0], linePos2D[2, 1, 1], 0);
            lines2D[3].SetPoint1(linePos2D[3, 0, 0], linePos2D[3, 0, 1], 0);
            lines2D[3].SetPoint2(linePos2D[3, 1, 0], linePos2D[3, 1, 1], 0);
            // 3D
            linePos3D[4, 0, 1] = layers[1] * spacing[1];
            linePos3D[4, 1, 1] = layers[1] * spacing[1];
            linePos3D[5, 0, 1] = layers[1] * spacing[1];
            linePos3D[5, 1, 1] = layers[1] * spacing[1];
            linePos3D[6, 0, 1] = layers[1] * spacing[1];
            linePos3D[6, 1, 1] = layers[1] * spacing[1];
            linePos3D[7, 0, 1] = layers[1] * spacing[1];
            linePos3D[7, 1, 1] = layers[1] * spacing[1];
            linePos3D[12, 0, 1] = layers[1] * spacing[1];
            linePos3D[12, 1, 1] = layers[1] * spacing[1];
            linePos3D[14, 0, 1] = layers[1] * spacing[1];
            linePos3D[14, 1, 1] = layers[1] * spacing[1];
            lines3D[4].SetPoint1(linePos3D[4, 0, 0], linePos3D[4, 0, 1], linePos3D[4, 0, 2]);
            lines3D[4].SetPoint2(linePos3D[4, 1, 0], linePos3D[4, 1, 1], linePos3D[4, 1, 2]);
            lines3D[5].SetPoint1(linePos3D[5, 0, 0], linePos3D[5, 0, 1], linePos3D[5, 0, 2]);
            lines3D[5].SetPoint2(linePos3D[5, 1, 0], linePos3D[5, 1, 1], linePos3D[5, 1, 2]);
            lines3D[6].SetPoint1(linePos3D[6, 0, 0], linePos3D[6, 0, 1], linePos3D[6, 0, 2]);
            lines3D[6].SetPoint2(linePos3D[6, 1, 0], linePos3D[6, 1, 1], linePos3D[6, 1, 2]);
            lines3D[7].SetPoint1(linePos3D[7, 0, 0], linePos3D[7, 0, 1], linePos3D[7, 0, 2]);
            lines3D[7].SetPoint2(linePos3D[7, 1, 0], linePos3D[7, 1, 1], linePos3D[7, 1, 2]);
            lines3D[12].SetPoint1(linePos3D[12, 0, 0], linePos3D[12, 0, 1], linePos3D[12, 0, 2]);
            lines3D[12].SetPoint2(linePos3D[12, 1, 0], linePos3D[12, 1, 1], linePos3D[12, 1, 2]);
            lines3D[14].SetPoint1(linePos3D[14, 0, 0], linePos3D[14, 0, 1], linePos3D[14, 0, 2]);
            lines3D[14].SetPoint2(linePos3D[14, 1, 0], linePos3D[14, 1, 1], linePos3D[14, 1, 2]);
        }

        public void ZLayer(int isUp)
        {
            layers[2] += isUp;
            reslices[2].SetResliceAxesOrigin(center[0], center[1], (double)layers[2] * spacing[2]);
            // 2D
            linePos2D[4, 0, 0] = (offsets[1] + (double)motions[0] * oran[1]) * zooms[1] - ((width / 2) * (zooms[1] - 1));
            linePos2D[4, 0, 1] = (double)(layers[2] + motions[2]) * oran[1];
            linePos2D[4, 1, 0] = width + ((width / 2) * (zooms[1] - 1)) - (offsets[1] * zooms[1]) + ((double)motions[0] * oran[1] * zooms[1]);
            linePos2D[4, 1, 1] = (double)(layers[2] + motions[2]) * oran[1];
            linePos2D[5, 0, 0] = (offsets[0] + (double)motions[1] * oran[0]) * zooms[0] - ((width / 2) * (zooms[0] - 1));
            linePos2D[5, 0, 1] = (double)(layers[2] + motions[2]) * oran[0];
            linePos2D[5, 1, 0] = width + ((width / 2) * (zooms[0] - 1)) - (offsets[0] * zooms[0]) + ((double)motions[1] * oran[0] * zooms[0]);
            linePos2D[5, 1, 1] = (double)(layers[2] + motions[2]) * oran[0];
            lines2D[4].SetPoint1(linePos2D[4, 0, 0], linePos2D[4, 0, 1], 0);
            lines2D[4].SetPoint2(linePos2D[4, 1, 0], linePos2D[4, 1, 1], 0);
            lines2D[5].SetPoint1(linePos2D[5, 0, 0], linePos2D[5, 0, 1], 0);
            lines2D[5].SetPoint2(linePos2D[5, 1, 0], linePos2D[5, 1, 1], 0);
            // 3D
            linePos3D[8, 0, 2] = layers[2] * spacing[2];
            linePos3D[8, 1, 2] = layers[2] * spacing[2];
            linePos3D[9, 0, 2] = layers[2] * spacing[2];
            linePos3D[9, 1, 2] = layers[2] * spacing[2];
            linePos3D[10, 0, 2] = layers[2] * spacing[2];
            linePos3D[10, 1, 2] = layers[2] * spacing[2];
            linePos3D[11, 0, 2] = layers[2] * spacing[2];
            linePos3D[11, 1, 2] = layers[2] * spacing[2];
            linePos3D[12, 0, 2] = layers[2] * spacing[2];
            linePos3D[12, 1, 2] = layers[2] * spacing[2];
            linePos3D[13, 0, 2] = layers[2] * spacing[2];
            linePos3D[13, 1, 2] = layers[2] * spacing[2];
            lines3D[8].SetPoint1(linePos3D[8, 0, 0], linePos3D[8, 0, 1], linePos3D[8, 0, 2]);
            lines3D[8].SetPoint2(linePos3D[8, 1, 0], linePos3D[8, 1, 1], linePos3D[8, 1, 2]);
            lines3D[9].SetPoint1(linePos3D[9, 0, 0], linePos3D[9, 0, 1], linePos3D[9, 0, 2]);
            lines3D[9].SetPoint2(linePos3D[9, 1, 0], linePos3D[9, 1, 1], linePos3D[9, 1, 2]);
            lines3D[10].SetPoint1(linePos3D[10, 0, 0], linePos3D[10, 0, 1], linePos3D[10, 0, 2]);
            lines3D[10].SetPoint2(linePos3D[10, 1, 0], linePos3D[10, 1, 1], linePos3D[10, 1, 2]);
            lines3D[11].SetPoint1(linePos3D[11, 0, 0], linePos3D[11, 0, 1], linePos3D[11, 0, 2]);
            lines3D[11].SetPoint2(linePos3D[11, 1, 0], linePos3D[11, 1, 1], linePos3D[11, 1, 2]);
            lines3D[12].SetPoint1(linePos3D[12, 0, 0], linePos3D[12, 0, 1], linePos3D[12, 0, 2]);
            lines3D[12].SetPoint2(linePos3D[12, 1, 0], linePos3D[12, 1, 1], linePos3D[12, 1, 2]);
            lines3D[13].SetPoint1(linePos3D[13, 0, 0], linePos3D[13, 0, 1], linePos3D[13, 0, 2]);
            lines3D[13].SetPoint2(linePos3D[13, 1, 0], linePos3D[13, 1, 1], linePos3D[13, 1, 2]);
        }
        #endregion

        #region Motion Değişim Fonksiyonları
        public void XLayerMotion(int isUp)
        {
            motions[0] -= isUp;
            XLayer(isUp);
            YLayer(0);
            ZLayer(0);
            SetPanels();
            if (touchScreenWindow.state == State.NOR)
            {
                RenderAll();
            }
            else
            {
                if (touchScreenWindow.panel == Panels.X_PANEL)
                {
                    RenderReslices(0);
                }
                else if (touchScreenWindow.panel == Panels.Y_PANEL)
                {
                    RenderReslices(1);
                }
                else if (touchScreenWindow.panel == Panels.Z_PANEL)
                {
                    RenderReslices(2);
                }
            }
            camera3D.SetFocalPoint(layers[0] * spacing[0], layers[1] * spacing[1], layers[2] * spacing[2]);
        }

        public void YLayerMotion(int isUp)
        {
            motions[1] -= isUp;
            YLayer(isUp);
            XLayer(0);
            ZLayer(0);
            SetPanels();
            if (touchScreenWindow.state == State.NOR)
            {
                RenderAll();
            }
            else
            {
                if (touchScreenWindow.panel == Panels.X_PANEL)
                {
                    RenderReslices(0);
                }
                else if (touchScreenWindow.panel == Panels.Y_PANEL)
                {
                    RenderReslices(1);
                }
                else if (touchScreenWindow.panel == Panels.Z_PANEL)
                {
                    RenderReslices(2);
                }
            }
            camera3D.SetFocalPoint(layers[0] * spacing[0], layers[1] * spacing[1], layers[2] * spacing[2]);
        }

        public void ZLayerMotion(int isUp)
        {
            motions[2] -= isUp;
            ZLayer(isUp);
            YLayer(0);
            XLayer(0);
            SetPanels();
            if (touchScreenWindow.state == State.NOR)
            {
                RenderAll();
            }
            else
            {
                if (touchScreenWindow.panel == Panels.X_PANEL)
                {
                    RenderReslices(0);
                }
                else if (touchScreenWindow.panel == Panels.Y_PANEL)
                {
                    RenderReslices(1);
                }
                else if (touchScreenWindow.panel == Panels.Z_PANEL)
                {
                    RenderReslices(2);
                }
            }
            camera3D.SetFocalPoint(layers[0] * spacing[0], layers[1] * spacing[1], layers[2] * spacing[2]);
        }
        #endregion
    }
}
