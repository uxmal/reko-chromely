((window, $) => {
    $(document).ready(() => {
        $("#cmdDasm").bind("click", e => {
            // Ideally, Proto_DisassembleRandomBytes will return 
            // a Promise we can await.
            window.reko.Proto_DisassembleRandomBytes(sHtml => $("#dasmWin").html(sHtml));
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
