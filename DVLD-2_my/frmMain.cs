
using System;
using System.Windows.Forms;

namespace DVLD_2_my
{
    public partial class frmMainMenu : Form
    {
        public frmMainMenu()
        {
            InitializeComponent();
        }
              
        private void tsmiPeople_Click(object sender, EventArgs e)
        {
            Form frm = new frmManagePeople();
            frm.ShowDialog();
            
        }
    }
}
