ace.define("ace/mode/folding/ttl",["require","exports","module","ace/lib/oop","ace/range","ace/mode/folding/fold_mode"], function (require, exports, module) {
    "use strict";

    var oop = require("../../lib/oop");
    var Range = require("../../range").Range;
    var BaseFoldMode = require("./fold_mode").FoldMode;

    var FoldMode = exports.FoldMode = function() {
        BaseFoldMode.call(this);
    };
    oop.inherits(FoldMode, BaseFoldMode);

    (function () {

        this.foldingStartMarker = /(\{\{|\<\%)((?:[^\}]|\}[^\}])|(?:[^\%]|\%[^\>]))*$/;
        this.foldingStopMarker = /^((?:[^\{]|\{[^\{])|(?:[^\<]|\<[^\%]))*/;
        this._getFoldWidgetBase = this.getFoldWidget;
        this.getFoldWidget = function (session, foldStyle, row) {
            var line = session.getLine(row);
            var match = line.match(this.foldingStartMarker);
            if (match) {
                if (match[1])
                    "start";
            }
            match = line.match(this.foldingStopMarker);
            if (match) {
                if (match[1])
                    "end";
            }
            return "";
        };

        this.getFoldWidgetRange = function (session, foldStyle, row, forceMultiline) {
            var line = session.getLine(row);

            var match = line.match(this.foldingStartMarker);
            if (match) {
                var i = match.index;

                if (match[1])
                    return this.openingBracketBlock(session, match[1], row, i);
                return "";
            }
            var match = line.match(this.foldingStopMarker);
            if (match) {
                var i = match.index + match[0].length;

                if (match[1])
                    return this.closingBracketBlock(session, match[1], row, i);

                return "";
            }
        };

        this.getSectionRange = function (session, row) {
            var line = session.getLine(row);
            var startIndent = line.search(/\S/);
            var startRow = row;
            var startColumn = line.length;
            row = row + 1;
            var endRow = row;
            var maxRow = session.getLength();
            while (++row < maxRow) {
                line = session.getLine(row);
                var indent = line.search(/\S/);
                if (indent === -1)
                    continue;
                if (startIndent > indent)
                    break;
                var subRange = this.getFoldWidgetRange(session, "all", row);

                if (subRange) {
                    if (subRange.start.row <= startRow) {
                        break;
                    } else if (subRange.isMultiLine()) {
                        row = subRange.end.row;
                    } else if (startIndent == indent) {
                        break;
                    }
                }
                endRow = row;
            }

            return new Range(startRow, startColumn, endRow, session.getLine(endRow).length);
        };
    }).call(FoldMode.prototype);

});

ace.define("ace/mode/ttl_highlight_rules",["require","exports","module","ace/lib/oop","ace/mode/text_highlight_rules"], function(require, exports, module) {
"use strict";

var oop = require("../lib/oop");
var TextHighlightRules = require("./text_highlight_rules").TextHighlightRules;

var TtlHighlightRules = function(options) {
    this.$rules = {
        start: [
            {
                token: "comment.block",
                regex : "\\@\\*",
                push : "comment"
            },
            {
                token: ["keyword.operator", "paren.lparen"],
                regex : "\\<\\%",
                push : "def"
            },
            {
                token: ["keyword.operator", "paren.lparen"],
                regex: "\\{\\{",
                push: "start"
            },
            {
                token: ["keyword.operator", "paren.rparen"],
                regex: "\\}\\}",
                next: "pop"
            },
            {
                token: ["keyword", "paren.lparen"],
                regex: "\\@\\{",
                push: "raw"
            },
            {
                token: "keyword",
                regex:"\\@",
                next: "out"
            }
        ],
        "out": [
            {
                token: "comment.block",
                regex : "\\@\\*",
                push : "comment"
            },
            {
                token: ["keyword.operator", "paren.lparen"],
                regex: "\\{\\{",
                push: "start"
            },
            {
                token: ["keyword.operator", "paren.rparen"],
                regex: "\\}\\}",
                next: "pop"
            },
            {
                token: "variable.language",
                regex: "[a-zA-Z_]+[a-zA-Z_0-9]*",
                next: "out"
            },
            {
                token: "punctuation.operator",
                regex: ":",
                next: "out"
            },
            {
                token: "paren.lparen",
                regex: "\\(",
                push: "out"
            },
            {
                token: "paren.rparen",
                regex: "\\)",
                next: "pop"
            },
            {
                token: "punctuation.operator",
                regex: ";",
                next: "start"
            },
            {
                token: ["keyword", "paren.lparen"],
                regex: "\\@\\{",
                next: "raw"
            },
            {
                token: ["keyword.operator", "paren.lparen"],
                regex : "\\<\\%",
                push : "def"
            },
            {
                token: "keyword",
                regex:"\\@",
                next: "out"
            },
            {
                token: "text",
                regex: "\\s+",
                next: "out"
            },
            {
                token: "text",
                regex: ".",
                next: "start"
            }
        ],
        "raw": [
            {
                token: ["keyword", "paren.rparen"],
                regex: "\\}\\@",
                next: "pop"
            },
            {
                token: "string.unquoted",
                regex: "(?:[^\\}]|\\}[^@])*",
                next: "raw"
            }
        ],
        "def" : [
            {
                token: "comment.block",
                regex : "\\@\\*",
                push : "comment"
            },
            {
                token: ["keyword.operator", "paren.rparen"],
                regex: "\\%\\>",
                next: "pop"
            },
            {
                token: ["punctuation.operator", "paren.lparen"],
                regex: "\\<",
                next: "def_name"
            },
            {
                token: ["keyword.operator", "paren.rparen"],
                regex: "\\{\\{",
                push: "start"
            },
            {
                token: "keyword.operator",
                regex: "::",
                next: "def_type"
            },
            {
                token: "text",
                regex: "\\s+",
                next: "def"
            }
        ],
        "def_type" : [
            {
                token: "comment.block",
                regex : "\\@\\*",
                push : "comment"
            },
            {
                token: "storage.type",
                regex: "[a-zA-Z_]+[a-zA-Z_0-9\\.\\+]*",
                next: "def"
            }
        ],
        "def_name" : [
            {
                token: "comment.block",
                regex : "\\@\\*",
                push : "comment"
            },
            {
                token: "support.function",
                regex: "[a-zA-Z_]+[a-zA-Z_0-9]*",
                next: "def_name"
            },{
                token: ["punctuation.operator", "paren.rparen"],
                regex: "\\>",
                next: "def"
            },
            {
                token: "keyword.operator",
                regex: ":",
                next: "def_name"
            }
        ],
        "comment" : [
            {
                token: "comment.block",
                regex: ".*?\\*\\@",
                next: "pop"
            },
            {
                token: "comment.block",
                regex: "[^\\*]|\\*[^\\@]",
                next: "comment"
            }
        ]
    };
    this.normalizeRules();
};

oop.inherits(TtlHighlightRules, TextHighlightRules);

exports.TtlHighlightRules = TtlHighlightRules;
});

ace.define("ace/mode/ttl_brace_outdent",["require","exports","module","ace/range"], function(require, exports, module) {
"use strict";

var Range = require("../range").Range;

var TTLMatchingBraceOutdent = function () { };

(function() {

    this.checkOutdent = function(line, input) {
        if (! /^\s+$/.test(line))
            return false;

        return /^(\s*\}\})|(\s*\%\>)/.test(input);
    };

    this.autoOutdent = function(doc, row) {
        var line = doc.getLine(row);
        var match = line.match(/^(\s*\}\})|(\s*\%\>)/);

        if (!match) return 0;

        var column = match[1].length;
        var openBracePos = doc.findMatchingBracket({row: row, column: column});

        if (!openBracePos || openBracePos.row == row) return 0;

        var indent = this.$getIndent(doc.getLine(openBracePos.row));
        doc.replace(new Range(row, 0, row, column-1), indent);
    };

    this.$getIndent = function(line) {
        return line.match(/^\s*/)[0];
    };

}).call(TTLMatchingBraceOutdent.prototype);

