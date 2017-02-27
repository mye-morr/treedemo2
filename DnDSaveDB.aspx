<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DnDSaveDB.aspx.cs" Inherits="ASTreeViewDemo.DnDSaveDB" %>
<%@ Register Assembly="Goldtect.ASTreeView" Namespace="Goldtect" TagPrefix="astv" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>DND Tree with Text Area</title>
    <link href="<%#ResolveUrl("~/Scripts/astreeview/astreeview.css")%>" type="text/css" rel="stylesheet" />
    <link href="<%#ResolveUrl("~/Scripts/astreeview/contextmenu.css")%>" type="text/css" rel="stylesheet" />
    <link href="<%#ResolveUrl("~/Scripts/myStyle.css")%>" type="text/css" rel="stylesheet" />
    <script src="http://code.jquery.com/jquery-1.10.1.min.js"></script>
    
    <script src="<%#ResolveUrl("~/Scripts/astreeview/astreeview.min.js")%>" type="text/javascript"></script>
    <script src="<%#ResolveUrl("~/Scripts/astreeview/contextmenu.min.js")%>" type="text/javascript"></script>
    <script src="<%#ResolveUrl("~/Scripts/colresize.js")%>" type="text/javascript"></script>

    <script type="text/javascript">
        //parameter must be "elem", "newParent"
        var updatedText=[];

        function dndCompletedHandler(elem, newParent) {

            var nodeAdditionalAttr = JSON.parse(elem.getAttribute("additional-attributes"));
            var nodeValue = elem.getAttribute("treeNodeValue");

            var parentAdditionalAttr = JSON.parse(newParent.getAttribute("additional-attributes"));
            var parentValue = newParent.getAttribute("treeNodeValue");

            document.getElementById("<%#txtNodeValue.ClientID%>").value = nodeValue;
            document.getElementById("<%#txtNodeTreeName.ClientID%>").value = nodeAdditionalAttr.treeName;
            document.getElementById("<%#txtParentValue.ClientID%>").value = parentValue;
            document.getElementById("<%#txtParentTreeName.ClientID%>").value = parentAdditionalAttr.treeName;
        }

        function nodeSelectHandler(elem) {
            if(document.getElementById("<%#lblRoot.ClientID%>").value !=0)
            {
                if(document.getElementById("<%#prevText.ClientID%>").value != document.getElementById("<%#tbItem.ClientID%>").value)
                {
                    alert("your previous changes haven't been saved");
                    return false;
                }
            }

            var nodeAdditionalAttr = JSON.parse(elem.parentNode.getAttribute("additional-attributes"));
            document.getElementById("<%#tbItem.ClientID%>").value = nodeAdditionalAttr.LongText;
            document.getElementById("<%#lblRoot.ClientID%>").value = elem.parentNode.getAttribute("treeNodeValue");

            document.getElementById("<%#prevText.ClientID%>").value = nodeAdditionalAttr.LongText;
            GetMedia(document.getElementById("<%#lblRoot.ClientID%>").value);
           
        }

        function uploadComplete(sender,args) {
            GetMedia(document.getElementById("<%#lblRoot.ClientID%>").value);
            var imgDisplay = $get("imgDisplay");
            imgDisplay.style.cssText = "height:200px;width:200px";
            imgDisplay.src = document.getElementById("<%#lblRoot.ClientID%>").value + "/" + args.get_fileName();
        }

        function nodeSelectHandler2(elem) {
            var nodeAdditionalAttr = JSON.parse(elem.parentNode.getAttribute("additional-attributes"));
            document.getElementById("<%#tbItem2.ClientID%>").value = nodeAdditionalAttr.LongText;
            document.getElementById("<%#lblRoot2.ClientID%>").value = elem.parentNode.getAttribute("treeNodeValue");
        }

        function mediaSelectChange(ddl) {
            var imgDisplay = $get("imgDisplay");
            imgDisplay.style.cssText = "height:200px;width:200px";
            imgDisplay.src = document.getElementById("<%#lblRoot.ClientID%>").value + "/" + ddl.value;
        }

        function GetMedia(nodeID) {
            $.ajax({
                type: "POST",
                url: "DnDSaveDB.aspx/GetMedia",
                data: '{nodeID: ' + nodeID + '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    var lstMedia = $("[id*=lstMedia]");
                    lstMedia.empty();
                    $.each(r.d, function () {
                        lstMedia.append($("<option></option>").val(this['Value']).html(this['Text']));
                    });
                },
                failure: function (response) {
                    alert('fail');
                },
                error: function (response) {
                    alert('error');
                }
            });
        }

    </script>
    
