<%@ Page Title="Открытый грамматический словарь русского языка – oDict.ru" 
    Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="odict.ru._Default" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        h1
        {
            margin-top: 3em;
            margin-bottom: 2em;
        }
        
        ul.columns
        {
            width: 100%;
            clear: both;
        }

        ul.columns li
        {
            margin-left: 10%;
            margin-right: 10%;
            display: block;
            float: left;
            text-indent: 0px;
            text-align: center;
            width: 10em;
            margin-top: 1em;
            margin-bottom: 1em;
            padding-top: 1em;
            padding-bottom: 1em;
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

    <p> Топонимы на <b>-ино, цыно, -ово, -ево, -ёво</b> даны в словаре классическом склоняемом варианте: <b><code>Бородино 8 с 1в</code></b>.
        Склонять их или нет, зависит от приложения.  В целях порождения текста имеет смысл генерировать 
        современный несклоняемый вариант.  Для разбора текста не лишним будет учесть их возможную склоняемость.
        Понять, что перед нами такая статья, можно по характерному окончанию леммы и по заглавной букве.
        Заглавную букву необходимо учитывать, иначе можно зацепить такие слова как <b>вино, слово, марево.</b>
    </p>

    <h2>Другие открытые грамматические словари в Сети</h2>

    <ul>
        <li>Словарь А. А. Зализняка на <a href='http://starling.rinet.ru/downl.php?lan=ru'>сайте Сергея Старостина</a></li>
        <li><a href='http://opencorpora.org/dict.php'>Словарь OpenCorpora</a></li>
        <li><a href='http://ru.wiktionary.org/'>Викисловарь</a></li>
    </ul>


    <h2>Подписаться на новости «Открытого словаря»: </h2>

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

    <div id="disqus_thread"></div>
    <script type="text/javascript">
        var disqus_shortname = 'odict';

        (function () {
            var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;
            dsq.src = '//' + disqus_shortname + '.disqus.com/embed.js';
            (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
        })();
    </script>
    <noscript>Please enable JavaScript to view the <a href="http://disqus.com/?ref_noscript">comments powered by Disqus.</a></noscript>
</asp:Content>
