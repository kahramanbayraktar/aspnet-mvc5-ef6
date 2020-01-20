$("#eventSearchBox").select2(
    {
        placeholder: "Find events",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            quietMillis: 150,
            url: eventSearchUrl,
            dataType: "json",
            data: function (term, page) {
                return {
                    pageSize: pageSize,
                    pageNum: page,
                    searchTerm: term
                };
            },
            results: function (data, page) {
                var more = (page * pageSize) < data.Total;
                return { results: data.Results, more: more };
            }
        }
    });

$("#emailSearchBox").select2(
    {
        placeholder: "Find users by email",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            quietMillis: 150,
            url: userSearchUrl,
            dataType: "json",
            data: function (term, page) {
                return {
                    pageSize: pageSize,
                    pageNum: page,
                    searchTerm: term
                };
            },
            results: function (data, page) {
                var more = (page * pageSize) < data.Total;
                return { results: data.Results, more: more };
            }
        }
    });

$("#btnSearch").on("click", function (e) {
    e.preventDefault();
    btnSearch.html("<i class=\"fa fa-spinner fa-spin\" id=\"loading\"></i> Searching...").attr("disabled", true);
    refreshLogs();
});

function refreshLogs() {
    var eventId = $("#eventSearchBox").val();
    var userEmail = $("#emailSearchBox").val();
    var dayRange = $("#dayRange").val();

    if (eventId === null && userEmail === "" && dayRange === "") {
        toastr.warning("You must select at least one search option.");
        btnSearch.html("Search").attr("disabled", false);
        return;
    }
    $.get(logGetUrl + "?eventId=" + eventId + "&userEmail=" + userEmail + "&dayRange=" + dayRange, function (result) {
        $("#tableHolder").html(result);
        if (btnSearch.attr("disabled") === "disabled") {
            btnSearch.html("Search").attr("disabled", false);
        }
    });
}

$("body").on("click", ".view-log", function (e) {
        var detailsBtn = $(e.currentTarget);
        var logId = detailsBtn.attr("data-id");
        bindLogDetails(logId);
    });

function bindLogDetails(logId) {
    var url = logGetDetailsUrl + "?id=" + logId;
    $.ajax({
            type: "GET",
            url: url
        }).success(function(result) {
            $("#logDetailsContainer").html(result);
            $("#logDetails").modal("show");
        })
        .error(function (xhr, textStatus, error) {
            swal({
                title: "Error!",
                text: xhr.statusText,
                type: textStatus
            });
        });
}

$("body").on("click", ".del-log", function (e) {
    var delBtn = $(e.currentTarget);
    e.preventDefault();

    swal({
        title: "Are you sure?",
        text: "You are about to delete this log.",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!"
    }).then(function () {
        var innerSpan = $(delBtn.get(0).firstElementChild);
        var logId = delBtn.attr("data-id");
        delBtn.addClass("disabled");
        innerSpan.addClass("fa-spin");

        $.ajax({
            url: logDelUrl,
            data: { "id": logId },
            dataType: "json",
            success: function (result) {
                if (result.success === true) {
                    swal({
                        title: "Deleted!",
                        text: result.message,
                        type: "success"
                    });
                    refreshLogs();
                } else {
                    swal({
                        title: "Error!",
                        text: result.message,
                        type: "error"
                    });
                    delBtn.removeClass("disabled");
                    innerSpan.removeClass("fa-spin");
                }
            },
            error: function (xhr, textStatus, error) {
                swal({
                    title: "Error!",
                    text: xhr.statusText,
                    type: textStatus
                });
                delBtn.removeClass("disabled");
                innerSpan.removeClass("fa-spin");
            }
        });
    }).done();
});