exports.TTLMatchingBraceOutdent = TTLMatchingBraceOutdent;
});

ace.define("ace/mode/behaviour/ttl",["require","exports","module","ace/lib/oop","ace/mode/behaviour","ace/token_iterator","ace/lib/lang"], function(require, exports, module) {
"use strict";

var oop = require("../../lib/oop");
var Behaviour = require("../behaviour").Behaviour;
var TokenIterator = require("../../token_iterator").TokenIterator;
var lang = require("../../lib/lang");

var SAFE_INSERT_IN_TOKENS =
    ["text", "paren.rparen", "punctuation.operator"];
var SAFE_INSERT_BEFORE_TOKENS =
    ["text", "paren.rparen", "punctuation.operator", "comment"];

var context;
var contextCache = {};
var initContext = function(editor) {
    var id = -1;
    if (editor.multiSelect) {
        id = editor.selection.index;
        if (contextCache.rangeCount != editor.multiSelect.rangeCount)
            contextCache = {rangeCount: editor.multiSelect.rangeCount};
    }
    if (contextCache[id])
        return context = contextCache[id];
    context = contextCache[id] = {
        autoInsertedBrackets: 0,
        autoInsertedRow: -1,
        autoInsertedLineEnd: "",
        maybeInsertedBrackets: 0,
        maybeInsertedRow: -1,
        maybeInsertedLineStart: "",
        maybeInsertedLineEnd: ""
    };
};

var getWrapped = function(selection, selected, opening, closing) {
    var rowDiff = selection.end.row - selection.start.row;
    return {
        text: opening + selected + closing,
        selection: [
                0,
                selection.start.column + 1,
                rowDiff,
                selection.end.column + (rowDiff ? 0 : 1)
            ]
    };
};

var TTLstyleBehaviour = function() {
    this.add("braces", "insertion", function(state, action, editor, session, text) {
        var cursor = editor.getCursorPosition();
        var line = session.doc.getLine(cursor.row);
        if (text == '{{') {
            initContext(editor);
            var selection = editor.getSelectionRange();
            var selected = session.doc.getTextRange(selection);
            if (selected !== "" && selected !== "{{" && editor.getWrapBehavioursEnabled()) {
                return getWrapped(selection, selected, '{{', '}}');
            } else if (TTLstyleBehaviour.isSaneInsertion(editor, session)) {
                if (/(\%\>|\}\}|\))/.test(line[cursor.column]) || editor.inMultiSelectMode) {
                    TTLstyleBehaviour.recordAutoInsert(editor, session, "}}");
                    return {
                        text: '{{}}',
                        selection: [2, 1]
                    };
                } else {
                    TTLstyleBehaviour.recordMaybeInsert(editor, session, "{{");
                    return {
                        text: '{{',
                        selection: [2, 1]
                    };
                }
            }
        } else if (text == '}}') {
            initContext(editor);
            var rightChar = line.substring(cursor.column, cursor.column + 1);
            if (rightChar == '}}') {
                var matching = session.$findOpeningBracket('}}', {column: cursor.column + 1, row: cursor.row});
                if (matching !== null && TTLstyleBehaviour.isAutoInsertedClosing(cursor, line, text)) {
                    TTLstyleBehaviour.popAutoInsertedClosing();
                    return {
                        text: '',
                        selection: [2, 1]
                    };
                }
            }
        } else if (text == "\n" || text == "\r\n") {
            initContext(editor);
            var closing = "";
            if (TTLstyleBehaviour.isMaybeInsertedClosing(cursor, line)) {
                closing = lang.stringRepeat("}}", context.maybeInsertedBrackets);
                TTLstyleBehaviour.clearMaybeInsertedClosing();
            }
            var rightChar = line.substring(cursor.column, cursor.column + 1);
            if (rightChar === '}}') {
                var openBracePos = session.findMatchingBracket({row: cursor.row, column: cursor.column+1}, '}}');
                if (!openBracePos)
                     return null;
                var next_indent = this.$getIndent(session.getLine(openBracePos.row));
            } else if (closing) {
                var next_indent = this.$getIndent(line);
            } else {
                TTLstyleBehaviour.clearMaybeInsertedClosing();
                return;
            }
            var indent = next_indent + session.getTabString();

            return {
                text: '\n' + indent + '\n' + next_indent + closing,
                selection: [1, indent.length, 1, indent.length]
            };
        } else {
            TTLstyleBehaviour.clearMaybeInsertedClosing();
        }
    });

    this.add("braces", "deletion", function(state, action, editor, session, range) {
        var selected = session.doc.getTextRange(range);
        if (!range.isMultiLine() && selected == '{{') {
            initContext(editor);
            var line = session.doc.getLine(range.start.row);
            var rightChar = line.substring(range.end.column, range.end.column + 1);
            if (rightChar == '}}') {
                range.end.column++;
                return range;
            } else {
                context.maybeInsertedBrackets--;
            }
        }
    });

    this.add("parens", "insertion", function(state, action, editor, session, text) {
        if (text == '(') {
            initContext(editor);
            var selection = editor.getSelectionRange();
            var selected = session.doc.getTextRange(selection);
            if (selected !== "" && editor.getWrapBehavioursEnabled()) {
                return getWrapped(selection, selected, '(', ')');
            } else if (TTLstyleBehaviour.isSaneInsertion(editor, session)) {
                TTLstyleBehaviour.recordAutoInsert(editor, session, ")");
                return {
                    text: '()',
                    selection: [1, 1]
                };
            }
        } else if (text == ')') {
            initContext(editor);
            var cursor = editor.getCursorPosition();
            var line = session.doc.getLine(cursor.row);
            var rightChar = line.substring(cursor.column, cursor.column + 1);
            if (rightChar == ')') {
                var matching = session.$findOpeningBracket(')', {column: cursor.column + 1, row: cursor.row});
                if (matching !== null && TTLstyleBehaviour.isAutoInsertedClosing(cursor, line, text)) {
                    TTLstyleBehaviour.popAutoInsertedClosing();
                    return {
                        text: '',
                        selection: [1, 1]
                    };
                }
            }
        }
    });

    this.add("parens", "deletion", function(state, action, editor, session, range) {
        var selected = session.doc.getTextRange(range);
        if (!range.isMultiLine() && selected == '(') {
            initContext(editor);
            var line = session.doc.getLine(range.start.row);
            var rightChar = line.substring(range.start.column + 1, range.start.column + 2);
            if (rightChar == ')') {
                range.end.column++;
                return range;
            }
        }
    });

    this.add("brackets", "insertion", function(state, action, editor, session, text) {
        if (text == '<%') {
            initContext(editor);
            var selection = editor.getSelectionRange();
            var selected = session.doc.getTextRange(selection);
            if (selected !== "" && editor.getWrapBehavioursEnabled()) {
                return getWrapped(selection, selected, '<%', '%>');
            } else if (TTLstyleBehaviour.isSaneInsertion(editor, session)) {
                TTLstyleBehaviour.recordAutoInsert(editor, session, "%>");
                return {
                    text: '<%%>',
                    selection: [1, 1]
                };
            }
        } else if (text == '%>') {
            initContext(editor);
            var cursor = editor.getCursorPosition();
            var line = session.doc.getLine(cursor.row);
            var rightChar = line.substring(cursor.column, cursor.column + 1);
            if (rightChar == '%>') {
                var matching = session.$findOpeningBracket('%>', {column: cursor.column + 1, row: cursor.row});
                if (matching !== null && TTLstyleBehaviour.isAutoInsertedClosing(cursor, line, text)) {
                    TTLstyleBehaviour.popAutoInsertedClosing();
                    return {
                        text: '',
                        selection: [1, 1]
                    };
                }
            }
        }
    });

    this.add("brackets", "deletion", function(state, action, editor, session, range) {
        var selected = session.doc.getTextRange(range);
        if (!range.isMultiLine() && selected == '<%') {
            initContext(editor);
            var line = session.doc.getLine(range.start.row);
            var rightChar = line.substring(range.start.column + 1, range.start.column + 2);
            if (rightChar == '%>') {
                range.end.column++;
                return range;
            }
        }
    });
};

    
TTLstyleBehaviour.isSaneInsertion = function(editor, session) {
    var cursor = editor.getCursorPosition();
    var iterator = new TokenIterator(session, cursor.row, cursor.column);
    if (!this.$matchTokenType(iterator.getCurrentToken() || "text", SAFE_INSERT_IN_TOKENS)) {
        var iterator2 = new TokenIterator(session, cursor.row, cursor.column + 1);
        if (!this.$matchTokenType(iterator2.getCurrentToken() || "text", SAFE_INSERT_IN_TOKENS))
            return false;
    }
    iterator.stepForward();
    return iterator.getCurrentTokenRow() !== cursor.row ||
        this.$matchTokenType(iterator.getCurrentToken() || "text", SAFE_INSERT_BEFORE_TOKENS);
};

