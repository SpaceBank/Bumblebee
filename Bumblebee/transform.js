module.exports = function (callback, content, processes, sourceStateDiagramId, destinationStateDiagramId) {

    var transform = function (content, processes, sourceStateDiagramId, destinationStateDiagramId) {
        var data = JSON.parse(JSON.stringify(content.ops[0].scheme).replace(new RegExp(("conv[" + sourceStateDiagramId + "]").replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&'), "g"), "conv[" + destinationStateDiagramId + "]"));
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
                    }
                }

                var routes = [];

                for (var n = 0; n < keys.length; n++) {
                    var _keys = Object.keys(processes[keys[n]]);

                    routes = routes.concat(_keys.filter(function (_key) {
                        return processes[keys[n]][_key] == item.obj_id;
                    }));
                }

                if (routes.length > 0) {
                    item.params.push({
                        "name": "apiRoute",
                        "type": "string",
                        "descr": routes.join(";"),
                        "flags": [],
                        "regex": "",
                        "regex_error_text": ""
                    });
                }
            }
        }

        return JSON.stringify(data);
    }

    callback(null, transform(JSON.parse(content), JSON.parse(processes), sourceStateDiagramId, destinationStateDiagramId));
}