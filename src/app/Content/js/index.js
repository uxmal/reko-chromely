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
        $("#openFile").change(e => {
            //$TODO: window.reko.OpenFile(e.target.value, e => alert("Opened file" + e));
        });
    });

    diagnostics = {
        clear: () => { },
        error: (nav, msg) => { alert(msg) }
    };
})(window, jQuery);
