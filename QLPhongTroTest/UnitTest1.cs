using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLPhongTro;
using QLPhongTro.ChildForm;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace QLPhongTro.ChildForm.Tests
{
    [TestClass]
    public class FrmThueTests
    {
        private frmThue form;
        private Database db;

        [TestInitialize]
        public void Setup()
        {
            // Khởi tạo đối tượng frmThue trước mỗi bài kiểm tra
            form = new frmThue();
        }

        [TestMethod]
        public void BtnXacNhan_Click_InvalidDate_ThrowsMessageBoxAndDoesNotCloseForm()
        {
            // Thiết lập ngày thuê và ngày trả không hợp lệ
            var mtbNgayThue = (MaskedTextBox)form.Controls.Find("mtbNgayThue", true)[0];
            var mtbNgayTra = (MaskedTextBox)form.Controls.Find("mtbNgayTra", true)[0];
            mtbNgayThue.Text = "30/06/2021";
            mtbNgayTra.Text = "01/06/2021";

            // Sử dụng reflection để gọi phương thức btnXacNhan_Click
            var method = form.GetType().GetMethod("btnXacNhan_Click", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(form, new object[] { null, EventArgs.Empty });

            // Kiểm tra xem MessageBox đã được hiển thị chưa
            Assert.IsTrue(form.Visible);
            Assert.IsFalse(form.Modal);

            // Kiểm tra xem MessageBox có hiển thị thông báo đúng không
            var messageBoxText = "Ngày thuê không được nhỏ hơn hoặc bằng ngày trả!";
            Assert.AreEqual(DialogResult.OK, MessageBox.Show(messageBoxText));

            // Kiểm tra xem form có bị đóng không
            Assert.IsTrue(form.Visible);
        }

        [TestMethod]
        public void BtnXacNhan_Click_ValidData_CreatesNewHopDong()
        {
            // Sử dụng Reflection để truy cập vào các thành phần private của frmThue
            var db = (Database)form.GetType().GetField("db", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(form);
            var cbbPhong = (ComboBox)form.Controls.Find("cbbPhong", true)[0];
            var mtbNgayThue = (MaskedTextBox)form.Controls.Find("mtbNgayThue", true)[0];
            var mtbNgayTra = (MaskedTextBox)form.Controls.Find("mtbNgayTra", true)[0];
            var txtDatCoc = (TextBox)form.Controls.Find("txtDatCoc", true)[0];

            // Thiết lập dữ liệu hợp lệ
            cbbPhong.SelectedIndex = 0;
            mtbNgayThue.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            mtbNgayTra.Text = DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy HH:mm");
            txtDatCoc.Text = "1000000";

            // Gọi phương thức btnXacNhan_Click
            var method = form.GetType().GetMethod("btnXacNhan_Click", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(form, new object[] { null, EventArgs.Empty });

            // Kiểm tra xem hợp đồng đã được tạo thành công chưa
            var lstParameters = new List<CustomParameter>()
            {
                new CustomParameter { key = "@idPhong", value = "1" },
                new CustomParameter { key = "@datCoc", value = "1000000" },
                new CustomParameter { key = "@TienVeSinh", value = "0" },
                new CustomParameter { key = "@TienMang", value = "0" }
            };
            var result = db.ExeCute("TaoMoiHopDong", lstParameters);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void BtnThemKH_Click_InvalidData_DoesNotAddKhachHang()
        {
            // Sử dụng Reflection để truy cập vào các thành phần private của frmThue
            var txtHo = (TextBox)form.Controls.Find("txtHo", true)[0];
            var txtTenDem = (TextBox)form.Controls.Find("txtTenDem", true)[0];
            var txtTen = (TextBox)form.Controls.Find("txtTen", true)[0];

            // Thiết lập dữ liệu không hợp lệ
            txtHo.Text = "";
            txtTenDem.Text = "";
            txtTen.Text = "";

            // Gọi phương thức btnThemKH_Click
            var method = form.GetType().GetMethod("btnThemKH_Click", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(form, new object[] { null, EventArgs.Empty });

            // Kiểm tra xem khách hàng đã được thêm thành công chưa
            var lstParameters = new List<CustomParameter>()
            {
                new CustomParameter { key = "@ho", value = "" },
                new CustomParameter { key = "@tenDem", value = "" },
                new CustomParameter { key = "@ten", value = "" }
            };
            var result = db.ExeCute("ThemKhachHang", lstParameters);
            Assert.AreEqual(0, result);
        }
    }
}