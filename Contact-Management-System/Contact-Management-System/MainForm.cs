using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Contact_Management_System
{
    public partial class MainForm : Form
    {
        string connectionString = "Data Source=TOSHIBA;Initial Catalog=contact_db;Integrated Security=True;Encrypt=False";

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadContacts();
            cmbGenderFilter.SelectedIndex = 0; // Default to 'All'
            cmbGender.Text = "Male";
        }

        private void LoadContacts(string genderFilter = "All")
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Contacts";
                if (genderFilter != "All")
                {
                    query += " WHERE Gender = @Gender";
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (genderFilter != "All")
                    {
                        command.Parameters.AddWithValue("@Gender", genderFilter);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvContacts.DataSource = dataTable;
                }
            }
        }

        private void cmbGenderFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedGender = cmbGenderFilter.SelectedItem.ToString();
            LoadContacts(selectedGender);
        }

        private void Clear()
        {
            txtName.Text = "";
            txtAddress.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtSearch.Text = "";
            cmbGender.Text = "Male";           
            txtName.Focus();
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            string name = txtName.Text;
            string address = txtAddress.Text;
            string phone = txtPhone.Text;
            string email = txtEmail.Text;
            string gender = cmbGender.Text;

            try
            {
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(gender))
                {
                    MessageBox.Show("Please fill all fields", "Contact Management System", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtName.Focus();
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "INSERT INTO Contacts (Name, Address, Phone, Email, Gender) VALUES (@Name, @Address, @Phone, @Email, @Gender)";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Name", name);
                            command.Parameters.AddWithValue("@Address", address);
                            command.Parameters.AddWithValue("@Phone", phone);
                            command.Parameters.AddWithValue("@Email", email);
                            command.Parameters.AddWithValue("@Gender", gender);

                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }

                    MessageBox.Show("Contact registered successfully", "Contact Management System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadContacts();
                    Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Contact Management System", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtName.Focus();
            }
        }

        private void dgvContacts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvContacts.Rows[e.RowIndex];

                if (row != null)
                {
                    txtName.Text = row.Cells[1].Value.ToString();
                    txtAddress.Text = row.Cells[2].Value.ToString();
                    txtPhone.Text = row.Cells[3].Value.ToString();
                    txtEmail.Text = row.Cells[4].Value.ToString();
                    cmbGender.Text = row.Cells[5].Value.ToString();

                    btnAdd.Enabled = true;
                    btnEdit.Enabled = true;
                    btnDelete.Enabled = true;                    
                }
            }
        }

        private void dgvContacts_Leave(object sender, EventArgs e)
        {
            
            btnEdit.Enabled = true;
            btnDelete.Enabled = true;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Clear();
            btnAdd.Enabled = true;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string name = txtName.Text;
            string address = txtAddress.Text;
            string phone = txtPhone.Text;
            string email = txtEmail.Text;
            string gender = cmbGender.Text;

            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Please select a row", "Contact Management System", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Clear();
                }
                else
                {
                    DialogResult result = MessageBox.Show("Do you want to save changes?", "Contact Management System", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            string query = "UPDATE Contacts SET Name = @Name, Address = @Address, Phone = @Phone, Email = @Email, Gender = @Gender WHERE Id = @Id";
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@Name", name);
                                command.Parameters.AddWithValue("@Address", address);
                                command.Parameters.AddWithValue("@Phone", phone);
                                command.Parameters.AddWithValue("@Email", email);
                                command.Parameters.AddWithValue("@Gender", gender);
                                command.Parameters.AddWithValue("@Id", dgvContacts.SelectedRows[0].Cells[0].Value);

                                connection.Open();
                                command.ExecuteNonQuery();
                                connection.Close();                                
                            }
                        }

                        MessageBox.Show("Contact updated successfully", "Contact Management System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadContacts();
                        Clear();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Contact Management System", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Clear();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please select a row", "Contact Management System", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Clear();
            }
            else
            {
                DialogResult result = MessageBox.Show("Are you sure that you want to permanently delete this record?", "Contact Management System", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM Contacts WHERE Id = @Id";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", dgvContacts.SelectedRows[0].Cells[0].Value);

                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }

                    MessageBox.Show("Contact deleted successfully", "Contact Management System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadContacts();
                    Clear();
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            string selectedGender = cmbGenderFilter.SelectedItem.ToString();

            string query = @"
        SELECT * FROM Contacts
        WHERE 
            (Id LIKE @keyword OR
            Name LIKE @keyword OR
            Address LIKE @keyword OR
            Email LIKE @keyword OR
            Phone LIKE @keyword)";

            if (selectedGender != "All")
            {
                query += " AND Gender = @gender";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

                if (selectedGender != "All")
                {
                    command.Parameters.AddWithValue("@gender", selectedGender);
                }

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dgvContacts.DataSource = dataTable;
            }

        }

        private void cmbGenderFilter_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string selectedGender = cmbGenderFilter.SelectedItem.ToString();
            LoadContacts(selectedGender);
        }

        private void lblClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}