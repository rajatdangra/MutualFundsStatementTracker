﻿<%@ Page Title="Mutual Funds Statement Request" Language="C#" AutoEventWireup="true" CodeBehind="UserDetails.aspx.cs" Inherits="Mutual_Funds_Statement_Tracker.UserDetails" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="UserDetailsHeader" runat="server">
    <title>User Details</title>
</head>
<body>
    <form id="UserDetailsForm" runat="server" method="post" defaultfocus ="submit">

        <div id="UserDetailsFormError" runat="server">
            <span style="color: red">Please fill all mandatory fields.</span>
            <br />
            <br />
        </div>
        
        <input type="hidden" runat="server" id="rta_url" name="rta_url" />
        <table>
            <tr>
                <td><b>Mandatory Parameters</b></td>
            </tr>
            <tr>
                <td>Email: </td>
                <td>
                    <asp:TextBox ID="email" runat="server" /></td>
            </tr>
            <tr>
                <td>PAN: </td>
                <td>
                    <asp:TextBox ID="pan" runat="server" /></td>
            </tr>
            <tr>
                <td>Password: </td>
                <td colspan="3">
                    <asp:TextBox ID="password" runat="server" /></td>
            </tr>
            <tr>
                <td><b>Optional Parameters</b></td>
            </tr>
            <tr>
                <td>First Name: </td>
                <td>
                    <asp:TextBox ID="firstname" runat="server" /></td>
                <td>Last Name: </td>
                <td>
                    <asp:TextBox ID="lastname" runat="server" /></td>
            </tr>
            <tr>
                <td>Phone: </td>
                <td>
                    <asp:TextBox ID="phone" runat="server" /></td>
            </tr>
            <tr>
                <td colspan="4"></td>
            </tr>
        </table>
        <br />
        <asp:Button ID="submit" Text="Submit" Width="120px" BackColor="#3399ff" BorderColor="Black" ForeColor="White" Font-Bold="true" runat="server" OnClick="OnSubmitButtonClicked" />
        <div style="padding-top: 20px" id="UserDetailsResponse" runat="server">
            <span style="color: greenyellow"></span>
            <br />
            <br />
        </div>

    </form>
</body>
</html>