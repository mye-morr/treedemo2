#region using
using System;
using System.Data;
using System.Collections.Generic;

using System.Data.OleDb;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Xml;
using System.IO;
using System.Web.UI.HtmlControls;

using System.Web.Services;

using Goldtect;

#endregion

namespace ASTreeViewDemo
{
    public partial class DnDSaveDB : PageBase
    {

        

        protected void Page_Load( object sender, EventArgs e )
        {
            String foo = Session["UserName"].ToString();
            lbUsername.Text = Session["UserName"].ToString();

            if (Session["UserName"]==null)
                Response.Redirect("Default.aspx");
            if (Request.QueryString["ID"] != null)
                lblRoot.Text = Request.QueryString["ID"];
            if (!IsPostBack)
            {
                Page.Header.DataBind();
               ddlRoot1.DataSource= OleDbHelper.ExecuteDataset(base.NorthWindConnectionString, CommandType.Text, string.Format("Select ua.productID,pt.ProductName from UserAccess ua inner join ProductsTree pt on pt.ProductID=ua.ProductID where pt.parentID=0 and ua.username='{0}'", Session["UserName"]));
               ddlRoot1.DataBind();
               ddlRoot2.DataSource = OleDbHelper.ExecuteDataset(base.NorthWindConnectionString, CommandType.Text, string.Format("Select ua.productID,pt.ProductName from UserAccess ua inner join ProductsTree pt on pt.ProductID=ua.ProductID where pt.parentID=0 and ua.username='{0}'", Session["UserName"]));
               ddlRoot2.DataBind();
               BindData();
               this.astvMyTree1.ClearNodesSelection();
               this.astvMyTree2.ClearNodesSelection();
            }
        }

        private void BindCombo1()
        {
            if (!String.IsNullOrEmpty(ddlRoot1.SelectedValue))
            {
                XmlDocument doc = new XmlDocument();
                ASTreeViewXMLDescriptor descripter = new ASTreeViewXMLDescriptor();
                this.astvMyTree1.DataSourceDescriptor = descripter;

                if (File.Exists(Server.MapPath("~/" + ddlRoot1.SelectedValue + ".xml")))
                {
                    doc.Load(Server.MapPath("~/" + ddlRoot1.SelectedValue + ".xml"));
                    this.astvMyTree1.DataSource = doc;
                }

                this.astvMyTree1.EnableRoot = true;
                this.astvMyTree1.RootNodeText = ddlRoot1.SelectedItem.Text;
                this.astvMyTree1.RootNodeValue = ddlRoot1.Text;
                this.astvMyTree1.DataBind();
            }
        }

        private void BindCombo2()
        {
            if (!String.IsNullOrEmpty(ddlRoot2.SelectedValue))
            {
                XmlDocument doc = new XmlDocument();
                ASTreeViewXMLDescriptor descripter = new ASTreeViewXMLDescriptor();
                this.astvMyTree2.DataSourceDescriptor = descripter;

                if (File.Exists(Server.MapPath("~/" + ddlRoot2.SelectedValue + ".xml")))
                {
                    doc.Load(Server.MapPath("~/" + ddlRoot2.SelectedValue + ".xml"));
                    this.astvMyTree2.DataSource = doc;
                }

                this.astvMyTree2.EnableRoot = true;
                this.astvMyTree2.RootNodeText = ddlRoot2.SelectedItem.Text;
                this.astvMyTree2.RootNodeValue = ddlRoot2.Text;
                this.astvMyTree2.DataBind();
            }
        }

        private void BindData()
        {
            BindCombo1();
            BindCombo2();
            ManageNodeTreeName();
        }

