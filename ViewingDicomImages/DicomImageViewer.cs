//-----------------------------------------------------------------------
// DICOM Image Viewer - Interactive Window/Level Control
//-----------------------------------------------------------------------
// This Windows Forms viewer demonstrates how to display DICOM images
// with interactive Window Width and Window Center (Level) adjustments.
//
// Key Features:
//   - Real-time image rendering as W/L values change
//   - Preset buttons for common CT viewing configurations
//   - TrackBar controls for fine adjustment
//   - Display of current W/L values
//
// Usage in your applications:
//   var viewer = new DicomImageViewer(pathToDicomFile);
//   Application.Run(viewer);
//-----------------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using FellowOakDicom;
using FellowOakDicom.Imaging;

namespace Com.SaravananSubramanian.ViewingDicomImages
{
    /// <summary>
    /// Interactive DICOM image viewer with Window/Level controls.
    /// </summary>
    public class DicomImageViewer : Form
    {
        // DICOM image object for rendering
        private DicomImage _dicomImage;
        private string _filePath;

        // UI Controls
        private PictureBox _pictureBox;
        private TrackBar _windowWidthSlider;
        private TrackBar _windowCenterSlider;
        private Label _windowWidthLabel;
        private Label _windowCenterLabel;
        private Label _windowWidthValueLabel;
        private Label _windowCenterValueLabel;
        private Panel _controlPanel;
        private FlowLayoutPanel _presetPanel;
        private Label _imageInfoLabel;
        private StatusStrip _statusStrip;
        private ToolStripStatusLabel _statusLabel;

        // Window/Level range constants (suitable for CT images)
        private const int MinWindowWidth = 1;
        private const int MaxWindowWidth = 4096;
        private const int MinWindowCenter = -2048;
        private const int MaxWindowCenter = 2048;

        // Common CT presets: Name, Width, Center
        private readonly (string Name, int Width, int Center)[] _presets = new[]
        {
            ("Default", 0, 0),           // Will use original values
            ("Lung", 1500, -600),
            ("Bone", 2500, 480),
            ("Soft Tissue", 400, 40),
            ("Brain", 80, 40),
            ("Abdomen", 350, 50),
            ("Mediastinum", 500, 50),
            ("Liver", 150, 30)
        };

        // Original W/L values from DICOM file
        private double _originalWindowWidth;
        private double _originalWindowCenter;

        /// <summary>
        /// Creates a new DICOM image viewer for the specified file.
        /// </summary>
        /// <param name="dicomFilePath">Path to the DICOM file to view</param>
        public DicomImageViewer(string dicomFilePath)
        {
            _filePath = dicomFilePath;
            InitializeComponent();
            LoadDicomImage();
        }

        /// <summary>
        /// Initialize all UI components programmatically.
        /// </summary>
        private void InitializeComponent()
        {
            // Form settings
            this.Text = "DICOM Image Viewer - Window/Level Demo";
            this.Size = new Size(1024, 768);
            this.MinimumSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(45, 45, 48);

            // Create main layout
            var mainContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Control panel
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Image area
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));  // Status bar