</head>
<body>
    <form id="form1" runat="server">
        <div style="float:left;width:50%;">
            Hello <asp:Label runat="server" ID="lbUsername" /> (<a href="Default.aspx">logout</a>)
            
        </div>
        <div style="text-align:right;float:left;">Share selected root node to : 
            <asp:TextBox ID="tbShare" runat="server" ToolTip="Enter a Username" Placeholder="Enter a Username" />
            <asp:Button ID="btnShare" runat="server" Text="Share" OnClick="btnShare_Click" />
            <asp:Button ID="btnSaveDragDrop" runat="server" Text="Save My Root" OnClick="btnSaveDragDrop_Click" />
        </div>
    <div>
        <asp:Literal ID="lASTreeViewThemeCssFile" runat="server"></asp:Literal>
        <asp:ScriptManager ID="sm" runat=server></asp:ScriptManager>
       
        <asp:UpdatePanel ID="upPanel2" runat="server">
            <ContentTemplate>
            <div style="border:2px solid red;padding:6px;color:red;display:none;">

                <asp:TextBox ID="txtNodeValue" runat="server"></asp:TextBox>
                <asp:TextBox ID="txtNodeTreeName" runat="server"></asp:TextBox>
                <asp:TextBox ID="txtParentValue" runat="server"></asp:TextBox>
                <asp:TextBox ID="txtParentTreeName" runat="server"></asp:TextBox>
                <asp:TextBox ID="lblRoot" runat="server" Text="0" />
                <asp:TextBox ID="lblRoot2" runat="server" Text="0" />

                <asp:TextBox ID="prevText" runat="server" Text="0" />
            </div>
            <table id="sample" width="100%">
                <tr>
                    <th><h3><asp:Literal ID="aslTreeOne" runat="server" Text="TreeOne"></asp:Literal></h3></th>
                    <th>Selected Tree One Root : <asp:DropDownList runat="server" ID="ddlRoot1" DataValueField="ProductID" DataTextField="ProductName" OnSelectedIndexChanged="ddlRoot1_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList> </th>
                    <th>Selected Tree Two Root : <asp:DropDownList runat="server" ID="ddlRoot2" DataValueField="ProductID" DataTextField="ProductName" OnSelectedIndexChanged="ddlRoot2_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList> </th>
                    <th><h3><asp:Literal ID="Literal1" runat="server" Text="TreeTwo"></asp:Literal></h3></th>
                </tr>
                <tr valign="top">
                    <td >

                        <astv:ASTreeView ID="astvMyTree1" 
                            runat="server"
                            BasePath="~/Scripts/astreeview/"
                            DataTableRootNodeValue="0"
                            EnableCheckbox="false"
                            EnableNodeIcon="false"
                            EnableNodeSelection="true" 
                            EnableXMLValidation="true"
                            EnableDragDrop="true" 
                            EnableTreeLines="true"
                            AutoPostBack="false"
                            RelatedTrees="astvMyTree2" 
                            EnableContextMenuAdd="false"
                             EnableAjaxOnEditDelete="false"
                            OnNodeDragAndDropCompletedScript="dndCompletedHandler( elem, newParent )"
                            OnNodeSelectedScript="nodeSelectHandler(elem)"
                            />
                    </td>
                    <td  style="background-color:yellow"> 
                         <img id = "imgDisplay" alt="" src="" style = "display:block"/>
                        <table width="100%" border="0">
                            <tr>
                                <td>
                        Selected Left Node :</td>
                            </tr>
                             <tr>
                                <td><asp:TextBox ID="tbItem" runat="server" Text="" Rows="35" TextMode="multiline" Width="100%"  /> </td>
                            </tr>
                            <tr>
                                <td>
                                    <cc1:AsyncFileUpload  runat="server" ID="AsyncFileUpload1" OnClientUploadComplete="uploadComplete"
    Width="400px" UploaderStyle="Modern" CompleteBackColor="White" UploadingBackColor="#CCFFFF"
    ThrobberID="imgLoader" OnUploadedComplete="FileUploadComplete" />
                                    <asp:Image ID="imgLoader" runat="server"  ImageUrl = "~/Images/loader.gif" /> 
                                </td>
                            </tr>
                            <tr>
                                <td><asp:ListBox ID="lstMedia" runat="server" SelectionMode="Single" onchange="mediaSelectChange(this)"></asp:ListBox></td>
                            </tr>
                             
                             <tr>
                                <td><asp:Button ID="btnUpdate" runat="server" Text="Update selected left Node" OnClick="btnUpdat_Click" />
                        <asp:Button ID="btnAdd" runat="server" Text="Add to bottom left tree" OnClick="btnAdd_Click" /></td>
                            </tr>
                        </table>
                         
                       
                    </td>
                    <td  style="background-color:cyan"> 
                        <table width="100%" border="0">
                            <tr>
                                <td> Selected Right Node :</td>
                            </tr>
                            <tr>
                                <td><asp:TextBox ID="tbItem2" runat="server" Text="" Rows="35" TextMode="multiline" Width="100%" /> </td>
                            </tr>
                            <tr>
                                <td><asp:Button ID="btnUpdate2" runat="server" Text="Update selected right Node" OnClick="btnUpdat2_Click" />
                        <asp:Button ID="btnAdd2" runat="server" Text="Add to bottom right tree" OnClick="btnAdd2_Click" /></td>
                            </tr>
                        </table>
                        
                        
                       
                    </td>
                    <td >

                        <astv:ASTreeView ID="astvMyTree2" 
                            runat="server"
                            BasePath="~/Scripts/astreeview/"
                            EnableRoot="true"
                            EnableCheckbox="false"
                            EnableNodeIcon="false"
                            EnableNodeSelection="true" 
                            EnableDragDrop="true" 
                            EnableTreeLines="true"
                            AutoPostBack="false"
                            RelatedTrees="astvMyTree1" 
                            OnOnSelectedNodeChanged="astvMyTree2_OnSelectedNodeChanged"
                            EnableContextMenuAdd="false"
                            OnNodeDragAndDropCompletedScript="dndCompletedHandler( elem, newParent )"
                            OnNodeSelectedScript2="nodeSelectHandler(elem)"
                            />
                    </td>
                </tr>
            </table>
            </ContentTemplate>
           
        </asp:UpdatePanel>
       
    </div>
    </form>
     <script type="text/javascript">
         $(function () {
             $("#sample").colResizable({
                 liveDrag: true,
                 postbackSafe: true,
                 partialRefresh: true
             });
         });
         var postbackSample = function () {
             $("#sample").colResizable({
                 liveDrag: true,
                 postbackSafe: true,
                 partialRefresh: true
             });
         };

         var prm = Sys.WebForms.PageRequestManager.getInstance();

         prm.add_endRequest(postbackSample);

         


    </script>
</body>
</html>
