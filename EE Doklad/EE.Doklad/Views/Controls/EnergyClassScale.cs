using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace EE.Doklad.Views.Controls
{
    /// <summary>
    /// Визуална скала A-G с 7 цветни ленти и маркер
    /// </summary>
    public class EnergyClassScale : Control
    {
        private Canvas? _scaleCanvas;
        private Canvas? _markerCanvas;

        static EnergyClassScale()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnergyClassScale), 
                new FrameworkPropertyMetadata(typeof(EnergyClassScale)));
            
            // Create default template
            var template = new ControlTemplate(typeof(EnergyClassScale));
            
            var factory = new FrameworkElementFactory(typeof(Grid));
            
            // Scale canvas (background)
            var scaleCanvasFactory = new FrameworkElementFactory(typeof(Canvas));
            scaleCanvasFactory.SetValue(NameProperty, "PART_ScaleCanvas");
            scaleCanvasFactory.SetValue(Panel.ZIndexProperty, 0);
            factory.AppendChild(scaleCanvasFactory);
            
            // Marker canvas (foreground overlay)
            var markerCanvasFactory = new FrameworkElementFactory(typeof(Canvas));
            markerCanvasFactory.SetValue(NameProperty, "PART_MarkerCanvas");
            markerCanvasFactory.SetValue(Panel.ZIndexProperty, 10);
            markerCanvasFactory.SetValue(Canvas.IsHitTestVisibleProperty, false);
            factory.AppendChild(markerCanvasFactory);
            
            template.VisualTree = factory;
            
            var style = new Style(typeof(EnergyClassScale));
            style.Setters.Add(new Setter(TemplateProperty, template));
            style.Setters.Add(new Setter(MinWidthProperty, 250.0));
            style.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
            style.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Top));
            
            StyleProperty.OverrideMetadata(typeof(EnergyClassScale), 
                new FrameworkPropertyMetadata(style));
        }

        #region Dependency Properties

        public static readonly DependencyProperty BandHeightProperty =
            DependencyProperty.Register(nameof(BandHeight), typeof(double), typeof(EnergyClassScale),
                new PropertyMetadata(60.0, OnVisualPropertyChanged));

        public static readonly DependencyProperty MarkerValueProperty =
            DependencyProperty.Register(nameof(MarkerValue), typeof(int?), typeof(EnergyClassScale),
                new PropertyMetadata(null, OnVisualPropertyChanged));

        public static readonly DependencyProperty NormalizedMarkerPositionProperty =
            DependencyProperty.Register(nameof(NormalizedMarkerPosition), typeof(double?), typeof(EnergyClassScale),
                new PropertyMetadata(null, OnVisualPropertyChanged));

        public static readonly DependencyProperty CurrentClassProperty =
            DependencyProperty.Register(nameof(CurrentClass), typeof(string), typeof(EnergyClassScale),
                new PropertyMetadata("—", OnVisualPropertyChanged));

        public double BandHeight
        {
            get => (double)GetValue(BandHeightProperty);
            set => SetValue(BandHeightProperty, value);
        }

        public int? MarkerValue
        {
            get => (int?)GetValue(MarkerValueProperty);
            set => SetValue(MarkerValueProperty, value);
        }

        public double? NormalizedMarkerPosition
        {
            get => (double?)GetValue(NormalizedMarkerPositionProperty);
            set => SetValue(NormalizedMarkerPositionProperty, value);
        }

        public string CurrentClass
        {
            get => (string)GetValue(CurrentClassProperty);
            set => SetValue(CurrentClassProperty, value);
        }

        private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EnergyClassScale scale)
            {
                scale.RedrawScale();
            }
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _scaleCanvas = GetTemplateChild("PART_ScaleCanvas") as Canvas;
            _markerCanvas = GetTemplateChild("PART_MarkerCanvas") as Canvas;

            SizeChanged += (s, e) => RedrawScale();
            Loaded += (s, e) => RedrawScale();

            RedrawScale();
        }

        private void RedrawScale()
        {
            if (_scaleCanvas == null || _markerCanvas == null)
                return;

            _scaleCanvas.Children.Clear();
            _markerCanvas.Children.Clear();

            DrawBands();
            DrawMarker();
        }

        private void DrawBands()
        {
            if (_scaleCanvas == null)
                return;

            var classes = new[] { "A", "B", "C", "D", "E", "F", "G" };
            var colors = new[]
            {
                "#00C853", // A - Dark Green
                "#64DD17", // B - Light Green
                "#C6FF00", // C - Lime
                "#FFD600", // D - Yellow
                "#FFAB00", // E - Amber
                "#FF6D00", // F - Orange
                "#DD2C00"  // G - Red
            };

            double width = ActualWidth > 0 ? ActualWidth - 20 : 280;
            if (width <= 0) width = 280;
            
            double y = 0;

            for (int i = 0; i < classes.Length; i++)
            {
                // Лента с цвят
                var band = new Rectangle
                {
                    Width = width,
                    Height = BandHeight,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors[i])),
                    Stroke = new SolidColorBrush(Colors.White),
                    StrokeThickness = 2
                };

                Canvas.SetLeft(band, 0);
                Canvas.SetTop(band, y);
                _scaleCanvas.Children.Add(band);

                // Текст с класа
                var text = new TextBlock
                {
                    Text = classes[i],
                    FontSize = 28,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.White),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                Canvas.SetLeft(text, 15);
                Canvas.SetTop(text, y + (BandHeight - 35) / 2);
                _scaleCanvas.Children.Add(text);

                // Highlight current class
                if (classes[i] == CurrentClass && CurrentClass != "—")
                {
                    var highlight = new Rectangle
                    {
                        Width = width,
                        Height = BandHeight,
                        Stroke = new SolidColorBrush(Colors.Black),
                        StrokeThickness = 4,
                        Fill = Brushes.Transparent
                    };

                    Canvas.SetLeft(highlight, 0);
                    Canvas.SetTop(highlight, y);
                    _scaleCanvas.Children.Add(highlight);
                }

                y += BandHeight;
            }

            // Set total height
            _scaleCanvas.Height = y;
            if (_markerCanvas != null)
            {
                _markerCanvas.Height = y;
            }
        }

        private void DrawMarker()
        {
            if (_markerCanvas == null || !MarkerValue.HasValue || !NormalizedMarkerPosition.HasValue)
                return;

            double totalHeight = BandHeight * 7;
            double markerY = NormalizedMarkerPosition.Value * totalHeight;
            double width = ActualWidth > 0 ? ActualWidth - 20 : 280;
            if (width <= 0) width = 280;

            // Marker line (arrow pointing to the scale)
            var markerLine = new Line
            {
                X1 = 0,
                Y1 = markerY,
                X2 = width,
                Y2 = markerY,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 3
            };
            _markerCanvas.Children.Add(markerLine);

            // Marker value label
            var markerLabel = new Border
            {
                Background = new SolidColorBrush(Colors.Black),
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8, 4, 8, 4),
                Child = new TextBlock
                {
                    Text = $"{MarkerValue} kWh/m²",
                    Foreground = new SolidColorBrush(Colors.White),
                    FontWeight = FontWeights.Bold,
                    FontSize = 12
                }
            };

            Canvas.SetRight(markerLabel, 10);
            Canvas.SetTop(markerLabel, markerY - 15);
            _markerCanvas.Children.Add(markerLabel);

            // Arrow triangle
            var arrow = new Polygon
            {
                Fill = new SolidColorBrush(Colors.Black),
                Points = new PointCollection
                {
                    new Point(0, markerY - 8),
                    new Point(0, markerY + 8),
                    new Point(12, markerY)
                }
            };
            _markerCanvas.Children.Add(arrow);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            RedrawScale();
        }
    }
}
