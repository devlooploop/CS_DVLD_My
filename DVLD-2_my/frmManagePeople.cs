using System;
using System.Data;
using System.Windows.Forms;
using Business;
using DVLD_2_my.GlobalClasses;


namespace DVLD_2_my
{
    public partial class frmManagePeople : Form
    {

        private DataTable _AllPeopleData = clsPerson.GetAllPeople();

        public frmManagePeople()
        {
            InitializeComponent();
            cbFilterBy.SelectedIndexChanged += CbFilterBy_SelectedIndexChanged;
        }

        private void CbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbFilterBy.Visible = (cbFilterBy.SelectedIndex != 0);
        }

        private void btnAddPerson_Click(object sender, EventArgs e)
        {
            Form frm = new frmAddEditPerson();
            frm.ShowDialog();
        }

        private void _RefreshPeopleData()
        {
            if (_AllPeopleData == null || tbFilterBy.Text == string.Empty)
                _AllPeopleData = clsPerson.GetAllPeople();

            dgvManagePeople.DataSource = _AllPeopleData;
            lblRecord.Text = "#Recorde: " + _AllPeopleData.Rows.Count.ToString();
        }

        private void _FillCBboxFilterBy()
        {
            string[] _FilterOptions = { "None", "Person ID", "National No.", "First Name", "Second Name",
                                                        "Third Name", "Last Name", "Nationality", "Gender", "Phone", "Email" };
            cbFilterBy.Items.Clear();
            cbFilterBy.Items.AddRange(_FilterOptions);
            cbFilterBy.SelectedIndex = 0;

            tbFilterBy.Visible = false;
        }

        private void frmManagePeople_Load(object sender, EventArgs e)
        {
            _RefreshPeopleData();
            _FillCBboxFilterBy();
        }

        private void tbFilterBy_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Delete)
                return;

            if (clsValidations.ValidateDigits(e.KeyChar.ToString()))
                return;
        }

        private void tbFilterBy_TextChanged(object sender, EventArgs e)
        {
            DataView dv = _AllPeopleData.DefaultView;

            if (string.IsNullOrEmpty(tbFilterBy.Text) && cbFilterBy.SelectedIndex == 0)
            {
                dv.RowFilter = string.Empty;
                dgvManagePeople.DataSource = dv;
                lblRecord.Text = "#Recorde: " + _AllPeopleData.Rows.Count.ToString();
                return;
            }

            if (cbFilterBy.Text == "Person ID")
            {
                if (int.TryParse(tbFilterBy.Text, out int PersonID))
                    dv.RowFilter = $"PersonID = {PersonID}";
                else
                    dv.RowFilter = string.Empty;
            }

            dgvManagePeople.DataSource = dv;
            lblRecord.Text = "#Recorde: " + dv.Count.ToString();
        }

    }

}