        private void ManageNodeTreeName()
        {
            ASTreeView.ASTreeNodeHandlerDelegate nodeDelegate1 = delegate(ASTreeViewNode node)
            {
                node.AdditionalAttributes.Add(new KeyValuePair<string, string>("treeName", "astvMyTree1"));
                if(!String.IsNullOrEmpty(node.NodeValue))
                {
                    OleDbDataReader reader = OleDbHelper.ExecuteReader(base.NorthWindConnectionString, CommandType.Text, "select ProductName,UserName from ProductsTree where ProductID=" + node.NodeValue);
                    reader.Read();
                    node.AdditionalAttributes.Add(new KeyValuePair<string, string>("LongText", reader.GetValue(0).ToString()));
                    node.AdditionalAttributes.Add(new KeyValuePair<string, string>("Owner", reader.GetValue(1).ToString()));
                }
               
            };

            astvMyTree1.TraverseTreeNode(this.astvMyTree1.RootNode, nodeDelegate1);

            ASTreeView.ASTreeNodeHandlerDelegate nodeDelegate2 = delegate(ASTreeViewNode node)
            {
                node.AdditionalAttributes.Add(new KeyValuePair<string, string>("treeName", "astvMyTree2"));
                if (!String.IsNullOrEmpty(node.NodeValue))
                {
                    OleDbDataReader reader = OleDbHelper.ExecuteReader(base.NorthWindConnectionString, CommandType.Text, "select ProductName,UserName from ProductsTree where ProductID=" + node.NodeValue);
                    reader.Read();
                    node.AdditionalAttributes.Add(new KeyValuePair<string, string>("LongText", reader.GetValue(0).ToString()));
                    node.AdditionalAttributes.Add(new KeyValuePair<string, string>("Owner", reader.GetValue(1).ToString()));
                }
            };

            astvMyTree2.TraverseTreeNode(this.astvMyTree2.RootNode, nodeDelegate2);
        }

        private void saveAll()
        {
            if (cekOwner(astvMyTree1.RootNodeValue))
            {
                XmlDocument doc1 = astvMyTree1.GetTreeViewXML();
                doc1.Save(Server.MapPath("~/" + astvMyTree1.RootNodeValue + ".xml"));
            }
            if (cekOwner(astvMyTree2.RootNodeValue))
            {
                XmlDocument doc2 = astvMyTree2.GetTreeViewXML();
                doc2.Save(Server.MapPath("~/" + astvMyTree2.RootNodeValue + ".xml"));
            }
        }

        protected void btnSaveDragDrop_Click(object sender, EventArgs e)
        {
            saveAll();
        }

        private bool cekOwner(string productID)
        {
            String owner = (string)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, "select Username from ProductsTree where ProductID=" + productID);
            if (owner.Equals(Session["UserName"].ToString()))
                return true;
            else
                return false;
        }

