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

        h1
        {
            margin-top: 3em;
            margin-bottom: 2em;
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
        
        .columns
        {
            width: 100%;
            clear: both;
        }

        .columns li
        {
            display: block;
            float: left;
            padding-left: 0px;
            text-indent: 0px;
            text-align: center;
            width: 10em;
            margin-top: 1em;
            margin-bottom: 1em;
            padding-top: 1em;
            padding-bottom: 1em;
            border: 1px solid green;
        }
    </style>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1>
        Открытый грамматический словарь русского языка
    </h1>
    <p>
        oDict.ru – это открытый словарь русского языка, основанный на «Грамматическом словаре» <span style="white-space: nowrap"> А. А. Зализняка</span>.  
        «Открытый» означает, что он доступен для скачивания бесплатно и что любой желающий может его пополнять и редактировать.
    </p>

    <ul class="columns">
        <li><a href="/add/">Пополнить словарь</a></li> 
        <li><a href="download/odict.zip">Скачать словарь</a></li>
    </ul>

    <div style="clear: both; height: 1px"></div>

    <pre style="float: left; margin-right: 2em; clear: both;">лемматизация 9 ж 7а
полнотекстовый 7.2 п 1а
мерчендайзер 8 мо 1а
масс-спектрометрист 17.2 мо 1а
антипиратский 8.1 п 3а!~ $пират
сервисный 2 п 1*а $сервис
Филиппины 7 мн. ж 1а
Минэкономразвития 14 с 0
Россотрудничество 8 с 1а
Битлы 5 мн. мо 1а
леггинсы 2 мн. м 1а
гриль 3 м 2а
интрузивный 7 п 1*а
промзона 6 ж 1а
рейтинг 2 м 1а
Швеция 3 ж 7а
Эстония 4 ж 7а
Антарктида 8 ж 1а
Арктика 1 ж 3а
деэскалация 8 ж 7а
Евросоюз 7 м 1а
госсекретарь 10 м 2в
Женева 4 ж 1а
женевский 4 п 3а!~ $Женева
Боинг 2 м 3а
экстремальный 8 п 1*а
выживание 6 с 7а
техногенный 7 п 1*а
личка 2 ж 3*а
характерно 7 н
перепрыгивание 7 с 7а
достраиваемость 6 ж 8а
когнитивистика 9 3а
флуктуация 7 ж 7а
сверхскопление 11 с 7а
Константинополь 11 м 2а
реабилитационный 12 п 1*а</pre>
    <p> Формат словаря &mdash; текстовый файл в кодировке Windows-1251 в архиве ZIP. Размер 735K. </p>

    <p> Большая часть словаря является одним из ранних изданий словаря Зализняка,
        переведенным в электронный вид Сергеем Старостиным.
        В версии Старостина исправлены найденные ошибки (в основном ошибки OCR).
        Добавлено более 1000 новых слов, из них немалую часть составляют топонимы.
        Каждое неочевидное слово тщательно проверялось с целью точно определить ударение и тип словоизменения.
        Для этого привлекались все доступные источники: словарь Агеенко, 6-е издание словаря Зализняка, словари на сайте gramota.ru, Википедия,
        статистика употреблений в интернете по данным поисковых систем.
    </p>
    <p> Порядок слов в словаре произвольный. Набор помет в основном соответствует версии Старостина, 
        который отличается от помет в бумажной версии Зализняка по форме, но не по содержанию.
        При желании легко находятся соответствия с бумажной версией.
    </p>
    <p> Были добавлены две новые пометы: 
    </p>

    <p>
        <code>М(на)</code> означает, что слово употребляется с предлогом <em>на</em> (а не <em>в</em>) в местном падеже (локативе). 
        Отличается от пометы <code>П2(на)</code> окончанием <em>-е</em> в этой форме (а не ударным <em>-у</em>).  Примеры:
    </p>

    <ul style="list-style-type: none">
        <li><code>рынок 2 м 3*а, М(на)</code> (на рынке)</li>
        <li><code>плот 3 м 1в, П2(на)</code> (на плоту)</li>
    </ul>

    <p>Помета <code>$</code> добавлена к отдельным прилагательным и указывает на мотивирующее существительное:</p>

    <ul style="list-style-type: none">
        <li><code>абонентный 5 п 1*а! $абонент</code></li>
        <li><code>адыгейский 5 п 3а!~ $Адыгея</code></li>
    </ul>

    <p> Были восстановлены сверкой с бумажной версией словаря ударения в нерегулярных формах:
    </p>

    <ul style="list-style-type: none">
        <li><code>близкий 3 п 3*а/с' @ _сравн._ блИже</code></li>
        <li><code>лиловый 4 п 1а @ _сравн._ лиловЕе</code></li>
        <li><code>большой 6 п 4в @ _кф_ велИк, великА, великО, великИ; _сравн._ бОльше</code></li>
    </ul>


    <h2>Подписаться на новости «Грамматического словаря»: </h2>

    <p> Мы уведомим вас о новых крупных пополнениях словаря и улучшениях на сайте.
        Отписаться можно в любой момент по ссылке в письме.
    </p>

    <div style="text-align: center; padding-bottom: 10em">
        <div>
            <p>
                Ваш email: <br />
                <asp:TextBox runat="server" ID="EmailTextBox" Width="15em" />
            </p>

            <p>
                
                <asp:Button runat="server" ID="SubmitButton" Text="Подписаться" 
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
