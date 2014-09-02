<%@ Page Title="Открытый грамматический словарь русского языка – oDict.ru" 
    Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="odict.ru._Default" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        li
        {
            text-indent: -2em;
            margin-left: 10%;
            margin-right: 10%;
            text-align: left;
        }

        li label
        {
            margin-left: 1em;
        }

        input[type=checkbox]
        {
          /* Double-sized Checkboxes */
          -ms-transform: scale(1.5); /* IE */
          -moz-transform: scale(1.5); /* FF */
          -webkit-transform: scale(1.5); /* Safari and Chrome */
          -o-transform: scale(1.5); /* Opera */
          padding: 10px;
        }

        input[type=submit]
        {
            font-size: 1.5em;
            padding: 0.5em;
        }

    </style>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Открытый грамматический словарь русского языка
    </h2>
    <p>
        oDict.ru – это открытый грамматический словарь русского языка.  
        «Открытый» означает, что он доступен для скачивания бесплатно и что любой желающий может его пополнять и настраивать под свои задачи.
    </p>
    <p>
        Составление словарей – очень трудоемкий процесс и только крупные компании могут позволить себе 
        содержать штат лингвистов-лексикографов для поддержания словарей в актуальном состоянии 
        и штат программистов для разработки инструментов для составления словарей и адаптации их под конкретные задачи.  
        Малому и среднему бизнесу такое не под силу, не говоря уже об энтузиастах-одиночках.  
        А иметь качественные словари хочется всем.
    </p>
    <p> Поэтому мы предлагаем объединить усилия всех заинтересованных лиц для составления одного грамматического словаря, доступного всем.
        А чтобы было с чего начинать, мы выложили на сайт свой словарь, результат 10-летней работы.
        Вы можете скачать его прямо сейчас.
    </p>
    <p> Как известно специалистам, мало составить словарь.  Нужно еще его правильно приготовить.  
        Отобрать слова, подходящие к текущей тематике.  
        Разобраться с омографами.  
        Слить словари из различных форматов в один, убрав дубли и устранив несоответствия.
        Преобразовать в выходной формат.
        Во всем этом вам поможет данный сайт.
    </p>

    <p> Очень скоро. </p>

    <p> Сейчас же мы в самом начале пути и хотим узнать ваше мнение:
    </p>

    <ul style="list-style-type: none">
        <li>
            <asp:CheckBox runat="server" ID="UseCheckBox" Checked="true" Text="Да, я хочу скачать бесплатный грамматический словарь!" />
        </li>
        <li>
            <asp:CheckBox runat="server" ID="ContributeCheckBox" Text="Я и сам приму участие в его пополнении." />
        </li>
        <li>
            <asp:CheckBox runat="server" ID="PayCheckBox" Text="Я, возможно, даже буду готов заплатить за использование фильтров и конвертеров словарей и за возможность получения обновлений через API."></asp:CheckBox>
        </li>
    </ul>

    <p>Введите свой имейл для перехода на страницу скачивания словаря: </p>

    <div style="text-align: center; padding-bottom: 10em">
        <div>
            <p>
                <asp:TextBox runat="server" ID="EmailTextBox" Width="20em" />
            </p>

            <p>
                <asp:Button runat="server" ID="SubmitButton" Text="Отправить и скачать словарь" 
                    onclick="SubmitButton_Click" />
            </p>

            <p>
                <asp:Label runat="server" ID="BadEmailLabel" Visible="false" EnableViewState="false" ForeColor="Red">
                    Не удалось отправить вам имейл. Проверьте правильность адреса.
                </asp:Label>
            </p>
        </div>
    </div>

</asp:Content>
