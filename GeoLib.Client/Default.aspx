<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GeoLib.Client.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <table>
    <tr>
    <td><asp:Label ID="lblZipCode" Text="ZipCode:" runat="server"></asp:Label></td>
    <td><asp:TextBox ID="txtZipCode" runat="server"></asp:TextBox></td>
    <td><asp:Label ID="lblState" Text="State:" runat="server"></asp:Label></td>
    <td><asp:TextBox ID="txtState" runat="server"></asp:TextBox></td>
    </tr>
    <tr><td></td> 
        <td><asp:Button ID="btnGetZipCode" runat="server" Text="Get Info" OnClick="btnGetZipCode_Click"/></td>
        <td></td>
        <td><asp:Button ID="btnGetState" runat="server" Text="Get Zip Codes" OnClick="btnGetState_Click"/></td>
    </tr>
    <tr>
        <td></td>
        <td><asp:TextBox ID="txtCityInfo" runat="server"></asp:TextBox></td>
        <td><asp:TextBox ID="txtStateInfo" runat="server"></asp:TextBox></td>
        <td></td>
        <td><asp:ListBox ID="lbStateData" runat="server"></asp:ListBox> </td>
    </tr>
    </table>
        <div>
            <asp:Button ID="btnSendMessage" runat="server" OnClick="btnSendMessage_Click" Text="Send Message to host"/>
        </div>
        <div>
            <asp:TextBox ID="txtMessage" runat="server"></asp:TextBox>
        </div>
        <div>
            <asp:Button ID="btnUpdateCityBatch" runat="server" OnClick="btnUpdateCityBatch_Click" Text="Update City Batch" />
        </div>
    </form>
</body>
</html>
