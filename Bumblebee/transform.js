module.exports = function (callback, content, processes, stateDiagramIds) {

    var transform = function (content, processes, stateDiagramIds) {

        var data = JSON.stringify(content);

        for (var i = 0; i < stateDiagramIds.length; i++) {
            data = data.replace(new RegExp(("conv[" + stateDiagramIds[i].sourceStateDiagramId + "]").replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&'), "g"), "conv[" + stateDiagramIds[i].destinationStateDiagramId  + "]");
        }

        data = JSON.parse(data);

        var keys = Object.keys(processes);

        for (var i = 0; i < data.length; i++) {
            var item = data[i];

            if (item.conv_type == "process") {
                if (!item.params) {
                    item["params"] = [];
                }

                for (var j = 0; j < item.params.length; j++) {
                    if (item.params[j].name == "apiRoute") {
                        item.params.splice(j, 1);
                        j--;
                    } else if (item.params[j].name == "apiRouteKeys") {
                        item.params.splice(j, 1);
                        j--;
                    }
                }

                var routes = [];
                var apiRouteKeys = [];

                for (var n = 0; n < keys.length; n++) {
                    var _keys = Object.keys(processes[keys[n]]);

                    var routesLength = routes.length;

                    routes = routes.concat(_keys.filter(function (_key) {
                        return processes[keys[n]][_key] == item.obj_id;
                    }));

                    if (routesLength != routes.length) {
                        apiRouteKeys.push(keys[n]);
                    }
                }

                if (routes.length > 0 && apiRouteKeys.length > 0) {
                    item.params.push({
                        "name": "apiRoute",
                        "type": "string",
                        "descr": routes.join(";"),
                        "flags": [],
                        "regex": "",
                        "regex_error_text": ""
                    });
                    item.params.push({
                        "name": "apiRouteKeys",
                        "type": "string",
                        "descr": apiRouteKeys.join(";"),
                        "flags": [],
                        "regex": "",
                        "regex_error_text": ""
                    });
                }
            }
        }

        return JSON.stringify(data);
    }

    callback(null, transform(JSON.parse(content), JSON.parse(processes), stateDiagramIds));
}