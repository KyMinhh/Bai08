using DataBinding1.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataBinding1
{
    public partial class Form1 : Form
    {
        private SchoolContext SchoolContext;
        private int currentIndex = -1;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.studentTableAdapter.Fill(this.qLSV2DataSet.Student);

            LoadMajorsIntoComboBox();

            if (dgvSinhVien.Rows.Count > 0)
            {
                currentIndex = 0;
                UpdateStudentInfo(currentIndex);
            }
        }


        private void LoadMajorsIntoComboBox()
        {
            var majors = new HashSet<string>();
            foreach (DataGridViewRow row in dgvSinhVien.Rows)
            {
                if (row.Cells[3].Value != null) 
                {
                    majors.Add(row.Cells[3].Value.ToString());
                }
            }

            cmbChuyenNganh.Items.Clear();
            foreach (var major in majors)
            {
                cmbChuyenNganh.Items.Add(major);
            }
        }

        private void UpdateStudentInfo(int index)
        {
            if (index >= 0 && index < dgvSinhVien.Rows.Count)
            {
                var row = dgvSinhVien.Rows[index];

                // Kiểm tra giá trị ô trước khi truy cập
                txtName.Text = row.Cells[1].Value?.ToString() ?? string.Empty; 
                txtAge.Text = row.Cells[2].Value?.ToString() ?? string.Empty;
                cmbChuyenNganh.SelectedItem = row.Cells[3].Value?.ToString() ?? null; 
            }
        }


        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentIndex < dgvSinhVien.Rows.Count - 1)
            {
                currentIndex++;
                UpdateStudentInfo(currentIndex);
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                UpdateStudentInfo(currentIndex);
            }
        }

        private void dgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                currentIndex = e.RowIndex; 
                UpdateStudentInfo(currentIndex);
            }
        }


        private void LoadStudents()
        {
            using (var context = new SchoolContext())
            {
                var students = context.Students.ToList(); 
                dgvSinhVien.DataSource = students; 
            }
        }


        private void btnThem_Click(object sender, EventArgs e)
        {
            // Tạo một hàng mới trong DataTable
            var newRow = qLSV2DataSet.Student.NewRow();
            newRow["FullName"] = txtName.Text; 
            newRow["Age"] = int.Parse(txtAge.Text); 
            newRow["Major"] = cmbChuyenNganh.SelectedItem.ToString(); 


            qLSV2DataSet.Student.Rows.Add(newRow);

            studentTableAdapter.Update(qLSV2DataSet.Student);


            this.studentTableAdapter.Fill(this.qLSV2DataSet.Student);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSinhVien.CurrentRow != null)
            { 
                var row = (DataRowView)dgvSinhVien.CurrentRow.DataBoundItem;

                row.Row.Delete();

                studentTableAdapter.Update(qLSV2DataSet.Student);

                this.studentTableAdapter.Fill(this.qLSV2DataSet.Student);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvSinhVien.CurrentRow != null)
            {
                var row = dgvSinhVien.CurrentRow.DataBoundItem as Student; 

                if (row != null)
                {
                    row.FullName = txtName.Text; 
                    row.Age = int.Parse(txtAge.Text); 
                    row.Major = cmbChuyenNganh.SelectedItem?.ToString(); 

                    using (var context = new SchoolContext())
                    {
                        context.Entry(row).State = EntityState.Modified; 
                        context.SaveChanges(); 
                    }

                    dgvSinhVien.Refresh(); 
                    var index = dgvSinhVien.CurrentRow.Index; 
                    dgvSinhVien.Rows[index].Cells[1].Value = row.FullName;
                    dgvSinhVien.Rows[index].Cells[2].Value = row.Age;
                    dgvSinhVien.Rows[index].Cells[3].Value = row.Major;
                }
            }
        }

        private void btnPervious_Click(object sender, EventArgs e)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                UpdateStudentInfo(currentIndex);
            }
        }
    }
}