TTLstyleBehaviour.$matchTokenType = function(token, types) {
    return types.indexOf(token.type || token) > -1;
};

TTLstyleBehaviour.recordAutoInsert = function(editor, session, bracket) {
    var cursor = editor.getCursorPosition();
    var line = session.doc.getLine(cursor.row);
    if (!this.isAutoInsertedClosing(cursor, line, context.autoInsertedLineEnd[0]))
        context.autoInsertedBrackets = 0;
    context.autoInsertedRow = cursor.row;
    context.autoInsertedLineEnd = bracket + line.substr(cursor.column);
    context.autoInsertedBrackets++;
};

TTLstyleBehaviour.recordMaybeInsert = function(editor, session, bracket) {
    var cursor = editor.getCursorPosition();
    var line = session.doc.getLine(cursor.row);
    if (!this.isMaybeInsertedClosing(cursor, line))
        context.maybeInsertedBrackets = 0;
    context.maybeInsertedRow = cursor.row;
    context.maybeInsertedLineStart = line.substr(0, cursor.column) + bracket;
    context.maybeInsertedLineEnd = line.substr(cursor.column);
    context.maybeInsertedBrackets++;
};

TTLstyleBehaviour.isAutoInsertedClosing = function(cursor, line, bracket) {
    return context.autoInsertedBrackets > 0 &&
        cursor.row === context.autoInsertedRow &&
        bracket === context.autoInsertedLineEnd[0] &&
        line.substr(cursor.column) === context.autoInsertedLineEnd;
};

