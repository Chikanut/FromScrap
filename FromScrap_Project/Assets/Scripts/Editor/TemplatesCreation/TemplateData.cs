using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TemplateData", menuName = "Templates/Create Template", order = 1)]
public class TemplateData : ScriptableObject
{
    [System.Serializable]
    public class TemplateFile
    {
        public string FileNameTemplate;
        public string FileNameReplace;
        
        public string Template;
        public List<string> ReplaceNames = new List<string>();
    }
    
    public List<TemplateFile> TemplateFiles = new List<TemplateFile>();

    public bool _isCreation;
}
