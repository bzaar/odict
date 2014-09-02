<%@ Page Title="Добавление слова в словарь" 
    Language="C#" 
    MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" 
    CodeBehind="Default.aspx.cs" 
    Inherits="odict.ru.add.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .lemma-textbox
        {
            margin: 0px;
            padding: 0px;
        }
        .block
        {
        }
    </style>
    <script type="text/javascript">
        function clickButton1() {
            __doPostBack('<%= LemmaTextBox.ClientID %>', '');
        }
        function insertAtCursor(myField, myValue) {
            //IE support
            if (document.selection) {
                myField.focus();
                sel = document.selection.createRange();
                sel.text = myValue;
            }
            //MOZILLA and others
            else if (myField.selectionStart || myField.selectionStart == '0') {
                var startPos = myField.selectionStart;
                var endPos = myField.selectionEnd;
                myField.value = myField.value.substring(0, startPos)
            + myValue
            + myField.value.substring(endPos, myField.value.length);
                myField.selectionStart = startPos + myValue.length;
                myField.selectionEnd = startPos + myValue.length;
            } else {
                myField.value += myValue;
            }
        }
        function InsertStressMark() {
            insertAtCursor('<%= LemmaTextBox.ClientID %>', '');
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <ajaxToolkit:ToolkitScriptManager runat="server" ID="ScriptManager1" />

    <div class="block">
        <asp:TextBox runat="server" onkeyup="clickButton1();"  ID="LemmaTextBox" CssClass="lemma-textbox" />
        <asp:Button runat="server" Text="'" onclik="insertAtCursor();" />
    </div>

    <div class="block">
        <asp:UpdatePanel runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
                <asp:ListBox ID="ModelsListBox" runat="server" AutoPostBack="true" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="TextChanged" ControlID="LemmaTextBox" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

    <ajaxToolkit:AutoCompleteExtender 
        ID="AutoCompleteExtender1" 
        runat="server" 
        TargetControlID="LemmaTextBox" 
        ServiceMethod="GetCompletionList" 
        ServicePath="DictionaryService.asmx" 
        CompletionInterval="100" 
        MinimumPrefixLength="1"
        OnClientItemSelected="clickButton1" />

    <div class="block">
        <asp:UpdatePanel ID="FormsPanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
                <p>
                    <asp:Label runat="server" ID="LineLabel" />
                </p>
                <asp:Literal ID="FormsLiteral" runat="server"/>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="TextChanged" ControlID="LemmaTextBox" />
                <asp:AsyncPostBackTrigger EventName="TextChanged" ControlID="ModelsListBox" />
            </Triggers>
        </asp:UpdatePanel>
    </div>


    <script type="text/javascript" language='javascript'>
        var tempval = document.getElementById('<%=LemmaTextBox.ClientID%>');
        tempval.select();
    </script>

    
</asp:Content>
