using System.Collections.Generic;
using I2.Loc;
using UnityEditor;
using UnityEngine;

namespace EditorTools.Editor
{
    public static class EditorExtensions
    {
        public static string GetLocalizationTermField(Rect rect, string term, string label = "")
        {
           var mTermsArray = UpdateTermsList(term);
           
           var index = (term=="-" || term=="") ? mTermsArray.Length-1 : System.Array.IndexOf( mTermsArray, term );
           var newIndex = string.IsNullOrEmpty(label) ? EditorGUI.Popup(rect, index, mTermsArray) : EditorGUI.Popup(rect, label, index, mTermsArray);
            
           return mTermsArray[newIndex];
        }

        public static string GetLocalizationTermFieldLayout(string Term, string label = "")
        {
            var mTermsArray = UpdateTermsList(Term);
           
            var index = (Term=="-" || Term=="") ? mTermsArray.Length-1 : System.Array.IndexOf( mTermsArray, Term );
            var newIndex = string.IsNullOrEmpty(label) ? EditorGUILayout.Popup(index, mTermsArray) : EditorGUILayout.Popup(label, index, mTermsArray);
            
            return mTermsArray[newIndex];
        }
        
        public static string[] UpdateTermsList( string currentTerm )
        {
            string[] mTermsArray = null;
            List<string> Terms = LocalizationManager.GetTermsList();
        
            if (!string.IsNullOrEmpty(currentTerm) && currentTerm!="-" && !Terms.Contains(currentTerm))
                Terms.Add (currentTerm);

            Terms.Sort(System.StringComparer.OrdinalIgnoreCase);
            Terms.Add("");

            Terms.Add("<inferred from text>");
            Terms.Add("<none>");

            mTermsArray = Terms.ToArray();
            return mTermsArray;
        }
    }
}