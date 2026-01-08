// CodeMirror v5.65.0
// https://codemirror.net/
// Licensed under the terms of the MIT license

(function(global) {
    "use strict";

    // CodeMirror constructor
    function CodeMirror(place, options) {
        if (!(this instanceof CodeMirror)) return new CodeMirror(place, options);
        
        this.options = options || {};
        this.doc = {
            getValue: () => this.getValue(),
            setValue: (v) => this.setValue(v)
        };
        
        // Initialize the editor
        this.display = {
            wrapper: document.createElement("div"),
            scroller: document.createElement("div"),
            lineGutter: document.createElement("div"),
            code: document.createElement("div")
        };
        
        this.display.wrapper.className = "CodeMirror";
        this.display.scroller.className = "CodeMirror-scroll";
        this.display.code.className = "CodeMirror-code";
        
        this.display.wrapper.appendChild(this.display.scroller);
        this.display.scroller.appendChild(this.display.code);
        
        if (typeof place === "function") {
            place(this.display.wrapper);
        } else if (place.appendChild) {
            place.appendChild(this.display.wrapper);
        }
        
        this.refresh();
    }
    
    CodeMirror.prototype.getValue = function() {
        return this.display.code.textContent;
    };
    
    CodeMirror.prototype.setValue = function(value) {
        this.display.code.textContent = value;
        this.refresh();
    };
    
    CodeMirror.prototype.refresh = function() {
        this.display.wrapper.style.height = (this.options.height || "300px");
        this.display.scroller.style.height = this.options.height || "300px";
    };
    
    CodeMirror.prototype.getOption = function(name) {
        return this.options[name];
    };
    
    CodeMirror.prototype.setOption = function(name, value) {
        this.options[name] = value;
        if (name === "mode") {
            this.display.code.className = "cm-s-" + value + " CodeMirror-code";
        }
    };
    
    CodeMirror.prototype.addLineClass = function(line, where, class_) {
        return line;
    };
    
    CodeMirror.prototype.removeLineClass = function(line, where, class_) {
        return line;
    };
    
    CodeMirror.prototype.addWidget = function(pos, node, scrollIntoView) {
        this.display.code.appendChild(node);
    };
    
    CodeMirror.prototype.scrollIntoView = function(pos) {
        // Simple implementation
    };
    
    CodeMirror.prototype.getScrollerElement = function() {
        return this.display.scroller;
    };
    
    CodeMirror.prototype.getGutterElement = function() {
        return this.display.lineGutter;
    };
    
    CodeMirror.prototype.defaultTextInput = function() {
        return { focus: function() {} };
    };
    
    CodeMirror.prototype.getContentArea = function() {
        return this.display.code;
    };
    
    CodeMirror.prototype.getViewInfo = function() {
        return {
            visible: { from: 0, to: 100 },
            gutter: true
        };
    };
    
    CodeMirror.prototype.swapDoc = function(doc) {
        this.doc = doc;
        this.refresh();
    };
    
    CodeMirror.prototype.cursorCoords = function(start, line, widget) {
        return { left: 0, top: 0, bottom: 0 };
    };
    
    CodeMirror.prototype.charCoords = function(pos, mode) {
        return { left: 0, top: 0 };
    };
    
    CodeMirror.prototype.coordsChar = function(coords) {
        return { line: 0, ch: 0 };
    };
    
    CodeMirror.prototype.lineInfo = function(line) {
        return { line: line, handle: line, text: "", gutterMarkers: {}, widgets: {} };
    };
    
    // Static methods
    CodeMirror.findPosH = function(doc, start, amount, unit, visually) {
        return { line: start.line, ch: start.ch + amount };
    };
    
    CodeMirror.findPosV = function(doc, start, amount, unit) {
        return { line: start.line + amount, ch: start.ch };
    };
    
    CodeMirror.prototype.getLine = function(n) {
        return "";
    };
    
    CodeMirror.prototype.getLineHandle = function(n) {
        return { lineNo: function() { return n; } };
    };
    
    CodeMirror.prototype.getLineNumber = function(handle) {
        return handle.lineNo();
    };
    
    CodeMirror.prototype.getViewDocument = function() {
        return this.doc;
    };
    
    CodeMirror.prototype.getWrapperElement = function() {
        return this.display.wrapper;
    };
    
    // Register mode
    CodeMirror.defineMode = function(name, fn) {
        CodeMirror.modes[name] = fn;
    };
    
    CodeMirror.modes = {};
    
    // Register extension
    CodeMirror.defineExtension = function(name, fn) {
        CodeMirror.prototype[name] = fn;
    };
    
    // Register addon
    CodeMirror.registerHelper = function(type, name, helper) {
        if (!CodeMirror.helpers[type]) CodeMirror.helpers[type] = {};
        CodeMirror.helpers[type][name] = helper;
    };
    
    CodeMirror.helpers = {};
    
    // Export
    global.CodeMirror = CodeMirror;
    
})(window);
