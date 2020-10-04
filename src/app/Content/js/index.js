((window, $) => {
    $(document).ready(() => {
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
            e.preventDefault();
            var filePath = await window.reko.OpenFile(e.target.value);
            await window.reko.LoadFile(filePath);       // make sense? Now there is a loaded instance.
            updateAllWindows();                         // this would suck in info from the REko object (procedures, segments)
            // it would also update the image map, the "heat map" etc. But one thing at a time.
            alert(`Opened file ${filePath}`);
        });
        $("#cmdGeneratePng").click(e => {
            //$TODO: pass a parameter
            //let image = await window.reko.Proto_GeneratePng();
        });
    });

    diagnostics = {
        clear: () => { },
        error: (nav, msg) => { alert(msg) }
    };
})(window, jQuery);