TTLstyleBehaviour.isMaybeInsertedClosing = function(cursor, line) {
    return context.maybeInsertedBrackets > 0 &&
        cursor.row === context.maybeInsertedRow &&
        line.substr(cursor.column) === context.maybeInsertedLineEnd &&
        line.substr(0, cursor.column) == context.maybeInsertedLineStart;
};

TTLstyleBehaviour.popAutoInsertedClosing = function() {
    context.autoInsertedLineEnd = context.autoInsertedLineEnd.substr(1);
    context.autoInsertedBrackets--;
};

TTLstyleBehaviour.clearMaybeInsertedClosing = function() {
    if (context) {
        context.maybeInsertedBrackets = 0;
        context.maybeInsertedRow = -1;
    }
};



oop.inherits(TTLstyleBehaviour, Behaviour);

exports.TTLstyleBehaviour = TTLstyleBehaviour;
});

ace.define("ace/mode/ttl_completions",["require","exports","module","ace/token_iterator"], function(require, exports, module) {
"use strict";

var TokenIterator = require("../token_iterator").TokenIterator;

var defaultExtensions = [
    "date",
    "time",
    "html",
    "guid",
    "int",
    "list",
    "model",
    "money",
    "out",
    "param",
    "partial",
    "string",
    "if",
    "else",
    "import",
    "using"
];

function is(token, type) {
    return token.type == type;
}

var TtlCompletions = function() {

};

(function() {

    this.getCompletions = function(state, session, pos, prefix) {
        var token = session.getTokenAt(pos.row, pos.column);

        if (!token)
            return [];

        if (is(token, "out-start"))
            return this.getExtensionCompletions(state, session, pos, prefix);

        if (is(token, "def_override"))
            return this.getExtensionOverrideCompletions(state, session, pos, prefix);

        return [];
    };

    this.getExtensionCompletions = function (state, session, pos, prefix) {
        return defaultExtensions.map(function (name) {
            return {
                caption: name,
                snippet: name + '($0)',
                meta: "variable",
                score: Number.MAX_VALUE
            };
        });
    };
    this.getExtensionOverrideCompletions = function (state, session, pos, prefix) {
        return defaultExtensions.map(function (name) {
            return {
                caption: name,
                snippet: name,
                meta: "variable",
                score: Number.MAX_VALUE
            };
        });
    };

}).call(TtlCompletions.prototype);

exports.TtlCompletions = TtlCompletions;
});

