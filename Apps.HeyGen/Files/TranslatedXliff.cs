namespace Apps.HeyGen.Files;

internal record TranslatedXliff(string Original, string TargetLanguage, Dictionary<string, TranslatedUnit> Units);
