((window, $) => {
    $(document).ready(() => {
        $("#cmdDasm").bind("click", async e => {
            $("#dasmWin").html("Simulating a slow load...");
            var sHtml = await window.reko.Proto_DisassembleRandomBytes();
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
            alert(`Opened file ${filePath}`);
        });
        $("#cmdGeneratePng").click(async e => {
            let data = await window.reko.Proto_GeneratePng();
            let blob = new Blob([Uint8Array.from(data)], { type: "image/png" });
            $("#imgGenerated").attr("src", URL.createObjectURL(blob));
        })
    });

    diagnostics = {
        clear: () => { },
        error: (nav, msg) => { alert(msg) }
    };
})(window, jQuery);
