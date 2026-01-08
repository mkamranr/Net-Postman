// CodeMirror XML Mode
(function(CodeMirror) {
    "use strict";
    
    CodeMirror.defineMode("xml", function(config, parserConfig) {
        var indentUnit = config.indentUnit || 2;
        var multilineTagIndentFactor = parserConfig.multilineTagIndentFactor || 1;
        var multilineTagIndentPastTag = parserConfig.multilineTagIndentPastTag;
        
        var Kludges = parserConfig.htmlMode ? {
            autoSelfClosers: {
                'area': true, 'base': true, 'br': true, 'col': true, 'command': true,
                'embed': true, 'frame': true, 'hr': true, 'img': true, 'input': true,
                'keygen': true, 'link': true, 'meta': true, 'param': true, 'source': true,
                'track': true, 'wbr': true
            },
            implicitlyClosed: {
                'dd': true, 'li': true, 'optgroup': true, 'option': true, 'p': true,
                'rp': true, 'rt': true, 'tbody': true, 'td': true, 'tfoot': true,
                'th': true, 'tr': true
            },
            contextGrabbers: {
                'dd': {'dd': true, 'dt': true},
                'dt': {'dd': true, 'dt': true},
                'li': {'li': true},
                'option': {'option': true, 'optgroup': true},
                'optgroup': {'optgroup': true},
                'p': {'address': true, 'article': true, 'aside': true, 'blockquote': true, 'dir': true,
                    'div': true, 'dl': true, 'fieldset': true, 'footer': true, 'form': true,
                    'h1': true, 'h2': true, 'h3': true, 'h4': true, 'h5': true, 'h6': true,
                    'header': true, 'hgroup': true, 'hr': true, 'menu': true, 'nav': true, 'ol': true,
                    'p': true, 'pre': true, 'section': true, 'table': true, 'ul': true
                },
                'rp': {'rp': true, 'rt': true},
                'rt': {'rp': true, 'rt': true},
                'tbody': {'tbody': true, 'tfoot': true},
                'td': {'td': true, 'th': true},
                'tfoot': {'tbody': true, 'tfoot': true},
                'th': {'td': true, 'th': true},
                'thead': {'tbody': true, 'tfoot': true},
                'tr': {'tr': true}
            },
            doNotIndent: {"pre": true},
            allowUnquoted: true,
            allowMissing: true,
            caseFold: true
        } : {
            autoSelfClosers: {},
            implicitlyClosed: {},
            contextGrabbers: {},
            doNotIndent: {},
            allowUnquoted: false,
            allowMissing: false,
            caseFold: false
        };
        
        var alignCDATA = parserConfig.alignCDATA || false;
        var allowMissingTagName = parserConfig.allowMissingTagName || false;
        
        function inText(stream, state) {
            var ch = stream.next();
            if (ch == "<") {
                if (stream.eat("!")) {
                    if (stream.eat("[")) {
                        if (stream.match("CDATA[")) {
                            return state.tokenize = inCData;
                        } else {
                            stream.match("]]>");
                            return "comment";
                        }
                    } else if (stream.eat("-")) {
                        if (stream.eat("-")) {
                            stream.skipTo("-->");
                            stream.match("-->");
                            return "comment";
                        }
                    } else if (stream.eat("?")) {
                        stream.skipTo("?>");
                        stream.match("?>");
                        return "meta";
                    } else {
                        state.tokenize = inTag;
                    }
                } else if (stream.eat("/")) {
                    state.tokenize = inClosingTag;
                } else {
                    state.tokenize = inOpeningTag;
                }
                return "tag bracket";
            } else if (ch == "&") {
                stream.eatWhile(/[=a-zA-Z0-9]/);
                stream.eat(";");
                return "atom";
            } else {
                stream.eatWhile(/[^&<]/);
                return null;
            }
        }
        
        function inCData(stream, state) {
            var ch;
            if ((ch = stream.next()) == "]" && stream.eat("]") && stream.eat(">")) {
                state.tokenize = inText;
                return "comment";
            }
            stream.match("]]>");
            return "comment";
        }
        
        function inTag(stream, state) {
            var ch = stream.next();
            if (ch == ">" || (ch == "/" && stream.eat(">"))) {
                state.tokenize = inText;
                return "tag bracket";
            }
            if (ch == "=") {
                state.tokenize = inAttribute;
                return "operator";
            }
            stream.eatWhile(/[^\s=\/>]/);
            return "tagName";
        }
        
        function inClosingTag(stream, state) {
            var ch = stream.next();
            if (ch == ">" || ch == "/") {
                state.tokenize = inText;
                return "tag bracket";
            }
            stream.eatWhile(/[^\s=>\/]/);
            return "tagName";
        }
        
        function inOpeningTag(stream, state) {
            var ch = stream.next();
            if (ch == ">" || ch == "/" || ch == "\n") {
                state.tokenize = inText;
                return "tag bracket";
            }
            if (ch == "=") {
                state.tokenize = inAttribute;
                return "operator";
            }
            if (ch == "<" && !allowMissingTagName) {
                state.tokenize = inText;
                return "tag bracket";
            }
            stream.eatWhile(/[^\s=>\/]/);
            return "tagName";
        }
        
        function inAttribute(stream, state) {
            var ch = stream.eat(/['"]/);
            if (!ch) {
                stream.skipTo(/[\s=/>]/);
                return "attribute";
            }
            if (ch == "'") {
                state.tokenize = inSingleQuotedAttribute;
            } else {
                state.tokenize = inDoubleQuotedAttribute;
            }
            return "string";
        }
        
        function inDoubleQuotedAttribute(stream, state) {
            var ch;
            while ((ch = stream.next()) !== null) {
                if (ch == "\"") {
                    state.tokenize = inTag;
                    return "string";
                } else if (ch == "&") {
                    stream.eatWhile(/[^=&]/);
                    stream.eat(";");
                }
            }
            return "string";
        }
        
        function inSingleQuotedAttribute(stream, state) {
            var ch;
            while ((ch = stream.next()) !== null) {
                if (ch == "'") {
                    state.tokenize = inTag;
                    return "string";
                } else if (ch == "&") {
                    stream.eatWhile(/[^='&]/);
                    stream.eat(";");
                }
            }
            return "string";
        }
        
        return {
            startState: function() {
                return {
                    tokenize: inText,
                    cc: [],
                    context: null,
                    indented: 0
                };
            },
            
            token: function(stream, state) {
                if (stream.sol()) {
                    state.indented = stream.indentation();
                }
                return state.tokenize(stream, state);
            },
            
            indent: function(state, textAfter) {
                return state.indented;
            },
            
            electricInput: /^\s*<\/?[^\s>/]*(?:\/?>)?$/,
            blockCommentStart: "<!--",
            blockCommentEnd: "-->",
            fold: "xml",
            helperType: "xml"
        };
    });
    
    CodeMirror.defineMIME("text/xml", "xml");
    CodeMirror.defineMIME("application/xml", "xml");
    CodeMirror.defineMIME("text/html", { name: "xml", htmlMode: true });
    
})(window.CodeMirror);
