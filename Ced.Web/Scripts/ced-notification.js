$(function () {
    $("#searchNotifForm").submit(function (e) {
        var btn = $("#btnSearch").html("<i class=\"fa fa-spinner fa-spin\" id=\"loading\"></i> Searching...").attr("disabled", true);
        e.preventDefault();
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            dataType: "html",
            success: function (result) {
                $("#tableHolder").html(result);
                btn.html("Search").attr("disabled", false);
            },
            error: function (xhr, textStatus, error) {
                swal({
                    title: "Error!",
                    text: xhr.statusText,
                    type: textStatus
                });
                btn.html("Search").attr("disabled", false);
            }
        });
    });
});