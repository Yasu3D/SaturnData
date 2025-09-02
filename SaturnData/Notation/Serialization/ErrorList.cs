namespace SaturnData.Notation.Serialization;

internal static class ErrorList
{
    internal const string ErrorSat001 = "Error SAT001 : Invalid event structure."; 
    internal const string ErrorSat002 = "Error SAT002 : Invalid event attributes."; 
    internal const string ErrorSat003 = "Error SAT003 : Invalid object structure."; 
    internal const string ErrorSat004 = "Error SAT004 : Invalid object attributes."; 
    internal const string ErrorSat005 = "Error SAT005 : Invalid lane toggle structure."; 
    internal const string ErrorSat006 = "Error SAT006 : Invalid bookmark structure."; 
    internal const string ErrorSat007 = "Error SAT007 : A required value could not be found."; 
    internal const string ErrorSat008 = "Error SAT008 : A provided value was not in the valid format."; 
    internal const string ErrorSat009 = "Error SAT009 : A provided value is outside of the valid range."; 
    internal static string ErrorSat010(string type) => $"Error SAT010 : Type \"{type}\" was not recognized."; 
    internal const string ErrorSat011 = "Error SAT011 : No active layer was found while attempting to add objects to layers."; 
    internal const string ErrorSat012 = "Error SAT012 : A sub-object cannot be defined when no parent object exists."; 
    internal const string ErrorSat013 = "Error SAT013 : A reverse effect event must begin with REV_START."; 
    internal const string ErrorSat014 = "Error SAT014 : A REV_END event cannot have an earlier timestamp than REV_START."; 
    internal const string ErrorSat015 = "Error SAT015 : A REV_ZONE_END event must be preceded by REV_END."; 
    internal const string ErrorSat016 = "Error SAT016 : A REV_ZONE_END event cannot have an earlier timestamp than REV_END."; 
    internal const string ErrorSat017 = "Error SAT017 : A stop effect event must begin with STOP_START."; 
    internal const string ErrorSat018 = "Error SAT018 : A STOP_END event cannot have an earlier timestamp than STOP_START."; 
    internal const string ErrorSat019 = "Error SAT019 : No previously defined hold note was found."; 
    
    internal const string ErrorSat201 = "Error SAT201 : Invalid chart structure. Please make sure all header tags exist and are defined in the correct order: @COMMENTS - @GIMMICKS - @OBJECTS"; 
    
    internal const string ErrorMer001 = "Error MER001 : No #BODY declaration was found."; 
    internal const string ErrorMer002 = "Error MER002 : A required value could not be found."; 
    internal const string ErrorMer003 = "Error MER003 : A provided value was not in the valid format."; 
    internal const string ErrorMer004 = "Error MER004 : A provided value is outside of the valid range.";
    
    internal const string WarningSat020 = "Warning SAT020 : The last defined reverse effect event was incomplete and has been discarded.";
    internal const string WarningSat021 = "Warning SAT021 : The last defined stop effect event was incomplete and has been discarded.";
    internal const string WarningSat022 = "Warning SAT022 : The last defined hold note was incomplete and has been discarded.";
}