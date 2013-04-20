(function () {
    $('#header-sign-in').click(function () {
        // calculate the values for center alignment
        var dialogTop = ($(window).height() / 3) - ($('#dialog-box').height() / 3);
        var dialogLeft = ($(window).width() / 2) - ($('#dialog-box').width() / 2);

        // assign values to the overlay and dialog box
        $('#dialog-overlay').css({ height: $(document).height(), width: $(document).width() }).show();
        $('#dialog-box').css({ top: dialogTop, left: dialogLeft }).show();

        // display the message
        //$('#dialog-message').html("asdasdasd");
    });

    $('a.btn-close, #dialog-overlay').click(function () {
        $('#dialog-overlay, #dialog-box').hide();
        return false;
    });

    $(window).resize(function () {
        //only do it if the dialog box is not hidden
        if (!$('#dialog-box').is(':hidden')) {
            $('#header-sign-in').trigger('click');
        }
    });

})();