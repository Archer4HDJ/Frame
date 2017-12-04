using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Modules
{
    public static class LocalizedLanguageTool
    {

        public static string LanguageNameToLocalizeName(string langName)
        {

            switch (langName)
            {
                case "Afrikaans":
                    return "Afrikaans";
                case "Arabic":
                    return "العربية";
                case "Basque":
                    return langName;
                case "Belarusian":
                    return "Belarusian";
                case "Bulgarian":
                    return "Български";
                case "Catalan":
                    return "Català";
                case "Czech":
                    return "Čeština";
                case "Danish":
                    return "danske";
                case "Dutch":
                    return "De Nederlandse";
                case "English":
                    return "English";
                case "Estonian":
                    return "eesti";
                case "Faroese ":
                    return langName;
                case "Finnish":
                    return "suomen";
                case "French":
                    return "le français";
                case "German":
                    return "deutsche";
                case "Greek":
                    return "Οι Έλληνες";
                case "Hebrew":
                    return langName;
                case "Hungarian":
                    return "magyar";
                case "Icelandic":
                    return langName;
                case "Indonesian ":
                    return "English";
                case "Italian":
                    return "italiano";
                case "Japanese":
                    return "日本語";
                case "Korean":
                    return "한국";
                case "Latvian":
                    return langName;
                case "Lithuanian":
                    return langName;
                case "Norwegian":
                    return langName;
                case "Polish":
                    return "polskie";
                case "Portuguese":
                    return "Português";
                case "Romanian":
                    return "română";
                case "Russian":
                    return "в россии";
                case "Serbo-Croatian":
                    return langName;
                case "Slovak":
                    return "slovaška";
                case "Slovenian":
                    return langName;
                case "Spanish":
                    return "Español";
                case "Swedish":
                    return "svenska";
                case "Thai":
                    return "ไทย";
                case "Turkish":
                    return langName;
                case "Ukrainian":
                    return langName;
                case "Vietnamese":
                    return "Việt Nam";
                case "ChineseSimplified":
                    return "简体中文";
                case "ChineseTraditional":
                    return "繁體中文";
                case "Chinese":
                    return "中文";

            }
            return langName;
        }
    }
}
