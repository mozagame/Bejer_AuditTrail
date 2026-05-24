//--------------------------------------------------------------
// Press F1 to get help with using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memory leaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
    using System.Windows.Forms;
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows.Media.Imaging;
    using Cognex.DataMan.SDK;
    using Neo.ApplicationFramework.Tools;
    using Neo.ApplicationFramework.Common.Graphics.Logic;
    using Neo.ApplicationFramework.Controls;
    using Neo.ApplicationFramework.Interfaces;


    public partial class ScrCamera
    {
        // Cognex SDK objects
        private DataManSystem _system;
        private EthSystemConnector _connector;

        // Data storage
        private string _lastReadString = "";
        private string _lastSvgData = "";
        private System.Drawing.Image _currentImage = null;

        // Configuration
        private string _cameraIP = "192.168.0.11"; // IP của Cognex DM262

        void btConnect_Click(System.Object sender, System.EventArgs e)
        {
            try
            {
                // Kiểm tra IP hợp lệ
                System.Net.IPAddress ipAddress;
                if (!System.Net.IPAddress.TryParse(_cameraIP, out ipAddress))
                {
                    MessageBox.Show("Địa chỉ IP không hợp lệ: " + _cameraIP);
                    return;
                }

                // Nếu đã kết nối, ngắt kết nối trước
                if (_system != null && _system.State == Cognex.DataMan.SDK.ConnectionState.Connected)
                {
                    DisconnectCamera();
                    MessageBox.Show("Đã ngắt kết nối camera!");
                    return;
                }

                // Khởi tạo kết nối mới
                _connector = new EthSystemConnector(ipAddress);
                _system = new DataManSystem(_connector);
                _system.DefaultTimeout = 5000;

                // Đăng ký các sự kiện
                _system.ImageArrived += new ImageArrivedHandler(OnImageArrived);
                _system.ReadStringArrived += new ReadStringArrivedHandler(OnReadStringArrived);
                _system.ImageGraphicsArrived += new ImageGraphicsArrivedHandler(OnImageGraphicsArrived);

                // Kết nối
                _system.Connect();

                if (_system.State == Cognex.DataMan.SDK.ConnectionState.Connected)
                {
                    // Thiết lập các loại kết quả cần nhận
                    _system.SetResultTypes(ResultTypes.Image | ResultTypes.ReadString | ResultTypes.ImageGraphics);
                    MessageBox.Show("Đã kết nối thành công đến camera!");
                }
                else
                {
                    MessageBox.Show("Không thể kết nối đến camera!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối Cognex: " + ex.Message);
            }
        }

        void btCamTrigger_Click(System.Object sender, System.EventArgs e)
        {
            try
            {
                // Kiểm tra xem đã kết nối chưa
                if (_system == null || _system.State != Cognex.DataMan.SDK.ConnectionState.Connected)
                {
                    MessageBox.Show("Vui lòng kết nối camera trước!");
                    return;
                }

                // Reset dữ liệu cũ
                _lastReadString = "";
                _lastSvgData = "";

                // Gửi lệnh Trigger chụp ảnh
                _system.SendCommand("TRIGGER ON");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi gửi lệnh chụp hình: " + ex.Message);
            }
        }

        // Event handler khi ảnh trả về từ camera
        private void OnImageArrived(object sender, ImageArrivedEventArgs args)
        {
            if (_currentImage != null)
            {
                _currentImage.Dispose();
                _currentImage = null;
            }

            // Deep clone ảnh gốc vì Cognex sẽ tự động Dispose
            _currentImage = new System.Drawing.Bitmap(args.Image.Width, args.Image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(_currentImage))
            {
                g.DrawImage(args.Image, 0, 0);
            }

            UpdateUI();
        }

        // Event handler khi kết quả đọc barcode trả về
        private void OnReadStringArrived(object sender, ReadStringArrivedEventArgs args)
        {
            _lastReadString = args.ReadString;
            UpdateUI();
        }

        // Event handler khi SVG graphics trả về (box xung quanh barcode)
        private void OnImageGraphicsArrived(object sender, ImageGraphicsArrivedEventArgs args)
        {
            _lastSvgData = args.ImageGraphics;
            UpdateUI();
        }

        // Cập nhật UI - vẽ ảnh và thông tin lên picture control
        private void UpdateUI()
        {
            // Invoke trên UI thread
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (_currentImage == null) return;

                try
                {
                    // Tạo bitmap để vẽ
                    using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(_currentImage.Width, _currentImage.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                    {
                        using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                        {
                            // Vẽ ảnh gốc
                            g.DrawImage(_currentImage, 0, 0);

                            // Vẽ text Result và DateTime
                            System.Drawing.Font font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Regular);
                            System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Lime);

                            string resultText = "Result: " + (_lastReadString != "" ? _lastReadString : "No Read");
                            string dateTimeText = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt");

                            g.DrawString(resultText, font, brush, new System.Drawing.PointF(5, 5));
                            g.DrawString(dateTimeText, font, brush, new System.Drawing.PointF(5, 25));

                            // Vẽ SVG box nếu có
                            DrawSvgOverlay(g);
                        }

                        // Convert sang BitmapImage cho WPF control
                        MemoryStream ms = new MemoryStream();
                        try
                        {
                            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                            ms.Seek(0, SeekOrigin.Begin);
                            BitmapImage bi = new BitmapImage();
                            bi.BeginInit();
                            bi.StreamSource = ms;
                            bi.CacheOption = BitmapCacheOption.OnLoad;
                            bi.EndInit();
                            bi.Freeze();
                            this.m_picCamera.Source = bi;
                        }
                        catch
                        {
                            ms.Dispose();
                        }
                    }
                }
                catch
                {
                    // Bỏ qua lỗi để không bị popup
                }
            }));
        }

        // Vẽ SVG overlay (box xung quanh barcode)
        private void DrawSvgOverlay(System.Drawing.Graphics g)
        {
            if (string.IsNullOrEmpty(_lastSvgData))
                return;

            try
            {
                // Lấy kích thước SVG từ viewBox
                float svgWidth = _currentImage.Width;
                float svgHeight = _currentImage.Height;
                var vbMatch = System.Text.RegularExpressions.Regex.Match(_lastSvgData, @"viewBox\s*=\s*[""']\s*[\d\.]+\s+[\d\.]+\s+([\d\.]+)\s+([\d\.]+)\s*[""']", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (vbMatch.Success)
                {
                    float.TryParse(vbMatch.Groups[1].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out svgWidth);
                    float.TryParse(vbMatch.Groups[2].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out svgHeight);
                }

                float scaleX = svgWidth > 0 ? (float)_currentImage.Width / svgWidth : 1f;
                float scaleY = svgHeight > 0 ? (float)_currentImage.Height / svgHeight : 1f;

                // Xử lý polygon/polyline
                var matches = System.Text.RegularExpressions.Regex.Matches(_lastSvgData, @"<(?:polygon|polyline)[^>]*points\s*=\s*[""']([^""']+)[""']", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                foreach (System.Text.RegularExpressions.Match m in matches)
                {
                    if (m.Value.Contains("class=\"ROI\"") || m.Value.Contains("class='ROI'"))
                        continue;

                    string[] pairs = m.Groups[1].Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var points = new System.Collections.Generic.List<System.Drawing.Point>();
                    foreach (string pair in pairs)
                    {
                        string[] coords = pair.Split(',');
                        if (coords.Length == 2)
                        {
                            float fx, fy;
                            if (float.TryParse(coords[0].Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out fx) &&
                                float.TryParse(coords[1].Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out fy))
                            {
                                points.Add(new System.Drawing.Point((int)(fx * scaleX), (int)(fy * scaleY)));
                            }
                        }
                    }
                    if (points.Count > 1)
                    {
                        using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Lime, 1))
                        {
                            g.DrawPolygon(pen, points.ToArray());
                        }
                    }
                }

                // Xử lý path
                var pathMatches = System.Text.RegularExpressions.Regex.Matches(_lastSvgData, @"<path[^>]*d\s*=\s*[""']([^""']+)[""']", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                foreach (System.Text.RegularExpressions.Match m in pathMatches)
                {
                    if (m.Value.Contains("class=\"ROI\"") || m.Value.Contains("class='ROI'"))
                        continue;

                    string pathData = m.Groups[1].Value;
                    var coordMatches = System.Text.RegularExpressions.Regex.Matches(pathData, @"([0-9\.]+)[,\s]+([0-9\.]+)");
                    var points = new System.Collections.Generic.List<System.Drawing.Point>();
                    foreach (System.Text.RegularExpressions.Match cm in coordMatches)
                    {
                        float fx, fy;
                        if (float.TryParse(cm.Groups[1].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out fx) &&
                            float.TryParse(cm.Groups[2].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out fy))
                        {
                            points.Add(new System.Drawing.Point((int)(fx * scaleX), (int)(fy * scaleY)));
                        }
                    }
                    if (points.Count > 1)
                    {
                        using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Lime, 3))
                        {
                            g.DrawPolygon(pen, points.ToArray());
                        }
                    }
                }
            }
            catch
            {
                // Bỏ qua lỗi SVG
            }
        }

        // Ngắt kết nối camera
        private void DisconnectCamera()
        {
            if (_system != null)
            {
                _system.ImageArrived -= OnImageArrived;
                _system.ReadStringArrived -= OnReadStringArrived;
                _system.ImageGraphicsArrived -= OnImageGraphicsArrived;
                _system.Disconnect();
                _system.Dispose();
                _system = null;
            }

            if (_connector != null)
            {
                _connector.Dispose();
                _connector = null;
            }

            if (_currentImage != null)
            {
                _currentImage.Dispose();
                _currentImage = null;
            }

            // Xóa ảnh trên picture control
            this.m_picCamera.Source = null;
        }

        // Cleanup khi đóng màn hình
        void ScrCamera_Closed(System.Object sender, System.EventArgs e)
        {
            DisconnectCamera();
        }
    }
}