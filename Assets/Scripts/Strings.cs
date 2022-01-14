using System.Collections;
using System.Collections.Generic;
using System.Globalization; 
using UnityEngine;

abstract public class StringsBase
{
    abstract public string String_AskExit();
    abstract public string String_Yes();
    abstract public string String_No();
}

public class StringsEnUs: StringsBase
{
    override public string String_AskExit() { return "Do you really want to Exit?"; }
    override public string String_Yes() { return "Yes"; }
    override public string String_No() { return "No"; }
}

public class StringsKoKR: StringsBase
{
    override public string String_AskExit() { return "정말로 나가시겠습니까?"; }
    override public string String_Yes() { return "예"; }
    override public string String_No() { return "아니오"; }
}

public class StringManager
{
    static StringsBase _instance = null;

    public static void InitializeStringManager()
    {
        if("ko-KR" == CultureInfo.CurrentCulture.Name)
        {
            _instance = new StringsKoKR();
        }
        else
        {
            // default
            _instance = new StringsEnUs();
        }
    }
    
    // Singleton instantiation
    public static StringsBase Instance
    {
        get
        {
            return _instance;
        }
    }
}