ace.define("ace/snippets/ttl",["require","exports","module"], function(require, exports, module) {
"use strict";

exports.snippetText = "## STL Collections\n\
# list\n\
snippet list\n\
	@list(${1}) {{\n\
\n\
	}}\n\
# if\n\
snippet if\n\
	@if(${1}) {{\n\
\n\
	}}\n\
# else\n\
snippet else\n\
	@else(${1}) {{\n\
\n\
	}}";
exports.scope = "ttl";

});