            // Control Panel
            _controlPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(60, 60, 65),
                Padding = new Padding(10)
            };

            // Window Width controls
            _windowWidthLabel = new Label
            {
                Text = "Window Width:",
                ForeColor = Color.White,
                Location = new Point(10, 10),
                AutoSize = true
            };

            _windowWidthSlider = new TrackBar
            {
                Minimum = MinWindowWidth,
                Maximum = MaxWindowWidth,
                Value = 400,
                TickFrequency = 200,
                LargeChange = 100,
                SmallChange = 10,
                Location = new Point(120, 5),
                Width = 300
            };
            _windowWidthSlider.ValueChanged += OnWindowWidthChanged;

            _windowWidthValueLabel = new Label
            {
                Text = "400",
                ForeColor = Color.LightGreen,
                Font = new Font("Consolas", 10, FontStyle.Bold),
                Location = new Point(430, 10),
                AutoSize = true
            };

            // Window Center controls
            _windowCenterLabel = new Label
            {
                Text = "Window Center:",
                ForeColor = Color.White,
                Location = new Point(10, 45),
                AutoSize = true
            };

            _windowCenterSlider = new TrackBar
            {
                Minimum = MinWindowCenter,
                Maximum = MaxWindowCenter,
                Value = 40,
                TickFrequency = 200,
                LargeChange = 100,
                SmallChange = 10,
                Location = new Point(120, 40),
                Width = 300
            };
            _windowCenterSlider.ValueChanged += OnWindowCenterChanged;

            _windowCenterValueLabel = new Label
            {
                Text = "40",
                ForeColor = Color.LightGreen,
                Font = new Font("Consolas", 10, FontStyle.Bold),
                Location = new Point(430, 45),
                AutoSize = true
            };

            // Preset buttons panel
            _presetPanel = new FlowLayoutPanel
            {
                Location = new Point(500, 5),
                Size = new Size(500, 85),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            // Create preset buttons
            foreach (var preset in _presets)
            {
                var button = new Button
                {
                    Text = preset.Name,
                    Size = new Size(90, 35),
                    Margin = new Padding(3),
                    BackColor = Color.FromArgb(80, 80, 85),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Tag = preset
                };
                button.FlatAppearance.BorderColor = Color.Gray;
                button.Click += OnPresetButtonClick;
                _presetPanel.Controls.Add(button);
            }

            // Add controls to control panel
            _controlPanel.Controls.Add(_windowWidthLabel);
            _controlPanel.Controls.Add(_windowWidthSlider);
            _controlPanel.Controls.Add(_windowWidthValueLabel);
            _controlPanel.Controls.Add(_windowCenterLabel);
            _controlPanel.Controls.Add(_windowCenterSlider);
            _controlPanel.Controls.Add(_windowCenterValueLabel);
            _controlPanel.Controls.Add(_presetPanel);

            // Image display area
            var imagePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                AutoScroll = true
            };

            _pictureBox = new PictureBox
            {
                BackColor = Color.Black,
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Fill
            };
            imagePanel.Controls.Add(_pictureBox);

            // Image info label (overlay)
            _imageInfoLabel = new Label
            {
                AutoSize = true,
                BackColor = Color.FromArgb(150, 0, 0, 0),
                ForeColor = Color.Yellow,
                Font = new Font("Consolas", 9),
                Location = new Point(10, 10),
                Padding = new Padding(5)
            };
            _pictureBox.Controls.Add(_imageInfoLabel);

            // Status strip
            _statusStrip = new StatusStrip
            {
                BackColor = Color.FromArgb(0, 122, 204)
            };
            _statusLabel = new ToolStripStatusLabel
            {
                ForeColor = Color.White,
                Text = "Ready"
            };
            _statusStrip.Items.Add(_statusLabel);

            // Add to main container
            mainContainer.Controls.Add(_controlPanel, 0, 0);
            mainContainer.Controls.Add(imagePanel, 0, 1);
            mainContainer.Controls.Add(_statusStrip, 0, 2);

            this.Controls.Add(mainContainer);

            // Form events
            this.KeyPreview = true;
            this.KeyDown += OnKeyDown;
        }

        /// <summary>
        /// Loads the DICOM image and extracts metadata.
        /// </summary>
        private void LoadDicomImage()
        {
            try
            {
                _statusLabel.Text = "Loading DICOM file...";

                // Load DICOM file and extract metadata
                var file = DicomFile.Open(_filePath);
                var dataset = file.Dataset;

                // Get original window/level values
                _originalWindowWidth = dataset.GetSingleValueOrDefault(DicomTag.WindowWidth, 400.0);
                _originalWindowCenter = dataset.GetSingleValueOrDefault(DicomTag.WindowCenter, 40.0);

                // Update presets array with original values
                _presets[0] = ("Default", (int)_originalWindowWidth, (int)_originalWindowCenter);

                // Extract image info for display
                var patientName = dataset.GetSingleValueOrDefault(DicomTag.PatientName, "Unknown");
                var studyDate = dataset.GetSingleValueOrDefault(DicomTag.StudyDate, "Unknown");
                var modality = dataset.GetSingleValueOrDefault(DicomTag.Modality, "Unknown");
                var rows = dataset.GetSingleValueOrDefault(DicomTag.Rows, 0);
                var columns = dataset.GetSingleValueOrDefault(DicomTag.Columns, 0);
                var bitsAllocated = dataset.GetSingleValueOrDefault(DicomTag.BitsAllocated, 0);
                var photometric = dataset.GetSingleValueOrDefault(DicomTag.PhotometricInterpretation, "Unknown");

                _imageInfoLabel.Text =
                    $"Patient: {patientName}\n" +
                    $"Study Date: {studyDate}\n" +
                    $"Modality: {modality}\n" +
                    $"Size: {columns} x {rows}\n" +
                    $"Bits: {bitsAllocated}\n" +
                    $"Photometric: {photometric}";

                // Create DICOM image for rendering
                _dicomImage = new DicomImage(_filePath);

                // Set initial slider values from file
                _windowWidthSlider.Value = ClampValue((int)_originalWindowWidth, MinWindowWidth, MaxWindowWidth);
                _windowCenterSlider.Value = ClampValue((int)_originalWindowCenter, MinWindowCenter, MaxWindowCenter);

                // Render the initial image
                RenderImage();

                _statusLabel.Text = $"Loaded: {Path.GetFileName(_filePath)} | Original W/L: {_originalWindowWidth}/{_originalWindowCenter}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading DICOM file:\n{ex.Message}",
                    "Load Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                _statusLabel.Text = "Error loading file";
            }
        }

        /// <summary>
        /// Renders the DICOM image with current Window/Level settings.
        /// </summary>
        private void RenderImage()
        {
            if (_dicomImage == null) return;

            try
            {
                // Apply current window/level settings
                _dicomImage.WindowWidth = _windowWidthSlider.Value;
                _dicomImage.WindowCenter = _windowCenterSlider.Value;

                // Render and display
                var renderedImage = _dicomImage.RenderImage(0);
                var bitmap = renderedImage.As<Bitmap>();

                // Dispose old image if exists
                _pictureBox.Image?.Dispose();
                _pictureBox.Image = bitmap;
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"Render error: {ex.Message}";
            }
        }

        /// <summary>
        /// Handles Window Width slider changes.
        /// </summary>
        private void OnWindowWidthChanged(object sender, EventArgs e)
        {
            _windowWidthValueLabel.Text = _windowWidthSlider.Value.ToString();
            RenderImage();
        }

        /// <summary>
        /// Handles Window Center slider changes.
        /// </summary>
        private void OnWindowCenterChanged(object sender, EventArgs e)
        {
            _windowCenterValueLabel.Text = _windowCenterSlider.Value.ToString();
            RenderImage();
        }

        /// <summary>
        /// Handles preset button clicks.
        /// </summary>
        private void OnPresetButtonClick(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var preset = ((string Name, int Width, int Center))button.Tag;

            // Update sliders (which will trigger re-render)
            _windowWidthSlider.Value = ClampValue(preset.Width, MinWindowWidth, MaxWindowWidth);
            _windowCenterSlider.Value = ClampValue(preset.Center, MinWindowCenter, MaxWindowCenter);

            _statusLabel.Text = $"Applied preset: {preset.Name} (WW={preset.Width}, WL={preset.Center})";
        }

        /// <summary>
        /// Handles keyboard shortcuts.
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.R: // Reset to default
                    _windowWidthSlider.Value = ClampValue((int)_originalWindowWidth, MinWindowWidth, MaxWindowWidth);
                    _windowCenterSlider.Value = ClampValue((int)_originalWindowCenter, MinWindowCenter, MaxWindowCenter);
                    _statusLabel.Text = "Reset to original window/level values";
                    break;

                case Keys.S: // Save current view
                    SaveCurrentView();
                    break;

                case Keys.Escape: // Close
                    this.Close();
                    break;

                case Keys.D1: // Preset 1 - Lung
                    ApplyPreset(1);
                    break;

                case Keys.D2: // Preset 2 - Bone
                    ApplyPreset(2);
                    break;

                case Keys.D3: // Preset 3 - Soft Tissue
                    ApplyPreset(3);
                    break;

                case Keys.D4: // Preset 4 - Brain
                    ApplyPreset(4);
                    break;

                case Keys.D5: // Preset 5 - Abdomen
                    ApplyPreset(5);
                    break;
            }
        }

        /// <summary>
        /// Applies a preset by index.
        /// </summary>
        private void ApplyPreset(int index)
        {
            if (index >= 0 && index < _presets.Length)
            {
                var preset = _presets[index];
                _windowWidthSlider.Value = ClampValue(preset.Width, MinWindowWidth, MaxWindowWidth);
                _windowCenterSlider.Value = ClampValue(preset.Center, MinWindowCenter, MaxWindowCenter);
                _statusLabel.Text = $"Applied preset: {preset.Name}";
            }
        }

        /// <summary>
        /// Saves the current view as a PNG file.
        /// </summary>
        private void SaveCurrentView()
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PNG Image|*.png|JPEG Image|*.jpg|BMP Image|*.bmp",
                    Title = "Save Current View",
                    FileName = $"dicom_view_WW{_windowWidthSlider.Value}_WL{_windowCenterSlider.Value}"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var format = ImageFormat.Png;
                    if (saveDialog.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                        format = ImageFormat.Jpeg;
                    else if (saveDialog.FileName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                        format = ImageFormat.Bmp;

                    _pictureBox.Image.Save(saveDialog.FileName, format);
                    _statusLabel.Text = $"Saved: {saveDialog.FileName}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Clamps a value to the specified range.
        /// </summary>
        private static int ClampValue(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// Clean up resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pictureBox?.Image?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
