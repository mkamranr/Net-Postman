// CodeMirror JavaScript Mode
(function(CodeMirror) {
    "use strict";
    
    CodeMirror.defineMode("javascript", function(config, parserConfig) {
        var indentUnit = config.indentUnit || 2;
        var jsonldMode = parserConfig.jsonld;
        var jsonMode = parserConfig.json || jsonldMode;
        var isTS = parserConfig.typescript;
        var wordRE = parserConfig.wordCharacters || /[\w$\xa1-\uffff]/;
        
        var tokens = [];
        var conts = [];
        var style = {};
        
        function typeToString(type) {
            if (type.lexeme) return type.lexeme;
            return "";
        }
        
        function kw(type) {
            style[type] = "keyword";
            tokens.push(type);
        }
        
        function a(type) {
            style[type] = "atom";
            tokens.push(type);
        }
        
        function p(type) {
            style[type] = "property";
            tokens.push(type);
        }
        
        function f(type) {
            style[type] = "def";
            tokens.push(type);
        }
        
        return {
            startState: function() {
                return {
                    tokenizeData: null,
                    lastType: null,
                    cc: null,
                    expressionStart: false,
                    maybeEol: false,
                    context: null,
                    localVars: null,
                    macros: null,
                    parent: null,
                    lastToken: null
                };
            },
            
            token: function(stream, state) {
                if (stream.sol()) {
                    state.maybeEol = true;
                }
                
                if (state.tokenizeData) {
                    var newState = state.tokenizeData(stream, state);
                    if (newState) {
                        state.tokenizeData = null;
                        return newState;
                    }
                }
                
                var style = state.tokenize(stream, state);
                
                if (style === "comment") {
                    return "comment";
                }
                
                if (style === "meta") {
                    return "meta";
                }
                
                return style;
            },
            
            indent: function(state, textAfter) {
                if (state.tokenizeData) return CodeMirror.Pass;
                
                if (state.lastType === "operator" || state.lastType === "arrow") {
                    return indentUnit;
                }
                
                if (state.lastType === "}") {
                    return indentUnit;
                }
                
                return 0;
            },
            
            electricInput: /^\s*[}\]]$/,
            lineComment: "//",
            blockCommentStart: "/*",
            blockCommentEnd: "*/",
            fold: "brace",
            closeBrackets: "()[]{}''\"\"``",
            helperType: "javascript"
        };
    });
    
    CodeMirror.defineMIME("text/javascript", "javascript");
    CodeMirror.defineMIME("application/javascript", "javascript");
    CodeMirror.defineMIME("application/x-javascript", "javascript");
    CodeMirror.defineMIME("text/ecmascript", "javascript");
    CodeMirror.defineMIME("application/ecmascript", "javascript");
    CodeMirror.defineMIME("application/x-json", { name: "javascript", json: true });
    CodeMirror.defineMIME("application/ld+json", { name: "javascript", jsonld: true });
    CodeMirror.defineMIME("text/typescript", { name: "javascript", typescript: true });
    CodeMirror.defineMIME("application/typescript", { name: "javascript", typescript: true });
    
})(window.CodeMirror);