ace.define("ace/mode/ttl",["require","exports","module","ace/lib/oop","ace/mode/text","ace/mode/folding/ttl","ace/mode/ttl_highlight_rules","ace/mode/ttl_brace_outdent","ace/mode/behaviour/ttl","ace/mode/ttl_completions","ace/mode/folding/ttl"], function(require, exports, module) {
"use strict";

var oop = require("../lib/oop");
var TextMode = require("./text").Mode;
var TtlFoldMode = require("./folding/ttl").FoldMode;
var TtlHighlightRules = require("./ttl_highlight_rules").TtlHighlightRules;
var TTLMatchingBraceOutdent = require("./ttl_brace_outdent").TTLMatchingBraceOutdent;
var TTLstyleBehaviour = require("./behaviour/ttl").TTLstyleBehaviour;
var TtlCompletions = require("./ttl_completions").TtlCompletions;
var FoldMode = require("./folding/ttl").FoldMode;

var Mode = function() {
    this.HighlightRules = TtlHighlightRules;
    this.$completer = new TtlCompletions();
    this.$outdent = new TTLMatchingBraceOutdent();
    this.$behaviour = new TTLstyleBehaviour();
    this.foldingRules = new TtlFoldMode();
};
oop.inherits(Mode, TextMode);

(function() {
    this.blockComment = { start: "@*", end: "*@" };
    this.getNextLineIndent = function(state, line, tab) {
        var indent = this.$getIndent(line);

        var tokenizedLine = this.getTokenizer().getLineTokens(line, state);
        var tokens = tokenizedLine.tokens;
        var endState = tokenizedLine.state;

        if (tokens.length && tokens[tokens.length-1].type == "comment") {
            return indent;
        }

        var match = line.match(/^.*?(\<\%|\{\{)\s*$/);
        if (match) {
            indent += tab;
        }
        return indent;
    };

    this.checkOutdent = function(state, line, input) {
        return this.$outdent.checkOutdent(line, input);
    };

    this.autoOutdent = function(state, doc, row) {
        this.$outdent.autoOutdent(doc, row);
    };

    this.$id = "ace/mode/ttl";
}).call(Mode.prototype);

exports.Mode = Mode;
});
