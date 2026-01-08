// CodeMirror JSON Mode
(function(CodeMirror) {
    "use strict";
    
    CodeMirror.defineMode("application/json", function(config, parserConfig) {
        var jsonMode = CodeMirror.getMode(config, {
            name: "javascript",
            json: true
        });
        
        return jsonMode;
    });
    
    CodeMirror.defineMIME("application/json", "application/json");
    
})(window.CodeMirror);