        protected void astvMyTree_OnSelectedNodeChanged(object src, ASTreeViewNodeSelectedEventArgs e)
        {
            //tb1.Text = e.NodeText;
            ASTreeViewNode selectedNode = astvMyTree1.GetSelectedNode();
            if (selectedNode != null)
            {
                lblRoot.Text = selectedNode.NodeValue;

                tbItem.Text = (string)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, "select ProductName from ProductsTree where ProductID=" + lblRoot.Text);
                btnUpdate.Enabled = cekOwner(ddlRoot1.SelectedValue);
                if (btnUpdate.Enabled)
                    btnUpdate.Enabled = cekOwner(lblRoot.Text);
            }
        }
        
        protected void astvMyTree2_OnSelectedNodeChanged(object src, ASTreeViewNodeSelectedEventArgs e)
        {
            //tb1.Text = e.NodeText;
            ASTreeViewNode selectedNode = astvMyTree2.GetSelectedNode();
            if (selectedNode != null)
            {
                lblRoot2.Text = selectedNode.NodeValue;
                tbItem2.Text = (string)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, "select ProductName from ProductsTree where ProductID=" + lblRoot2.Text);
                btnUpdate2.Enabled = cekOwner(ddlRoot2.SelectedValue);
                if (btnUpdate2.Enabled)
                    btnUpdate2.Enabled = cekOwner(lblRoot2.Text);
            }
        }

        protected void btnUpdat_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbItem.Text))
                return;

            if (String.IsNullOrEmpty(lblRoot.Text))
                return;

            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("UPDATE ProductsTree set ProductName='{0}' where ProductId={1}", tbItem.Text, lblRoot.Text));
            String qry = "SELECT SUBSTRING([ProductName], 1, CASE CHARINDEX(CHAR(10), [ProductName]) WHEN 0 THEN LEN([ProductName]) ELSE CHARINDEX(char(10), [ProductName]) - 1 END) as ProductName from [ProductsTree] where ProductID=" + lblRoot.Text;

            ASTreeViewNode selectedNode = astvMyTree1.FindByValue(lblRoot.Text);
            selectedNode.NodeText = (string)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, qry);

            prevText.Text = tbItem.Text;

            XmlDocument doc = astvMyTree1.GetTreeViewXML();
            doc.Save(Server.MapPath("~/" + ddlRoot1.SelectedValue + ".xml"));
            BindData();
            
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            /*
            if (String.IsNullOrEmpty(tbItem.Text))
                return;
            */

            string maxSql = string.Format("SELECT MAX(productId) from ProductsTree");
            int max = (int)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, maxSql);
            int newId = max + 1;

            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("INSERT INTO ProductsTree (ProductId, ProductName, ParentId, Username) VALUES({0},'{1}',{3},'{2}')", newId, "Untitled", Session["UserName"].ToString(),ddlRoot1.SelectedValue));
            String qry = "SELECT SUBSTRING([ProductName], 1, CASE CHARINDEX(CHAR(10), [ProductName]) WHEN 0 THEN LEN([ProductName]) ELSE CHARINDEX(char(10), [ProductName]) - 1 END) as ProductName from [ProductsTree] where ProductID=" + newId.ToString();

            ASTreeViewNode newNode = new ASTreeViewNode((string)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, qry), newId.ToString());
            List<KeyValuePair<string, string>> attrib=new List<KeyValuePair<string, string>>();

            ASTreeViewNode rootNode = astvMyTree1.FindByValue(ddlRoot1.Text);
            rootNode.AppendChild(newNode);
            
            XmlDocument doc = astvMyTree1.GetTreeViewXML();
            doc.Save(Server.MapPath("~/" + ddlRoot1.SelectedValue + ".xml"));
            BindData();
        }

        protected void btnAdd2_Click(object sender, EventArgs e)
        {
            /*
            if (String.IsNullOrEmpty(tbItem2.Text))
                return;
            */

            string maxSql = string.Format("SELECT MAX(productId) from ProductsTree");
            int max = (int)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, maxSql);
            int newId = max + 1;

            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("INSERT INTO ProductsTree (ProductId, ProductName, ParentId, Username) VALUES({0},'{1}',{3},'{2}')", newId, "Untitled", Session["UserName"].ToString(), ddlRoot2.SelectedValue));
            String qry = "SELECT SUBSTRING([ProductName], 1, CASE CHARINDEX(CHAR(10), [ProductName]) WHEN 0 THEN LEN([ProductName]) ELSE CHARINDEX(char(10), [ProductName]) - 1 END) as ProductName from [ProductsTree] where ProductID=" + newId.ToString();

            ASTreeViewNode newNode = new ASTreeViewNode((string)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, qry), newId.ToString());
            List<KeyValuePair<string, string>> attrib = new List<KeyValuePair<string, string>>();

            ASTreeViewNode rootNode = astvMyTree2.FindByValue(ddlRoot2.Text);
            rootNode.AppendChild(newNode);

            XmlDocument doc = astvMyTree2.GetTreeViewXML();
            doc.Save(Server.MapPath("~/" + ddlRoot2.SelectedValue + ".xml"));
            BindData();
        }

        protected void AddNewNode(string newText)
        {
            string maxSql = string.Format("SELECT MAX(productId) from ProductsTree");
            int max = (int)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, maxSql);
            int newId = max + 1;

            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("INSERT INTO ProductsTree (ProductId, ProductName, ParentId,Username) VALUES({0}, '{1}', 0,'{2}')", newId, newText, Session["UserName"].ToString()));
            String qry = "SELECT SUBSTRING([ProductName], 1, CASE CHARINDEX(CHAR(10), [ProductName]) WHEN 0 THEN LEN([ProductName]) ELSE CHARINDEX(char(10), [ProductName]) - 1 END) as ProductName from [ProductsTree] where ProductID=" + newId.ToString();
        }

        protected void btnUpdat2_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbItem2.Text))
                return;

            if (String.IsNullOrEmpty(lblRoot2.Text))
                return;

            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("Update ProductsTree set ProductName='{0}' where ProductId={1}", tbItem2.Text, lblRoot2.Text));
            String qry = "SELECT SUBSTRING([ProductName], 1, CASE CHARINDEX(CHAR(10), [ProductName]) WHEN 0 THEN LEN([ProductName]) ELSE CHARINDEX(char(10), [ProductName]) - 1 END) as ProductName from [ProductsTree] where ProductID=" + lblRoot2.Text;
            
            ASTreeViewNode selectedNode = astvMyTree2.FindByValue(lblRoot2.Text);
            selectedNode.NodeText = (string)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, qry);
            XmlDocument doc = astvMyTree2.GetTreeViewXML();
            doc.Save(Server.MapPath("~/" + ddlRoot2.SelectedValue + ".xml"));
            BindData();
        }

        protected void ddlRoot1_SelectedIndexChanged(object sender, EventArgs e)
        {
            saveAll();
            this.astvMyTree1.RootNode.Clear();
            BindData();

            btnAdd.Enabled = cekOwner(ddlRoot1.SelectedValue);
            tbItem.Text = "";
            lblRoot.Text = "0";
            this.astvMyTree1.ClearNodesSelection();
        }

        protected void ddlRoot2_SelectedIndexChanged(object sender, EventArgs e)
        {
            saveAll();
            this.astvMyTree2.RootNode.Clear();
            BindData();

            btnAdd2.Enabled = cekOwner(ddlRoot2.SelectedValue);
            tbItem2.Text = "";
            lblRoot2.Text = "0";
            this.astvMyTree2.ClearNodesSelection();
        }

        protected void btnShare_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbShare.Text))
                return;
            if (String.IsNullOrEmpty(ddlRoot1.SelectedValue))
                return;

            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("INSERT INTO UserAccess (ProductId, UserName) VALUES({0}, '{1}')", ddlRoot1.SelectedValue, tbShare.Text));
           
        }


        #region media
        [WebMethod]
        public static List<ListItem> GetMedia(int nodeID)
        {
            string query = "SELECT MediaID,MediaPath FROM Media where ProductID=" + nodeID;
            string constr = ConfigurationManager.ConnectionStrings["MaxCConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    List<ListItem> customers = new List<ListItem>();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            customers.Add(new ListItem
                            {
                                Value = sdr["MediaPath"].ToString(),
                                Text = sdr["MediaPath"].ToString()
                            });
                        }
                    }
                    con.Close();
                    return customers;
                }
            }
        }

        protected void FileUploadComplete(object sender, EventArgs e)
        {
            System.IO.Directory.CreateDirectory(Server.MapPath(lblRoot.Text));

            string filename = System.IO.Path.GetFileName(AsyncFileUpload1.FileName);
            AsyncFileUpload1.SaveAs(Server.MapPath(lblRoot.Text+"/") + filename);

            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("INSERT INTO Media (ProductId, MediaPath) VALUES({0}, '{1}')", lblRoot.Text, filename));
        }
        #endregion
    }
}
