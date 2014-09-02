using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using Zalizniak;

namespace odict.ru.add
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string trimmedLemma = this.LemmaTextBox.Text.Trim ();

            int stressPos = trimmedLemma.IndexOf ('*');


            string lemma = RemoveStressMarks (trimmedLemma);

            

            Control trigger = GetAsyncPostBackControl ();

            if (trigger == this.ModelsListBox)
            {
                string gramInfo = ModelsListBox.Text.Substring (0, ModelsListBox.Text.IndexOf ('\t'));

                string gramInfoLessStress = gramInfo.Substring (gramInfo.IndexOf (' '));

                string line = lemma.ToLowerInvariant () + " " + (stressPos == -1 ? "?" : stressPos.ToString ()) + " " + gramInfoLessStress;

                this.LineLabel.Text = line;

                var forms = FormGenerator.GetAccentedForms (line, delegate {});

                this.FormsLiteral.Text = string.Join ("<br/>", forms.Select (f => HttpUtility.HtmlEncode (f.AccentedForm)));
            }
            else
            {
                this.ModelsListBox.Visible = lemma.Length > 1;

                if (this.ModelsListBox.Visible)
                {
                    var dawg = DawgSharp.Dawg<string>.Load (new MemoryStream (Resources.zalizniak), r => {var s = r.ReadString (); return s == "" ? null : s;});

                    int prefixLen = dawg.GetLongestCommonPrefixLength(lemma.Reverse ());

                    var matches = dawg.MatchPrefix (lemma.Reverse ().Take (prefixLen))
                        .GroupBy (kvp => kvp.Value, kvp => kvp)
                        .SelectMany (g => g.Take (1))
                        .Select (kvp => kvp.Value + "\t" + new string (kvp.Key.Reverse ().ToArray ()) )
                        .Take (10)
                        .ToArray ();

                    this.ModelsListBox.Items.Clear ();

                    this.ModelsListBox.Rows = matches.Length;

                    foreach (var match in matches)
                    {
                        this.ModelsListBox.Items.Add (match);
                    }
                }
            }
        }

        public static string RemoveStressMarks (string trimmedLemma)
        {
            return trimmedLemma.Replace ("*", "");
        }

        public string GetAsyncPostBackControlID()
        {
            string smUniqueId = ScriptManager1.UniqueID;
            string smFieldValue = Request.Form[smUniqueId];

            if (!String.IsNullOrEmpty(smFieldValue) && smFieldValue.Contains('|'))
            {
                return smFieldValue.Split('|')[1];
            }

            return String.Empty;
        }

        public Control GetAsyncPostBackControl ()
        {
            string clientID = GetAsyncPostBackControlID ();

            return string.IsNullOrEmpty (clientID) ? null 
                : EnumerateControlsRecursive (this)
                    .Where (c => c.ClientID != null)
                    .SingleOrDefault (c => clientID.EndsWith (c.ClientID.Replace ('_', '$')));
        }

        static IEnumerable <Control> EnumerateControlsRecursive (Control root)
        {
            yield return root;

            foreach (Control child in root.Controls)
            {
                foreach (var control in EnumerateControlsRecursive (child))
                {
                    yield return control;
                }
            }
        }
    }
}