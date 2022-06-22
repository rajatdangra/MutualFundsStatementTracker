<%@ Page Title="Mutual Funds Statement Request" Language="C#" AutoEventWireup="true" CodeBehind="UserDetails.aspx.cs" Inherits="Mutual_Funds_Statement_Tracker.MutualFundsStatementRequest" %>

<%@ Import Namespace="Mutual_Funds_Statement_Tracker" %>
<link href="Styles/UserDetails.css" rel="stylesheet" />

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<h1><i><u>Mutual Funds Statement Request</u></i></h1>
<head id="UserDetailsHeader" title="Mutual Funds Statement Request" runat="server">
    <title>User Details</title>
    
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script type="text/javascript">  
        $(document).ready(function () {
            $('#show_password').hover(function show() {
                //Change the attribute to text  
                $('#password').attr('type', 'text');
                $('.icon').removeClass('fa fa-eye-slash').addClass('fa fa-eye');
            },
                function () {
                    //Change the attribute back to password  
                    $('#password').attr('type', 'password');
                    $('.icon').removeClass('fa fa-eye').addClass('fa fa-eye-slash');
                });
            //CheckBox Show Password  
            $('#ShowPassword').click(function () {
                $('#password').attr('type', $(this).is(':checked') ? 'text' : 'password');
            });
        });
    </script>

</head>
<body style="margin-left: 10px; padding: 10px">
    <form id="UserDetailsForm" runat="server" method="post" defaultfocus="submit_btn">
        <br />
        <div id="UserDetailsFormError" runat="server" style="color: red">
            <span style="color: red">Please fill all mandatory fields.</span>
            <br />
            <br />
        </div>

        <input type="hidden" runat="server" id="rta_url" name="rta_url" />
        <table>
            <tr>
                <td style="width:180px">
                    <b><i><u>Mandatory Parameters</u></i></b>
                </td>
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
                <td>
                    <asp:TextBox ID="password" TextMode="Password" runat="server" />
                </td>
                <td>
                    <button id="show_password" class="btn btn-primary" title="Hover on the eye to show/hide the password" type="button">
                        <span class="fa fa-eye-slash icon"></span>
                    </button>
                </td>
                <td>
                    <span class="input-group-text">
                        <asp:CheckBox ID="ShowPassword" runat="server" ToolTip="Check to show/hide the password" CssClass="checkbox"/>
                    </span>
                </td>
            </tr>
        </table>
        <br />
        <table>
            <tr>
                <td style="width:180px">
                    <b><i><u>Optional Parameters</i></u></b>
                </td>
            </tr>
            <tr>
                <td>First Name: </td>
                <td <%--colspan="2"--%>>
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
            <tr hidden="hidden">
                <td>Save Details: </td>
                <td>
                    <asp:CheckBox ID="saveUserDetails" runat="server" ToolTip="User details will be saved" /></td>
            </tr>
            <tr>
                <td colspan="4"></td>
            </tr>
        </table>
        <br />
        <%--<asp:Button ID="submit_btn" Text="Submit" Width="120px" BackColor="#3399ff" BorderColor="Black" ForeColor="White" Font-Bold="true" runat="server" OnClick="OnSubmitButtonClicked" ToolTip="Submit details to RTA Url"  CssClass="button"/>--%>
        <div style="padding-bottom: 20px;">
            <button id="submit_btn" class="button" title="Submit details to RTA Url" onserverclick="OnSubmitButtonClicked" runat="server">Submit</button>
        </div>
        <div style="padding-top: 20px; color: green" id="UserDetailsResponse" runat="server">
            <span style="color: greenyellow"></span>
            <br />
            <br />
        </div>

    </form>
</body>
</html>
