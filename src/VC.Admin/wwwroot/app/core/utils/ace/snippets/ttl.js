ace.define("ace/snippets/ttl",["require","exports","module"], function(require, exports, module) {
"use strict";

exports.snippetText = "# list\n\
snippet list\n\
	@list(${1}) {{\n\
\n\
	}}\n\
# if\n\
snippet if\n\
	@if(${1}) {{\n\
\n\
	}}\n\
# ifnot\n\
snippet ifnot\n\
	@else(${1}) {{\n\
\n\
	}}";
exports.scope = "ttl";

});
