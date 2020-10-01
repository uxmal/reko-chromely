((window, $) => {
    $(document).ready(() => {
        $("#cmdDasm").bind("click", async e => {
            var sHtml = await window.reko.Proto_DisassembleRandomBytes();
            $("#dasmWin").html(sHtml);
        });
        $("#cmdOpenFile").click(e => {
            $("#openFile").trigger("click");
        });
        $("#openFile").change(e => {
            //$TODO: window.reko.OpenFile(e.target.value, e => alert("Opened file" + e));
        });
    });

    diagnostics = {
        clear: () => { },
        error: (nav, msg) => { alert(msg) }
    };
})(window, jQuery);
