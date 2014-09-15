using System;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using DawgSharp;
using Zalizniak;

namespace odict.ru.add
{
    /// <summary>
    /// Summary description for Api
    /// </summary>
    public class Api : IHttpHandler
    {
        [DataContract]
        protected class LineForms
        {
            [DataMember]
            public string Line;
            [DataMember]
            public string[] Forms;
        }

        readonly HttpContext Context;

        public Api()
        {
            Context = HttpContext.Current;
        }
        
        private static string[] GetFormsByRule(string rule)
        {
            string[] Forms;
            try
            {
                Forms = FormGenerator.GetAccentedForms(rule, delegate { })
                    .Select(Func => HttpUtility.HtmlEncode(Func.AccentedForm)).ToArray();
            }
            catch (Exception exp)
            {
                Forms = new [] { exp.Message };
            }

            return Forms;
        }
        protected void GetForms(string rule)
        {
            WriteJSONToResponse(GetFormsByRule(rule));
        }

        protected void GetLineForms(string lemma, string rule)
        {            
            int StressPos = lemma.Trim().IndexOf(DictionaryHelper.StressMark);

            string GramInfo = rule.Substring(0, rule.IndexOf(DictionaryHelper.RuleLineDelimiter));

            string Line = DictionaryHelper.RemoveStressMarks(lemma).ToLowerInvariant() + " " + 
                (StressPos == -1 ? "?" : StressPos.ToString()) + GramInfo.Substring(GramInfo.IndexOf(' '));

            WriteJSONToResponse(new LineForms
                                    {
                    Line = Line,
                    Forms = GetFormsByRule(Line)
                });
        }

        protected void GetRules(string prefixText)
        {
            Dawg<string> Dawg;
            var PrefixText = DictionaryHelper.RemoveStressMarks(prefixText).ToLowerInvariant ().Reverse();
            
            try
            {
                using (Stream ReverseDict = new FileBasedDictionary(Context.Server).OpenReverseIndex())
                {
                    Dawg = Dawg<string>.Load(ReverseDict,
                        Func =>
                        {
                            string s = Func.ReadString();
                            return s == String.Empty ? null : s;
                        });
                }
                
                int PrefixLen = Dawg.GetLongestCommonPrefixLength(PrefixText);

                WriteJSONToResponse(Dawg.MatchPrefix(PrefixText.Take(PrefixLen))
                    .GroupBy(kvp => kvp.Value, kvp => kvp)
                    .SelectMany(g => g.Take(1))
                    .Select(kvp => kvp.Value + DictionaryHelper.RuleLineDelimiter + new string(kvp.Key.Reverse().ToArray()))
                    .Take(10)
                    .ToArray());
            }
            catch (Exception)
            {
                WriteJSONToResponse(new [] { "Доступ к словарю в данный момент отсутствует. Возможно происходит построение индексов." });//exp.Message });
                return;
            }
        }

        protected void WriteJSONToResponse<T>(T obj)
        {
            Context.Response.ContentType = "application/json";

            var RespData = new MemoryStream();

            var Serializer = new DataContractJsonSerializer(typeof(T));
            Serializer.WriteObject(RespData, obj);

            using (var Reader = new StreamReader(RespData))
            {
                RespData.Seek(0, SeekOrigin.Begin);
                Context.Response.Write(Reader.ReadToEnd());
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            string Action = context.Request.Params["action"];

            if (!String.IsNullOrEmpty(Action))
            {
                switch (Action.ToLower())
                {
                    case "getrules":
                        string PrefixText = context.Request.Params["prefixtext"];
                        if (PrefixText != null)
                        {
                            GetRules(PrefixText);
                            return;
                        }
                        break;
                    case "getforms":
                        string Rule0 = context.Request.Params["rule"];
                        if (!String.IsNullOrEmpty(Rule0))
                        {
                            GetForms(context.Server.UrlDecode(Rule0));
                            return;
                        }
                        break;
                    case "getlineforms":
                        string Lemma = context.Request.Params["lemma"];
                        string Rule1 = context.Request.Params["rule"];
                        if (!String.IsNullOrEmpty(Lemma) && !String.IsNullOrEmpty(Rule1))
                        {
                            GetLineForms(Lemma, Rule1);
                            return;
                        }
                        break;
                    default:
                        break;
                }
            }

            context.Response.Write("Invalid parameters");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}