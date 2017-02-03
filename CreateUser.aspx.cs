using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASTreeViewDemo
{
    public partial class CreateUser : PageBase
    {
        private bool UserExists(string username)
        {
            if (Membership.GetUser(username) != null) { return true; }

            return false;
        }

        protected void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
        {
            // Determine the checkbox values.
            
            TextBox userNameTextBox =
              (TextBox)CreateUserWizardStep1.ContentTemplateContainer.FindControl("UserName");

            string maxSql = string.Format("select max( productId ) from ProductsTree");
            int max = (int)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, maxSql);
            int newId = max + 1;

            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("INSERT INTO ProductsTree (ProductId, ProductName, ParentId,Username) VALUES({0}, '{1}', {3},'{2}')", newId, userNameTextBox.Text, userNameTextBox.Text, 0));
            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("INSERT INTO UserAccess VALUES({0}, '{1}')", newId, userNameTextBox.Text));
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
       

    }
}