module.exports = function (callback, content, newParameterName, newParameterSendToApis) {

    var process = function (content, newParameterName, newParameterSendToApis) {
        var data = content;
        var lowerCamelCaseParam = newParameterName.substring(0, 1).toLowerCase() + newParameterName.substring(1);
        var upperCamelCaseParam = newParameterName.substring(0, 1).toUpperCase() + newParameterName.substring(1);

        for (var i = 0; i < data.length; i++) {
            var item = data[i];

            if (item.conv_type == "process") {
                if (item.params) {
                    if (item.params.filter(param => param.name == lowerCamelCaseParam).length == 0) {
                        item.params.push({
                            name: lowerCamelCaseParam,
                            type: "string",
                            descr: lowerCamelCaseParam,
                            flags: ["input"],
                            regex: "",
                            regex_error_text: ""
                        });
                    }
                }

                if (item.scheme.nodes) {
                    for (var j = 0; j < item.scheme.nodes.length; j++) {
                        var node = item.scheme.nodes[j];
                        if (node.condition && node.condition.logics) {
                            for (var k = 0; k < node.condition.logics.length; k++) {
                                var logic = node.condition.logics[k];
                                if (logic.type == "api_rpc") {
                                    logic.extra[lowerCamelCaseParam] = "{{" + lowerCamelCaseParam + "}}";
                                    logic.extra_type[lowerCamelCaseParam] = "string";
                                } else if (logic.type == "api_copy") {
                                    logic.data[lowerCamelCaseParam] = "{{" + lowerCamelCaseParam + "}}";
                                    logic.data_type[lowerCamelCaseParam] = "string";
                                } else if (logic.type == "api" && newParameterSendToApis == true) {
                                    logic.extra_headers[upperCamelCaseParam] = "{{" + lowerCamelCaseParam + "}}";
                                }
                            }
                        }
                    }
                }
            }
        }

        return JSON.stringify(content);
    }

    callback(null, process(JSON.parse(content), newParameterName, newParameterSendToApis));
}