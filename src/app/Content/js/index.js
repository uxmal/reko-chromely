((window, $) => {
    function registerEventHandlers() {
        $("#cmdDasm").bind("click", async e => {
            $("#dasmWin").html("Simulating a slow load...");
            var sHtml = await window.reko.Proto_DisassembleRandomBytes("0010000", "00010");
            $("#dasmWin").html(sHtml);
        });
        $("#cmdOpenFile").click(e => {
            $("#openFile").trigger("click");
        });
        $("#cmdClearDasm").click(e => {
            $("#dasmWin").html("Cleared!");
        })
        $("#openFile").click(async e => {
            try {
                e.preventDefault();
                var filePath = await window.reko.OpenFile();
                await window.reko.LoadFile(filePath, null);    // Ask Reko to load the file.
                // updateAllWindows();                         // Now we can pull objects from the Reko instance (procedures, segments)
                // it would also update the image map, the "heat map" etc. But one thing at a time.
                loadProjectBrowser();
            } catch (err) {
                alert(`An error happened ${err.message} ${err.trace}`);
            }
        });
        $("#cmdGeneratePng").click(e => {
            //$TODO: pass a parameter
            //let image = await window.reko.Proto_GeneratePng();
        });
        $("#projectPane").on("dblclick", ".segment", e => {

            function getAttributes($node) {
                var attrs = {};
                let s = "";
                $.each($node[0].attributes, (index, attribute) => {
                    attrs[attribute.name] = attribute.value;
                    s += `${attribute.name}:${attribute.value}, `;
                });
                return s;
            }
            alert($(e.currentTarget).attr("data-addr"));
            //alert(getAttributes($(e.target)));
        });
    }

    function loadProjectBrowser() {
        let sHtml = window.reko.RenderProjectView();
        $("#projectPane").html(sHtml);
    }

    $(document).ready(() => {
        registerEventHandlers();
        let handlerId = window.reko.RegisterEventListener("Listener.Info", msg => {
            $("#diagnosticsWindow").append(msg);
        });
        console.log("Registered, id: " + handlerId);

        for (i = 0; i < 5; i++) {
            window.reko.TestListener();
        }
        loadProjectBrowser();
    });

    diagnostics = {
        clear: () => { },
        error: (nav, msg) => { alert(msg) }
    };
})(window, jQuery);
