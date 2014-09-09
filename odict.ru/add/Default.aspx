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

        //function writeToConsole(text) {
        //    document.getElementById("console").innerHTML += "<span>[" + (new Date()).toISOString().substr(0, 19).replace("T", " ") + "] " + text + "</span><br />";
        //}

        var words = [];
        var lemmaLastValue;
        wordSuggestions.prototype.requestSuggestions = function (oAutoSuggestControl /*:AutoSuggestControl*/,
                                                                 bTypeAhead /*:boolean*/,
                                                                 textValue) {
            //            writeToConsole("requestSuggestions()");
            if (isMessageShown) {
                document.getElementById("<%= messageContainer.ClientID %>").style.display = "none";
                isMessageShown = false;
            }

            var lemmaText = (textValue || textValue === "" ? textValue : oAutoSuggestControl.textbox.value).trim();
            if (lemmaLastValue === lemmaText) {
                return;
            }
            lemmaLastValue = lemmaText;
            lemmaText = lemmaText.replace("<%= odict.ru.add.DictionaryHelper.StressMark %>", "")

            clearLineForms();

            //document.getElementById("line").innerHTML = lemmaText;

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
                        //writeToConsole("suggestionsText: " + xmlhttp.responseText);

                        var sTextboxValue = oAutoSuggestControl.textbox.value.replace("<%= odict.ru.add.DictionaryHelper.StressMark %>", "");

                        if (sTextboxValue.length > 0) {

                            //search for matching states
                            for (var i = 0; i < words.length; i++) {
                                if (words[i].toLowerCase().indexOf(sTextboxValue.toLowerCase()) == 0) {
                                    aSuggestions.push(words[i]);
                                }
                            }
                        }
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
            //writeToConsole("getRules()");
            document.getElementById("<%= selectedRule.ClientID %>").value = "";
            setSubmitAvailability(false);

            var rulesElement = document.getElementById("rules");
            rulesElement.length = 0;

            if (rulesRequest !== null) {
                rulesRequest.abort();
                rulesRequest = null;
            }

            if (!prefix) {
                rulesElement.style.display = "none";
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
                            newRule.value = rulesArray[ruleIndex];
                            rulesElement.add(newRule);
                        }
                    }
                    rulesElement.selectedIndex = -1;
                    rulesElement.style.display = rulesElement.length ? "block" : "none";
                    rulesRequest = null;
                }
            }

            xmlhttp.open("GET", "/api?action=getrules&prefixtext=" + encodeURI(prefix), true);
            xmlhttp.send();
        }

        var isMessageShown;
        function onloadpage() {
            //writeToConsole(navigator.appVersion);
            var lemmaElement = document.getElementById("<%= lemma.ClientID %>");
            lemmaElement.focus();
            var rulesElement = document.getElementById("rules");
            rulesElement.style.display = "none";
            new AutoSuggestControl(lemmaElement, new wordSuggestions(), selectWord, rulesElement);//, writeToConsole);

            document.getElementById('<%= selectedRule.ClientID %>').onkeydown = function (event) { return nextByKeyDown(event, '<%= submitAdd.ClientID %>') };

            setSubmitAvailability(false);

            var messageContainerElement = document.getElementById("<%= messageContainer.ClientID %>");
            isMessageShown = !!document.getElementById("<%= message.ClientID %>").innerHTML;
            messageContainerElement.style.display = isMessageShown ? "block" : "none";

            if (this.__is__Mobile) {
                var pageElement = document.getElementById("page");
                pageElement.insertBefore(pageElement.removeChild(messageContainerElement), document.getElementById("clearblock"));
                document.getElementById("rules").size = 1;
            }
        }
        addLoadEvent(onloadpage);
        function selectWord() {
            getRules(document.getElementById("<%= lemma.ClientID %>").value.trim());
        }
        function clearLineForms() {
            //document.getElementById("line").innerHTML = "";
            document.getElementById("forms").innerHTML = "";
        }
        function checkStressPosition(text) {
            var stressPosition = text.indexOf("<%= odict.ru.add.DictionaryHelper.StressMark %>");

            return stressPosition > 0 // is strees mark specified and it's position after the first letter
                && "<%= Slepov.Russian.Syllable.LowercaseVowels %>".indexOf(text[stressPosition - 1].toLowerCase()) !== -1; //previous letter has to be a vowel
        }
        function getLineForms(ruleValue, formsOnly) {
            clearLineForms();
            var formsElement = document.getElementById("forms");

            var lemmaValue = document.getElementById("<%= lemma.ClientID %>").value;

            if (!lemmaValue || !ruleValue) {
                return;
            }

            var xmlhttp = getxmlhttp();

            xmlhttp.onreadystatechange = function () {
                if (xmlhttp.readyState == 4) {
                    if (xmlhttp.status == 200) {
                        getResult = JSON.parse(xmlhttp.responseText);

                        if (!formsOnly) {
                            var spacePos = getResult.Line.indexOf(" ");
                            document.getElementById("<%= selectedRule.ClientID %>").value = getResult.Line.substr(spacePos + 1);
                        }

                        var formsHTML = "";
                        var formsArray = !formsOnly ? getResult.Forms : getResult;
                        for (var formIndex = 0; formIndex < formsArray.length; formIndex++) {
                            formsHTML += "<span>" + formsArray[formIndex] + "</span>" + (formIndex !== (formsArray.length - 1) ? "<br/>" : "");
                        }
                        formsElement.innerHTML = formsHTML;
                        setSubmitAvailability(checkStressPosition(lemmaValue));
                    }
                }
            }

            xmlhttp.open("GET", "/api?action=get" + (!formsOnly ? "lineforms&lemma=" + encodeURI(lemmaValue) : "forms") + "&rule=" + encodeURIComponent(ruleValue), true);
            xmlhttp.send();
        }
        function placeStressMark() {
            var lemmaElement = document.getElementById("<%= lemma.ClientID %>");
            var selectedRuleValue = document.getElementById("<%= selectedRule.ClientID %>").value;
            var stressPos = +selectedRuleValue.substr(0, selectedRuleValue.indexOf(" "));

            var lemmaElementValue = lemmaElement.value.replace("<%= odict.ru.add.DictionaryHelper.StressMark %>", "");
            if (stressPos > 0 && stressPos <= lemmaElementValue.length) {
                lemmaElementValue = lemmaElementValue.substr(0, stressPos) + "<%= odict.ru.add.DictionaryHelper.StressMark %>" + lemmaElementValue.substr(stressPos);
            }
            lemmaElement.value = lemmaElementValue;
            lemmaLastValue = lemmaElementValue;
        }
        function selectRule() {
            var rulesElement = document.getElementById("rules");
            getLineForms(rulesElement.options[rulesElement.selectedIndex].value, false);
        }
        function focusRule() {
            var rulesElement = document.getElementById("rules");
            if (rulesElement.length > 0 && rulesElement.selectedIndex === -1) {
                rulesElement.selectedIndex = 0;
                selectRule();
            }
        }
        function setSubmitAvailability(flag) {
            var submitAddElement = document.getElementById("<%= submitAdd.ClientID %>").disabled = flag ? "" : "disabled";
        }
        function nextByKeyDown(event, nextId) {
            var keyCode = event.keyCode ? event.keyCode : event.charCode;
            var next = document.getElementById(nextId);
            if (keyCode === 13) {
                next.focus();
                return false;
            }
        }
        var lastSelectedRuleValue = null;
        function ruleChange() {
            var selectedRuleValue = document.getElementById("<%= selectedRule.ClientID %>").value.trim();

            if (lastSelectedRuleValue === selectedRuleValue) {
                return;
            }
            lastSelectedRuleValue = selectedRuleValue;

            var rulesElement = document.getElementById("rules");
            placeStressMark();
            rulesElement.selectedIndex = -1;
            for (var ruleIndex = 0; ruleIndex < rulesElement.length; ruleIndex++) {
                if (selectedRuleValue.trim() === rulesElement.options[ruleIndex].value.substr(0, rulesElement.options[ruleIndex].value.indexOf("<%= odict.ru.add.DictionaryHelper.RuleLineDelimiter %>"))) {
                    rulesElement.selectedIndex = ruleIndex;
                }
            }

            getLineForms(document.getElementById("<%= lemma.ClientID %>").value.replace("<%= odict.ru.add.DictionaryHelper.StressMark %>", "") + " " + selectedRuleValue, true);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="messageContainer" CssClass="divMessage" runat="server">
        <asp:Label ID="message" runat="server"></asp:Label>
    </asp:Panel>
    <div class="block div250">
        <asp:TextBox ID="lemma" CssClass="lemma-textbox width100pr" TabIndex="1" runat="server" />
    </div>
    <div class="block div400 lmargin10px">
        <asp:TextBox ID="selectedRule" onkeyup="ruleChange()" TabIndex="3" CssClass="width100pr" runat="server"></asp:TextBox>
        <br />
        <select id="rules" class="ruleslist width100pr" tabindex="2" onchange="selectRule()" ondblclick="selectRule()" onfocus="focusRule()" onkeydown="return nextByKeyDown(event, '<%= selectedRule.ClientID %>')" size="4"></select>        
    </div>
    <div class="block lmargin10px">
        <asp:Button ID="submitAdd" UseSubmitBehavior="false" TabIndex="4" OnClientClick="this.disabled = true" CssClass="submitAdd width100pr" Text="Добавить" runat="server" />
    </div>
    <div id="clearblock" class="clearblock">
        <div id="forms" class="forms"></div>
    </div>
    <div id="console" style="color:red;font-size:small">  
    </div>
</asp:Content>
