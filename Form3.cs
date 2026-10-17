using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BT17TH10
{
    public partial class Form3 : Form
    {
        // ==========================================
        // KHAI BÁO CÁC CONTROL GIAO DIỆN
        // ==========================================
        private MenuStrip menuStrip1;
        private ToolStripMenuItem menuFile;
        private ToolStripMenuItem menuExportCSV;
        private ToolStripMenuItem menuExit;

        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblStatusCount;
        private ToolStripStatusLabel lblTotalCost;

        private TableLayoutPanel mainLayout;
        private GroupBox gbInput;
        private TableLayoutPanel inputLayout;

        private Label lblOrderId, lblCustomerName, lblPhone, lblDeviceType;
        private Label lblFaultDescription, lblReceiveDate, lblEstimatedCost, lblStatus;

        private TextBox txtOrderId, txtCustomerName, txtPhone, txtEstimatedCost;
        private TextBox txtFaultDescription;
        private ComboBox cboDeviceType, cboStatus;
        private DateTimePicker dtpReceiveDate;
        private PictureBox picDevice;
        private Button btnBrowseImage;

        private FlowLayoutPanel buttonPanel;
        private Button btnAdd, btnUpdate, btnDelete, btnClear;

        private Panel filterPanel;
        private Label lblSearch, lblFilterStatus;
        private TextBox txtSearch;
        private ComboBox cboFilterStatus;

        private DataGridView dgvOrders;
        private ErrorProvider errorProvider1;

        // DỮ LIỆU & BINDING
        private BindingList<RepairOrder> _masterList = new BindingList<RepairOrder>();
        private BindingSource _bindingSource = new BindingSource();
        private string _selectedImagePath = string.Empty;

        // ==========================================
        // CONSTRUCTOR & KHỞI TẠO GIAO DIỆN
        // ==========================================
        public Form3()
        {
            InitializeComponentCustom();
            SetupDataGridView();
            InitData();
            RegisterEvents();
        }

        private void InitializeComponentCustom()
        {
            this.Text = "Repair Service Tracker - Quản lý đơn đăng ký dịch vụ sửa chữa";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            errorProvider1 = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };

            // 1. MenuStrip
            menuStrip1 = new MenuStrip();
            menuFile = new ToolStripMenuItem("Tệp");
            menuExportCSV = new ToolStripMenuItem("Xuất CSV", null, menuExportCSV_Click);
            menuExit = new ToolStripMenuItem("Thoát", null, menuExit_Click);
            menuFile.DropDownItems.AddRange(new ToolStripItem[] { menuExportCSV, menuExit });
            menuStrip1.Items.Add(menuFile);

            // 2. StatusStrip
            statusStrip1 = new StatusStrip();
            lblStatusCount = new ToolStripStatusLabel { Text = "Số đơn hiển thị: 0", AutoSize = true };
            lblTotalCost = new ToolStripStatusLabel { Text = "Tổng chi phí (chưa hoàn tất): 0 VNĐ", AutoSize = true, Margin = new Padding(20, 0, 0, 0) };
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatusCount, lblTotalCost });

            // 3. Main Layout (Responsive)
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                Padding = new Padding(10)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 230F)); // Khu vực nhập liệu
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));  // Thanh tìm kiếm / lọc
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Lưới danh sách

            // --- GroupBox Thông tin đơn ---
            gbInput = new GroupBox
            {
                Text = "Thông tin đơn đăng ký sửa chữa",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            inputLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 7,
                Padding = new Padding(5)
            };
            // Cấu hình tỷ lệ cột
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F)); // Label col 1
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));  // Control col 1
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F)); // Label col 2
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));  // Control col 2
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));  // Spacing
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F)); // PictureBox
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F)); // Button chọn ảnh

            // Khởi tạo Controls Nhập liệu
            lblOrderId = new Label { Text = "Mã đơn:", Anchor = AnchorStyles.Left, AutoSize = true, Font = new Font("Segoe UI", 9F) };
            txtOrderId = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 9F) };

            lblCustomerName = new Label { Text = "Khách hàng:", Anchor = AnchorStyles.Left, AutoSize = true, Font = new Font("Segoe UI", 9F) };
            txtCustomerName = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 9F) };

            lblPhone = new Label { Text = "Số điện thoại:", Anchor = AnchorStyles.Left, AutoSize = true, Font = new Font("Segoe UI", 9F) };
            txtPhone = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 9F) };

            lblDeviceType = new Label { Text = "Loại thiết bị:", Anchor = AnchorStyles.Left, AutoSize = true, Font = new Font("Segoe UI", 9F) };
            cboDeviceType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 9F) };

            lblFaultDescription = new Label { Text = "Mô tả lỗi:", Anchor = AnchorStyles.Left, AutoSize = true, Font = new Font("Segoe UI", 9F) };
            txtFaultDescription = new TextBox { Multiline = true, Height = 40, Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom, Font = new Font("Segoe UI", 9F) };

            lblReceiveDate = new Label { Text = "Ngày nhận:", Anchor = AnchorStyles.Left, AutoSize = true, Font = new Font("Segoe UI", 9F) };
            dtpReceiveDate = new DateTimePicker { Format = DateTimePickerFormat.Short, Anchor = AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 9F) };

            lblEstimatedCost = new Label { Text = "Chi phí dự kiến:", Anchor = AnchorStyles.Left, AutoSize = true, Font = new Font("Segoe UI", 9F) };
            txtEstimatedCost = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 9F) };

            lblStatus = new Label { Text = "Trạng thái:", Anchor = AnchorStyles.Left, AutoSize = true, Font = new Font("Segoe UI", 9F) };
            cboStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 9F) };

            picDevice = new PictureBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Fill
            };
            btnBrowseImage = new Button { Text = "Chọn ảnh...", Height = 30, Anchor = AnchorStyles.Top | AnchorStyles.Left, Font = new Font("Segoe UI", 8.5F) };

            // Thêm vào Input Layout
            inputLayout.Controls.Add(lblOrderId, 0, 0); inputLayout.Controls.Add(txtOrderId, 1, 0);
            inputLayout.Controls.Add(lblCustomerName, 2, 0); inputLayout.Controls.Add(txtCustomerName, 3, 0);

            inputLayout.Controls.Add(lblPhone, 0, 1); inputLayout.Controls.Add(txtPhone, 1, 1);
            inputLayout.Controls.Add(lblDeviceType, 2, 1); inputLayout.Controls.Add(cboDeviceType, 3, 1);

            inputLayout.Controls.Add(lblFaultDescription, 0, 2); inputLayout.Controls.Add(txtFaultDescription, 1, 2);
            inputLayout.Controls.Add(lblReceiveDate, 2, 2); inputLayout.Controls.Add(dtpReceiveDate, 3, 2);

            inputLayout.Controls.Add(lblEstimatedCost, 0, 3); inputLayout.Controls.Add(txtEstimatedCost, 1, 3);
            inputLayout.Controls.Add(lblStatus, 2, 3); inputLayout.Controls.Add(cboStatus, 3, 3);

            // PictureBox gộp dòng
            inputLayout.Controls.Add(picDevice, 5, 0);
            inputLayout.SetRowSpan(picDevice, 3);
            inputLayout.Controls.Add(btnBrowseImage, 5, 3);

            // Button Panel (Thêm, Sửa, Xóa, Làm mới)
            buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 35, FlowDirection = FlowDirection.RightToLeft };
            btnAdd = new Button { Text = "Thêm", Width = 80, Height = 30, Font = new Font("Segoe UI", 9F) };
            btnUpdate = new Button { Text = "Cập nhật", Width = 80, Height = 30, Font = new Font("Segoe UI", 9F) };
            btnDelete = new Button { Text = "Xóa", Width = 80, Height = 30, Font = new Font("Segoe UI", 9F) };
            btnClear = new Button { Text = "Làm mới", Width = 80, Height = 30, Font = new Font("Segoe UI", 9F) };

            buttonPanel.Controls.AddRange(new Control[] { btnClear, btnDelete, btnUpdate, btnAdd });

            Panel gbContentPanel = new Panel { Dock = DockStyle.Fill };
            gbContentPanel.Controls.Add(inputLayout);
            gbContentPanel.Controls.Add(buttonPanel);
            gbInput.Controls.Add(gbContentPanel);

            // --- Thanh Tìm kiếm & Lọc ---
            filterPanel = new Panel { Dock = DockStyle.Fill };
            lblSearch = new Label { Text = "Tìm kiếm (Mã/Tên/SĐT):", Location = new Point(5, 10), AutoSize = true, Font = new Font("Segoe UI", 9F) };
            txtSearch = new TextBox { Location = new Point(150, 7), Width = 200, Font = new Font("Segoe UI", 9F) };

            lblFilterStatus = new Label { Text = "Lọc trạng thái:", Location = new Point(380, 10), AutoSize = true, Font = new Font("Segoe UI", 9F) };
            cboFilterStatus = new ComboBox { Location = new Point(470, 7), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };

            filterPanel.Controls.AddRange(new Control[] { lblSearch, txtSearch, lblFilterStatus, cboFilterStatus });

            // --- DataGridView ---
            dgvOrders = new DataGridView
            {
                Dock = DockStyle.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White
            };

            // Ghép vào MainLayout
            mainLayout.Controls.Add(gbInput, 0, 0);
            mainLayout.Controls.Add(filterPanel, 0, 1);
            mainLayout.Controls.Add(dgvOrders, 0, 2);

            // Add vào Form
            this.Controls.Add(mainLayout);
            this.Controls.Add(statusStrip1);
            this.Controls.Add(menuStrip1);
            this.MainMenuStrip = menuStrip1;
        }

        // ==========================================
        // CẤU HÌNH DATA GRID VIEW & NẠP DỮ LIỆU BAN ĐẦU
        // ==========================================
        private void SetupDataGridView()
        {
            dgvOrders.AutoGenerateColumns = false;
            dgvOrders.Columns.Clear();

            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "OrderId", HeaderText = "Mã Đơn", Width = 90 });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CustomerName", HeaderText = "Tên Khách Hàng", Width = 150 });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Phone", HeaderText = "Số ĐT", Width = 100 });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DeviceType", HeaderText = "Loại Thiết Bị", Width = 110 });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FaultDescription", HeaderText = "Mô Tả Lỗi", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            var dateCol = new DataGridViewTextBoxColumn { DataPropertyName = "ReceiveDate", HeaderText = "Ngày Nhận", Width = 100 };
            dateCol.DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvOrders.Columns.Add(dateCol);

            var costCol = new DataGridViewTextBoxColumn { DataPropertyName = "EstimatedCost", HeaderText = "Chi Phí Dự Kiến", Width = 120 };
            costCol.DefaultCellStyle.Format = "N0";
            dgvOrders.Columns.Add(costCol);

            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Trạng Thái", Width = 120 });
        }

        private void InitData()
        {
            cboDeviceType.Items.AddRange(new string[] { "Điện thoại", "Laptop", "Máy tính bảng", "PC", "Khác" });
            cboDeviceType.SelectedIndex = 0;

            string[] statusList = new string[] { "Mới tiếp nhận", "Đang sửa", "Chờ linh kiện", "Hoàn tất", "Đã hủy" };
            cboStatus.Items.AddRange(statusList);
            cboStatus.SelectedIndex = 0;

            cboFilterStatus.Items.Add("Tất cả");
            cboFilterStatus.Items.AddRange(statusList);
            cboFilterStatus.SelectedIndex = 0;

            // Thêm dữ liệu mẫu
            _masterList.Add(new RepairOrder
            {
                OrderId = "HD01",
                CustomerName = "Nguyễn Văn A",
                Phone = "0901234567",
                DeviceType = "Laptop",
                FaultDescription = "Hỏng bàn phím và chai pin",
                ReceiveDate = DateTime.Now.AddDays(-2),
                EstimatedCost = 850000,
                Status = "Đang sửa",
                ImagePath = ""
            });

            _masterList.Add(new RepairOrder
            {
                OrderId = "HD02",
                CustomerName = "Trần Thị B",
                Phone = "0987654321",
                DeviceType = "Điện thoại",
                FaultDescription = "Vỡ màn hình cảm ứng",
                ReceiveDate = DateTime.Now.AddDays(-1),
                EstimatedCost = 1200000,
                Status = "Mới tiếp nhận",
                ImagePath = ""
            });

            _bindingSource.DataSource = _masterList;
            dgvOrders.DataSource = _bindingSource;
            UpdateStatusStrip();
        }

        private void RegisterEvents()
        {
            btnBrowseImage.Click += btnBrowseImage_Click;
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnClear.Click += (s, e) => ClearInput();

            txtSearch.TextChanged += FilterData;
            cboFilterStatus.SelectedIndexChanged += FilterData;
            dgvOrders.SelectionChanged += DgvOrders_SelectionChanged;
        }

        // ==========================================
        // XỬ LÝ LOGIC NGHIỆP VỤ & SỰ KIỆN
        // ==========================================
        private void FilterData(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            string selectedStatus = cboFilterStatus.SelectedItem != null ? cboFilterStatus.SelectedItem.ToString() : "Tất cả";

            var filtered = _masterList.Where(item =>
                (string.IsNullOrEmpty(keyword) ||
                 item.OrderId.ToLower().Contains(keyword) ||
                 item.CustomerName.ToLower().Contains(keyword) ||
                 item.Phone.ToLower().Contains(keyword)) &&
                (selectedStatus == "Tất cả" || item.Status == selectedStatus)
            ).ToList();

            _bindingSource.DataSource = new BindingList<RepairOrder>(filtered);
            UpdateStatusStrip();
        }

        private bool ValidateData(bool isUpdating = false)
        {
            bool isValid = true;
            errorProvider1.Clear();

            // 1. Kiểm tra Mã đơn
            if (string.IsNullOrWhiteSpace(txtOrderId.Text))
            {
                errorProvider1.SetError(txtOrderId, "Mã đơn không được để trống!");
                isValid = false;
            }
            else if (!isUpdating && _masterList.Any(x => x.OrderId.Equals(txtOrderId.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                errorProvider1.SetError(txtOrderId, "Mã đơn đã tồn tại trong hệ thống!");
                isValid = false;
            }

            // 2. Kiểm tra Tên khách hàng
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                errorProvider1.SetError(txtCustomerName, "Tên khách hàng không được để trống!");
                isValid = false;
            }

            // 3. Kiểm tra Số điện thoại
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                errorProvider1.SetError(txtPhone, "Số điện thoại không được để trống!");
                isValid = false;
            }

            // 4. Kiểm tra Mô tả lỗi
            if (string.IsNullOrWhiteSpace(txtFaultDescription.Text))
            {
                errorProvider1.SetError(txtFaultDescription, "Mô tả lỗi bắt buộc phải nhập!");
                isValid = false;
            }

            // 5. Kiểm tra Chi phí dự kiến
            if (!decimal.TryParse(txtEstimatedCost.Text, out decimal cost) || cost < 0)
            {
                errorProvider1.SetError(txtEstimatedCost, "Chi phí dự kiến phải là số không âm!");
                isValid = false;
            }

            return isValid;
        }

        private void DgvOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvOrders.CurrentRow != null && dgvOrders.CurrentRow.DataBoundItem is RepairOrder selectedOrder)
            {
                txtOrderId.Text = selectedOrder.OrderId;
                txtCustomerName.Text = selectedOrder.CustomerName;
                txtPhone.Text = selectedOrder.Phone;
                cboDeviceType.SelectedItem = selectedOrder.DeviceType;
                txtFaultDescription.Text = selectedOrder.FaultDescription;
                dtpReceiveDate.Value = selectedOrder.ReceiveDate;
                txtEstimatedCost.Text = selectedOrder.EstimatedCost.ToString("G0");
                cboStatus.SelectedItem = selectedOrder.Status;

                _selectedImagePath = selectedOrder.ImagePath;
                LoadImageToPictureBox(_selectedImagePath);
            }
        }

        private void LoadImageToPictureBox(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    picDevice.Image = Image.FromStream(stream);
                }
            }
            else
            {
                picDevice.Image = null;
            }
        }

        private void btnBrowseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                ofd.Title = "Chọn ảnh thiết bị";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _selectedImagePath = ofd.FileName;
                    LoadImageToPictureBox(_selectedImagePath);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateData(isUpdating: false)) return;

            var order = new RepairOrder
            {
                OrderId = txtOrderId.Text.Trim(),
                CustomerName = txtCustomerName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                DeviceType = cboDeviceType.SelectedItem.ToString(),
                FaultDescription = txtFaultDescription.Text.Trim(),
                ReceiveDate = dtpReceiveDate.Value,
                EstimatedCost = decimal.Parse(txtEstimatedCost.Text.Trim()),
                Status = cboStatus.SelectedItem.ToString(),
                ImagePath = _selectedImagePath
            };

            _masterList.Add(order);
            FilterData(null, null);
            MessageBox.Show("Thêm đơn sửa chữa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearInput();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvOrders.CurrentRow == null) return;
            if (!ValidateData(isUpdating: true)) return;

            string currentId = txtOrderId.Text.Trim();
            var order = _masterList.FirstOrDefault(x => x.OrderId.Equals(currentId, StringComparison.OrdinalIgnoreCase));

            if (order != null)
            {
                order.CustomerName = txtCustomerName.Text.Trim();
                order.Phone = txtPhone.Text.Trim();
                order.DeviceType = cboDeviceType.SelectedItem.ToString();
                order.FaultDescription = txtFaultDescription.Text.Trim();
                order.ReceiveDate = dtpReceiveDate.Value;
                order.EstimatedCost = decimal.Parse(txtEstimatedCost.Text.Trim());
                order.Status = cboStatus.SelectedItem.ToString();
                order.ImagePath = _selectedImagePath;

                _bindingSource.ResetBindings(false);
                FilterData(null, null);
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvOrders.CurrentRow != null && dgvOrders.CurrentRow.DataBoundItem is RepairOrder selectedOrder)
            {
                var confirm = MessageBox.Show($"Bạn có chắc muốn xóa đơn [{selectedOrder.OrderId}] không?",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    _masterList.Remove(selectedOrder);
                    FilterData(null, null);
                    ClearInput();
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ClearInput()
        {
            txtOrderId.Clear();
            txtCustomerName.Clear();
            txtPhone.Clear();
            txtFaultDescription.Clear();
            txtEstimatedCost.Clear();
            cboDeviceType.SelectedIndex = 0;
            cboStatus.SelectedIndex = 0;
            dtpReceiveDate.Value = DateTime.Now;
            _selectedImagePath = string.Empty;
            picDevice.Image = null;
            errorProvider1.Clear();
        }

        private void UpdateStatusStrip()
        {
            var currentDisplayed = _bindingSource.List.Cast<RepairOrder>().ToList();

            int displayedCount = currentDisplayed.Count;
            decimal totalPendingCost = currentDisplayed
                .Where(x => x.Status != "Hoàn tất" && x.Status != "Đã hủy")
                .Sum(x => x.EstimatedCost);

            lblStatusCount.Text = $"Số đơn hiển thị: {displayedCount}";
            lblTotalCost.Text = $"Tổng chi phí (chưa hoàn tất): {totalPendingCost:N0} VNĐ";
        }

        private void menuExportCSV_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV File (*.csv)|*.csv";
                sfd.FileName = $"DanhSachSuaChua_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Mã Đơn,Tên Khách Hàng,Số ĐT,Loại Thiết Bị,Mô Tả Lỗi,Ngày Nhận,Chi Phí Dự Kiến,Trạng Thái");

                        foreach (RepairOrder order in _bindingSource.List)
                        {
                            sb.AppendLine($"\"{order.OrderId}\",\"{order.CustomerName}\",\"{order.Phone}\",\"{order.DeviceType}\",\"{order.FaultDescription}\",\"{order.ReceiveDate:dd/MM/yyyy}\",\"{order.EstimatedCost}\",\"{order.Status}\"");
                        }

                        File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        MessageBox.Show("Xuất file CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}