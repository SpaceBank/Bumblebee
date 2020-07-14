module.exports = function (callback, content, processes) {

    var process = function (content, processes) {
        var data = content.ops[0].scheme;
        var keys = Object.keys(processes);

        for (var i = 0; i < data.length; i++) {
            var item = data[i];

            if (item.conv_type == "process") {
                if (item.params) {
                    for (var j = 0; j < item.params.length; j++) {
                        if (item.params[j].name == "apiRoute") {
                            var apiRoutes = item.params[j].descr.split(";");

                            for (var k = 0; k < apiRoutes.length; k++) {
                                for (var n = 0; n < keys.length; n++) {
                                    if (processes[keys[n]][apiRoutes[k]]) {
                                        processes[keys[n]][apiRoutes[k]] = item.obj_id.toString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return JSON.stringify(processes);
    }

    callback(null, process(JSON.parse(content), JSON.parse(processes)));
}