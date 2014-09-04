<%@ Page Title="Добавление слова в словарь" 
    Language="C#" 
    MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" 
    CodeBehind="Default.aspx.cs" 
    Inherits="odict.ru.add.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/Styles/autosuggest.css" rel="stylesheet" />
    <script src="../Scripts/autosuggest2.js"></script>
    <script type="text/javascript">
        function getxmlhttp() {
            var xmlhttp;
            if (window.XMLHttpRequest) { xmlhttp = new XMLHttpRequest(); }
            else { xmlhttp = new ActiveXObject("Microsoft.XMLHTTP"); }

            return xmlhttp;
        }

        function wordSuggestions() {
        }

        var words = [];

        wordSuggestions.prototype.requestSuggestions = function (oAutoSuggestControl /*:AutoSuggestControl*/,
                                                                 bTypeAhead /*:boolean*/) {

            clearLineForms();

            var lemmaText = document.getElementById("lemma").value.trim().replace("*", "");

            getRules(lemmaText);

            var aSuggestions = [];

            if (lemmaText.length === 0) {
                words = [];
                oAutoSuggestControl.autosuggest(aSuggestions, false);
                return;
            }

            var xmlhttp = getxmlhttp();

            xmlhttp.onreadystatechange = function () {
                if (xmlhttp.readyState == 4) {
                    if (xmlhttp.status == 200) {
                        words = JSON.parse(xmlhttp.responseText).d;

                        var sTextboxValue = oAutoSuggestControl.textbox.value.replace("*", "");

                        if (sTextboxValue.length > 0) {

                            //search for matching states
                            for (var i = 0; i < words.length; i++) {
                                if (words[i].toLowerCase().indexOf(sTextboxValue.toLowerCase()) == 0) {
                                    aSuggestions.push(words[i]);
                                }
                            }
                        }

                        //if (aSuggestions.length === 1 && aSuggestions[0].toLowerCase() === sTextboxValue.toLowerCase()) {
                        //    aSuggestions.pop();
                        //}

                        //provide suggestions to the control
                        oAutoSuggestControl.autosuggest(aSuggestions, false);
                    }
                    else {
                        words = [];
                    }
                }
            }

            xmlhttp.open("POST", "/add/DictionaryService.asmx/GetCompletionList", true);
            xmlhttp.setRequestHeader("Content-Type", "application/json");
            xmlhttp.send(JSON.stringify({ prefixText: lemmaText, count: 10 }));
        }

        var rulesRequest = null;
        function getRules(prefix) {
            var rulesElement = document.getElementById("rules");
            rulesElement.length = 0;

            if (rulesRequest !== null) {
                rulesRequest.abort();
                rulesRequest = null;
            }

            if (!prefix) {
                return;
            }

            var xmlhttp = getxmlhttp();
            rulesRequest = xmlhttp;

            xmlhttp.onreadystatechange = function () {
                if (xmlhttp.readyState == 4) {
                    if (xmlhttp.status == 200) {
                        rulesArray = JSON.parse(xmlhttp.responseText);
                        for (var ruleIndex = 0; ruleIndex < rulesArray.length; ruleIndex++) {
                            var newRule = document.createElement("option");
                            newRule.text = rulesArray[ruleIndex];
                            rulesElement.add(newRule);
                        }
                    }
                    rulesRequest = null;
                }
            }

            xmlhttp.open("GET", "/api?action=getrules&prefixtext=" + prefix, true);
            xmlhttp.send();
        }

        var lemmaTextbox;
        function onloadpage() {
            var lemmaElement = document.getElementById("lemma");
            lemmaElement.focus();
            lemmaTextbox = new AutoSuggestControl(lemmaElement, new wordSuggestions(), selectWord);
        }
        window.onload = onloadpage;

        function selectWord() {
            getRules(document.getElementById("lemma").value.trim());
        }

        function clearLineForms() {
            document.getElementById("line").innerHTML = "";
            document.getElementById("forms").innerHTML = "";
        }

        function getLineForms() {
            clearLineForms();
            var lineElement = document.getElementById("line");
            var formsElement = document.getElementById("forms");

            var lemmaValue = document.getElementById("lemma").value;
            var ruleElement = document.getElementById("rules");

            if (!lemmaValue || !ruleElement.value) {
                return;
            }

            var ruleValue = ruleElement.value;//ruleElement.options[ruleElement.selectedIndex].value;

            var xmlhttp = getxmlhttp();

            xmlhttp.onreadystatechange = function () {
                if (xmlhttp.readyState == 4) {
                    if (xmlhttp.status == 200) {
                        lineForms = JSON.parse(xmlhttp.responseText);
                        lineElement.innerHTML = lineForms.Line;
                        var formsHTML = "";
                        for (var formIndex = 0; formIndex < lineForms.Forms.length; formIndex++) {
                            formsHTML += "<span>" + lineForms.Forms[formIndex] + "</span>" + (formIndex !== (lineForms.Forms.length - 1) ? "<br/>" : "");
                        }
                        formsElement.innerHTML = formsHTML;
                    }
                }
            }

            xmlhttp.open("GET", "/api?action=getforms&lemma=" + lemmaValue + "&rule=" + ruleElement.value, true);
            xmlhttp.send();
        }

        function selectRule() {
            getLineForms();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="block">
        <input type="text" id="lemma" class="lemma-textbox" />
        <input class="stressButton" type="button" value="'" id="insertStressMark" />
    </div>
    <div class="block">
        <select id="rules" class="ruleslist" onchange="selectRule()" size="2"></select>
    </div>
    <div class="clearblock">
        <p>
            <span id="line"></span>
        </p>
        <div id="forms"></div>
    </div>
</asp:Content>
