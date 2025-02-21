﻿using Application.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Common
{
    public enum  FileType
    {
        [Description(".jpg,.png,.jpeg")]
        Image,
        [Description(".pdf,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.txt")]
        Document,
        [Description(".zip,.rar,.7z")]
        Archive,
        [Description(".mp4,.avi,.mkv,.flv,.wmv,.mov,.webm")]
        Video,
        [Description(".mp3,.wav,.flac,.ogg,.wma")]
        Audio,
        [Description(".json,.xml,.csv,.tsv")]
        Data
    }
}
public static class FileTypeExtensions
{
    public static string GetDescription(this FileType enumValue)
    {
        object[] attr = enumValue.GetType().GetField(enumValue.ToString())!
            .GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attr.Length > 0)
            return ((DescriptionAttribute)attr[0]).Description;
        string result = enumValue.ToString();
        result = Regex.Replace(result, "([a-z])([A-Z])", "$1 $2");
        result = Regex.Replace(result, "([A-Za-z])([0-9])", "$1 $2");
        result = Regex.Replace(result, "([0-9])([A-Za-z])", "$1 $2");
        result = Regex.Replace(result, "(?<!^)(?<! )([A-Z][a-z])", " $1");
        return result;
    }

    public static string[] GetExtentionList(this FileType enumValue)
    {
        string result = enumValue.GetDescription();
        return result.Split(',').ToArray();
    }
}
