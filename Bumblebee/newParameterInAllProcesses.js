﻿module.exports = function (callback, content, processes, newParameterSource, newParameterName, newParameterSendToApis) {

    var process = function (content, processes, newParameterSource, newParameterName, newParameterSendToApis) {
        var data = content;
        var lowerCamelCaseParam = newParameterName.substring(0, 1).toLowerCase() + newParameterName.substring(1);
        var upperCamelCaseParam = newParameterName.substring(0, 1).toUpperCase() + newParameterName.substring(1);

        var processIds = [];
        var keys = Object.keys(processes.API);

        for (var i = 0; i < keys.length; i++) {
            var item = keys[i];

            processIds.push(processes.API[keys[i]])
        }

        for (var i = 0; i < data.length; i++) {
            var item = data[i];

            if (item.conv_type == "process") {
                if (!processIds.includes(item.obj_id.toString())) {
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
                }

                var processArgument = "{{" + lowerCamelCaseParam + "}}";
                if (processIds.includes(item.obj_id.toString()) && newParameterSource != "Empty") {
                    processArgument = "{{" + newParameterSource + "." + upperCamelCaseParam + "}}";
                }

                if (item.scheme.nodes) {
                    for (var j = 0; j < item.scheme.nodes.length; j++) {
                        var node = item.scheme.nodes[j];
                        if (node.condition && node.condition.logics) {
                            for (var k = 0; k < node.condition.logics.length; k++) {
                                var logic = node.condition.logics[k];
                                if (logic.type == "api_rpc") {
                                    logic.extra[lowerCamelCaseParam] = processArgument;
                                    logic.extra_type[lowerCamelCaseParam] = "string";
                                } else if (logic.type == "api_copy") {
                                    logic.data[lowerCamelCaseParam] = processArgument;
                                    logic.data_type[lowerCamelCaseParam] = "string";
                                } else if (logic.type == "api" && newParameterSendToApis == true) {
                                    logic.extra_headers[upperCamelCaseParam] = processArgument;
                                }
                            }
                        }
                    }
                }
            }
        }

        return JSON.stringify(content);
    }

    callback(null, process(JSON.parse(content), JSON.parse(processes), newParameterSource, newParameterName, newParameterSendToApis));
}